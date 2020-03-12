using System;
using System.Collections.Generic;
using df_envimet_lib.IO;
using GHD = Grasshopper.Kernel.Data;
using Grasshopper.Kernel;
using Grasshopper;

namespace df_envimet.Grasshopper.IO
{
    public class ReadBuildingAvgResults : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ReadBuildingAvgResults class.
        /// </summary>
        public ReadBuildingAvgResults()
          : base("DF Envimet Read Building Avg Results", "DFReadBuildingAvgResults",
              "Use this component to read building results. (WIP)",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nFEB_29_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_buildingFiles", "_buildingFiles", "Output files of each simulated building. Connect avg output of 'DF Envimet Building Files' output here.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("_buildingVariable_", "_buildingVariable_", "Connect a number:" +
                "\n0 = Simtime" +
                "\n1 = Date" +
                "\n2 = Time" +
                "\n3 = Indoor Air Temperature (C)" +
                "\n4 = Average Air Temperature at Facade (K)" +
                "\n5 = Energy flux To Indoor Sum" +
                "\n6 = Energy flux To Indoor Average" +
                "\n7 = Energy flux Conduction Average" +
                "\n8 = Energy flux SW Transmission Average" +
                "\n9 = Shortwave Availabe" +
                "\n10 = Shortwave Absorbed" +
                "\n11 = Longwave Available" +
                "\n12 = Longwave Emitted Facade" +
                "\n13 = Sensible Outside" +
                "\n14 = Vapour From Greening" +
                "\n15 = Vapour From Substrate", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("runIt_", "runIt_", "Set runIt to True to read output.", GH_ParamAccess.item, false);
            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("variableName", "variableName", "Variable you are reading", GH_ParamAccess.tree);
            pManager.AddGenericParameter("date", "date", "Ouput dates", GH_ParamAccess.tree);
            pManager.AddGenericParameter("time", "time", "Ouput times", GH_ParamAccess.tree);
            pManager.AddGenericParameter("analysisResult", "analysisResult", "Ouput values", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> _buildingFiles = new List<string>();
            int _val_ = 0;
            bool runIt_ = false;

            DA.GetDataList(0, _buildingFiles);
            DA.GetData(1, ref _val_);
            DA.GetData(2, ref runIt_);


            // Unwrap variables
            var variables = BuildingOutputTypeMapping();


            if (runIt_)
            {
                DataTree<string> valueTree = new DataTree<string>();
                DataTree<string> dateTree = new DataTree<string>();
                DataTree<string> timeTree = new DataTree<string>();
                DataTree<BuldingDatVariable> variableNameTree = new DataTree<BuldingDatVariable>();

                // Warning!
                if (_val_ >= variables.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Variable is out of range, check description of the input.");
                    return;
                }

                //TO DO: Generic class for csv method
                for (int i = 0; i < _buildingFiles.Count; i++)
                {
                    GHD.GH_Path pth = new GHD.GH_Path(i);

                    valueTree.AddRange(ReceptorOutput.GetValueFromCsv(_buildingFiles[i], (int) variables[_val_]), pth);
                    dateTree.AddRange(ReceptorOutput.GetValueFromCsv(_buildingFiles[i], (int)BuldingDatVariable.Date), pth);
                    timeTree.AddRange(ReceptorOutput.GetValueFromCsv(_buildingFiles[i], (int)BuldingDatVariable.Time), pth);
                    variableNameTree.Add(variables[_val_], pth);
                }

                DA.SetDataTree(0, variableNameTree);
                DA.SetDataTree(1, dateTree);
                DA.SetDataTree(2, timeTree);
                DA.SetDataTree(3, valueTree);
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
                return Properties.Resources.envimetReadBuildingsAvg;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("05903954-7582-48da-9a74-23b1b03ccedc"); }
        }

        private static List<BuldingDatVariable> BuildingOutputTypeMapping()
        {
            List<BuldingDatVariable> availableValues = new List<BuldingDatVariable>();

            switch (true)
            {
                default:
                    availableValues.AddRange(new List<BuldingDatVariable>
                    {
                        BuldingDatVariable.Simtime,
                        BuldingDatVariable.Date,
                        BuldingDatVariable.Time,
                        BuldingDatVariable.IndoorTemp,
                        BuldingDatVariable.Av_Tair_at_facade,
                        BuldingDatVariable.EnergyfluxToIndoorSum,
                        BuldingDatVariable.EnergyfluxToIndoorAverage,
                        BuldingDatVariable.EnergyfluxConductionAverage,
                        BuldingDatVariable.EnergyfluxSWTransmissionAverage,
                        BuldingDatVariable.ShortwaveAvailabe,
                        BuldingDatVariable.ShortwaveAbsorbed,
                        BuldingDatVariable.LongwaveAvailable,
                        BuldingDatVariable.LongwaveEmittedFacade,
                        BuldingDatVariable.Sensibleoutside,
                        BuldingDatVariable.VapourFromGreening,
                        BuldingDatVariable.VapourFromSubstrate
                    });
                    break;
            }

            return availableValues;
        }

    }
}