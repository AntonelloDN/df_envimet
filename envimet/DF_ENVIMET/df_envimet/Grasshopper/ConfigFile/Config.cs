using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;
using System.Xml;
using System.Text;
using df_envimet_lib.IO;

namespace df_envimet.Grasshopper.ConfigFile
{
    public class Config : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Config()
          : base("DF Envimet Config", "DFenvimetConfig",
              "This component writes simulation file (SIMX) of ENVI_MET.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("_mainSettings", "_mainSettings", "Basic setting of the simulation file which comes from \"DF Envimet Config MainSettings\" component..", GH_ParamAccess.item);
            pManager.AddGenericParameter("_simpleForcing", "_simpleForcing", "Simple forcing condition which comes from \"DF Envimet Config SimpleForcing\" component.", GH_ParamAccess.item);
            pManager.AddGenericParameter("timestepsSettings_", "timestepsSettings_", "Timestep settings, important for radiation. it comes from \"DF Envimet Timesteps Settings\" component.", GH_ParamAccess.item);
            pManager.AddGenericParameter("buildingTemp_", "buildingTemp_", "Building indoor temperature setting. Output of \"DF Envimet Building Temp\" component.", GH_ParamAccess.item);
            pManager.AddGenericParameter("soilSettings_", "soilSettings_", "Initial condition to use for soil. Output of \"DF Envimet Soil Settings\" component.", GH_ParamAccess.item);
            pManager.AddGenericParameter("outputSettings_", "outputSettings_", "Output interval of files. Output of \"DF Envimet Output interval Settings\" component.", GH_ParamAccess.item);

