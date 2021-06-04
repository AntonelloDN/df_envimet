using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using df_envimet_lib.Settings;
using df_envimet_lib.IO;
using System.IO;
using Rhino;

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
            this.Message = "VER 0.0.04\nJUN_06_2021";
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("_mainSettings", "_mainSettings", "Basic setting of the simulation file which comes from \"DF Envimet Config MainSettings\" component..", GH_ParamAccess.item);
            pManager.AddGenericParameter("otherSettings_", "otherSettings_", "Other settings to customize simulation file."
                 + "\nConnect other simulation output among:"
                 + "\nDF Simple Forcing"
                 + "\nDF Full Forcing"
                 + "\nDF Thread"
                 + "\nDF Timestep"
                 + "\nDF Timing"
                 + "\nDF SoilSettings"
                 + "\nDF Pollutant"
                 + "\nDF Turbolence"
                 + "\nDF Output Settings"
                 + "\nDF Clouds"
                 + "\nDF Pollutant Concentration"
                 + "\nDF SolarAdjust"
                 + "\nDF Building Settings"
                 + "\nDF IVS"
                 + "\nDF Parallel Calculation"
                 + "\nDF SOR"
                 + "\nDF Averaged Inflow"
                 + "\nDF Wind Resistance"
                 + "\nDF Tree Settings", GH_ParamAccess.list);
            pManager.AddBooleanParameter("_runIt", "_runIt", "Set runIt to True to write SIMX file.\nDefault value is \"False\".", GH_ParamAccess.item, false);
            pManager[1].Optional = true;
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
            MainSettings _mainSettings = null;
            List<Configuration> otherSettings_ = new List<Configuration> ();
            bool _runIt = false;

            DA.GetData(0, ref _mainSettings);
            DA.GetDataList(1, otherSettings_);
            DA.GetData(2, ref _runIt);

            SimpleForcingSettings simpleForcingSettings = null;
            TThread tThread = null;
            TimeSteps timeSteps = null;
            ModelTiming modelTiming = null;
            SoilConfig soilConfig = null;
            Sources sources = null;
            Turbulence turbulence = null;
            OutputSettings outputSettings = null;
            Cloud cloud = null;
            Background background = null;
            SolarAdjust solarAdjust = null;
            BuildingSettings buildingSettings = null;
            IVS ivs = null;
            ParallelCPU parallelCPU = null;
            SOR sor = null;
            InflowAvg inflowAvg = null;
            Facades facades = null;
            PlantSetting plantSetting = null;
            LBC lbc = null;
            FullForcing fullForcing = null;

            if (_runIt)
            {
                try
                {
                    foreach (Configuration o in otherSettings_)
                    {
                        Type obj = o.GetType();

                        if (obj == typeof(SimpleForcingSettings))
                            simpleForcingSettings = o as SimpleForcingSettings;
                        else if (obj == typeof(TThread))
                            tThread = o as TThread;
                        else if (obj == typeof(TimeSteps))
                            timeSteps = o as TimeSteps;
                        else if (obj == typeof(ModelTiming))
                            modelTiming = o as ModelTiming;
                        else if (obj == typeof(SoilConfig))
                            soilConfig = o as SoilConfig;
                        else if (obj == typeof(Sources))
                            sources = o as Sources;
                        else if (obj == typeof(Turbulence))
                            turbulence = o as Turbulence;
                        else if (obj == typeof(OutputSettings))
                            outputSettings = o as OutputSettings;
                        else if (obj == typeof(Cloud))
                            cloud = o as Cloud;
                        else if (obj == typeof(Background))
                            background = o as Background;
                        else if (obj == typeof(SolarAdjust))
                            solarAdjust = o as SolarAdjust;
                        else if (obj == typeof(BuildingSettings))
                            buildingSettings = o as BuildingSettings;
                        else if (obj == typeof(IVS))
                            ivs = o as IVS;
                        else if (obj == typeof(ParallelCPU))
                            parallelCPU = o as ParallelCPU;
                        else if (obj == typeof(SOR))
                            sor = o as SOR;
                        else if (obj == typeof(InflowAvg))
                            inflowAvg = o as InflowAvg;
                        else if (obj == typeof(Facades))
                            facades = o as Facades;
                        else if (obj == typeof(PlantSetting))
                            plantSetting = o as PlantSetting;
                        else if (obj == typeof(LBC))
                            lbc = o as LBC;
                        else if (obj == typeof(FullForcing))
                            fullForcing = o as FullForcing;
                    }


                    Simx simx = new Simx(_mainSettings)
                    {
                        SimpleForcing = simpleForcingSettings,
                        TThread = tThread,
                        TimeSteps = timeSteps,
                        ModelTiming = modelTiming,
                        SoilSettings = soilConfig,
                        Sources = sources,
                        Turbulence = turbulence,
                        OutputSettings = outputSettings,
                        Cloud = cloud,
                        Background = background,
                        SolarAdjust = solarAdjust,
                        BuildingSettings = buildingSettings,
                        IVS = ivs,
                        ParallelCPU = parallelCPU,
                        SOR = sor,
                        InflowAvg = inflowAvg,
                        Facades = facades,
                        PlantSetting = plantSetting,
                        LBC = lbc,
                        FullForcing = fullForcing
                    };

                    simx.WriteSimx();
                    DA.SetData(0, Path.Combine(Path.GetDirectoryName(_mainSettings.Inx), _mainSettings.Name + ".simx"));
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