            pManager.AddBooleanParameter("parallel_", "parallel_", "Set True to increase the speed of calculation of ENVI_MET.\nThis option does not work with Basic version.\nDefault value is \"True\".", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set runIt to True to write SIMX file.\nDefault value is \"False\".", GH_ParamAccess.item, false);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("SIMXfileAddress", "SIMXfileAddress", "The file path of the simx file that has been generated on your machine.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // INPUT
            // declaration
            List<double> _dryBulbTemperature = new List<double>();
            List<double> _relativeHumidity = new List<double>();

            BuildingSetting buildingTemp = null;
            SoilTemp soilSettings = null;
            OutputTiming outputTiming = null;


            bool parallel_ = false;
            bool _runIt = false;

            MainSettings baseSetting = new MainSettings();

            TimeStepsSettings timestepsSettings = new TimeStepsSettings();



            if (buildingTemp != null)
            {
                buildingTemp = new BuildingSetting();
            }

            if (soilSettings != null)
            {
                soilSettings = new SoilTemp();
            }

            if (outputTiming != null)
            {
                outputTiming = new OutputTiming();
            }


            SampleForcingSettings simpleForcing = new SampleForcingSettings(_dryBulbTemperature, _relativeHumidity);

            DA.GetData(0, ref baseSetting);
            DA.GetData(1, ref simpleForcing);
            DA.GetData(2, ref timestepsSettings);
            DA.GetData(3, ref buildingTemp);
            DA.GetData(4, ref soilSettings);
            DA.GetData(5, ref outputTiming);

            DA.GetData(6, ref parallel_);
            DA.GetData(7, ref _runIt);

            // action

            if (_runIt)
            {
                try
                {
                    var now = DateTime.Now;
                    string revisionDate = now.ToString("yyyy.MM.dd HH:mm:ss");
                    string destination = System.IO.Path.GetDirectoryName(baseSetting.INXfileAddress);
                    string fileName = System.IO.Path.Combine(destination, baseSetting.SimName + ".simx");
                    string[] empty = { };
                    int simulationDuration = baseSetting.SimDuration;

                    XmlTextWriter xWriter = new XmlTextWriter(fileName, Encoding.UTF8);

                    // root
                    xWriter.WriteStartElement("ENVI-MET_Datafile");
                    xWriter.WriteString("\n ");

                    // contents
                    // Header section
                    string headerTitle = "Header";
                    string[] headerTag = new string[] { "filetype", "version", "revisiondate", "remark", "encryptionlevel" };
                    string[] headerValue = new string[] { "SIMX", "1", revisionDate, "Created with lb_envimet", "0" };

                    Inx.CreateXmlSection(xWriter, headerTitle, headerTag, headerValue, 0, empty);


                    // Main section
                    string mainTitle = "mainData";
                    string[] mainTag = new string[] { "simName", "INXFile", "filebaseName", "outDir", "startDate", "startTime", "simDuration", "windSpeed", "windDir", "z0", "T_H", "Q_H", "Q_2m" };
                    string[] mainValue = new string[]
                      { baseSetting.SimName,
                    baseSetting.INXfileAddress,
                    baseSetting.SimName,
                    " ",
                    baseSetting.StartDate,
                    baseSetting.StartTime,
                    simulationDuration.ToString(),
                    baseSetting.WindSpeed.ToString(),
                    baseSetting.WindDir.ToString(),
                    baseSetting.Roughness.ToString(),
                    baseSetting.InitialTemperature.ToString(),
                    baseSetting.SpecificHumidity.ToString(),
                    baseSetting.RelativeHumidity.ToString()
                      };

                    Inx.CreateXmlSection(xWriter, mainTitle, mainTag, mainValue, 0, empty);


                    // SimpleForcing section
                    string sfTitle = "SimpleForcing";
                    string[] sfTag = new string[] { "TAir", "Qrel" };
                    string[] sfValue = new string[] { simpleForcing.Temperature, simpleForcing.RelativeHumidity };

                    Inx.CreateXmlSection(xWriter, sfTitle, sfTag, sfValue, 0, empty);

                    // Parallel section
                    if (parallel_)
                    {
                        string parallelTitle = "Parallel";
                        string[] parallelTag = new string[] { "CPUdemand" };
                        string[] parallelValue = new string[] { "ALL" };

                        Inx.CreateXmlSection(xWriter, parallelTitle, parallelTag, parallelValue, 0, empty);
                    }


                    // TimeSteps section
                    if (timestepsSettings.Dt_step02 != 0)
                    {
                        string timestepTitle = "TimeSteps";
                        string[] timestepTag = new string[] { "sunheight_step01", "sunheight_step02", "dt_step00", "dt_step01", "dt_step02" };
                        string[] timestepValue = new string[] { timestepsSettings.Sunheight_step01.ToString(), timestepsSettings.Sunheight_step02.ToString(), timestepsSettings.Dt_step00.ToString(), timestepsSettings.Dt_step01.ToString(), timestepsSettings.Dt_step02.ToString() };

                        Inx.CreateXmlSection(xWriter, timestepTitle, timestepTag, timestepValue, 0, empty);
                    }


                    // Building section
                    if (buildingTemp != null)
                    {
                        string buildingTempTitle = "Building";
                        string[] buildingTempTag = new string[] { "indoorTemp", "indoorConst" };
                        string[] buildingTempValue = new string[] { buildingTemp.IndoorTemp.ToString(), buildingTemp.IndoorConst.ToString() };

                        Inx.CreateXmlSection(xWriter, buildingTempTitle, buildingTempTag, buildingTempValue, 0, empty);
                    }


                    // Soil section
                    if (soilSettings != null)
                    {
                        string soilTitle = "Soil";
                        string[] soilTag = new string[] { "tempUpperlayer", "tempMiddlelayer", "tempDeeplayer", "tempBedrockLayer", "waterUpperlayer", "waterMiddlelayer", "waterDeeplayer", "waterBedrockLayer" };
                        string[] soilValue = new string[] { soilSettings.TempUpperlayer.ToString(), soilSettings.TempMiddlelayer.ToString(), soilSettings.TempDeeplayer.ToString(), soilSettings.TempBedrockLayer.ToString(), soilSettings.WaterUpperlayer.ToString(), soilSettings.WaterMiddlelayer.ToString(), soilSettings.WaterDeeplayer.ToString(), soilSettings.WaterBedrockLayer.ToString() };

                        Inx.CreateXmlSection(xWriter, soilTitle, soilTag, soilValue, 0, empty);
                    }

                    // OutputTiming section
                    if (outputTiming != null)
                    {
                        string outputTimingTitle = "OutputTiming";
                        string[] outputTimingTag = new string[] { "mainFiles", "textFiles", "inclNestingGrids" };
                        string[] outputTimingValue = new string[] { outputTiming.MainFiles.ToString(), outputTiming.TextFiles.ToString(), outputTiming.InclNestingGrids.ToString() };

                        Inx.CreateXmlSection(xWriter, outputTimingTitle, outputTimingTag, outputTimingValue, 0, empty);
                    }


                    // close root and file
                    xWriter.WriteEndElement();
                    xWriter.Close();

                    DA.SetData(0, fileName);
                }
                catch
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please provide a valid mainSettings.");
                }
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.envimetConfig;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("06dc4bf3-1257-4d27-b777-b876c89effad"); }
        }
    }
}