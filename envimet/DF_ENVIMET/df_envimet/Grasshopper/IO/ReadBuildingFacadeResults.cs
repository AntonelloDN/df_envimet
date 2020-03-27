using System;
using df_envimet.Grasshopper.UI_GH;
using Grasshopper.Kernel;
using df_envimet_lib.IO;
using System.Collections.Generic;
using Grasshopper;
using GHD = Grasshopper.Kernel.Data;
using System.IO;
using System.Linq;

namespace df_envimet.Grasshopper.IO
{
    public class ReadBuildingFacadeResults : ComponentWithDirectionLogic
    {

        public ReadBuildingFacadeResults()
          : base("DF Envimet Read Building Facade Results", "DFReadBuildingFacadeResults",
              "Use this component to read building facade results.\nConnect output of this component to LB Recolor Mesh to visualize data.",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_27_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_buildingFiles", "_buildingFiles", "Output files about buildings. Connect 'DF Envimet Building Files' output here.", GH_ParamAccess.list);
            pManager.AddTextParameter("_buildingBinaryFiles", "_buildingBinaryFiles", "Output binary files about buildings. Connect 'DF Envimet Building Files' output here.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("_variable_", "_variable_", "Connect an integer:\n0 = Wall: Temperature Node 1 / outside(°C)\n" +
            "1 = Wall: Temperature Node 2 (°C)\n" +
            "2 = Wall: Temperature Node 3 (°C)\n" +
            "3 = Wall: Temperature Node 4 (°C)\n" +
            "4 = Wall: Temperature Node 5 (°C)\n" +
            "5 = Wall: Temperature Node 6 (°C)\n" +
            "6 = Wall: Temperature Node 7/ inside (°C)\n" +
            "7 = Building: Sum Humidity Flux at facade (g/s*m3)\n" +
            "8 = Wall: Longwave radiation emitted by facade (W/m2)\n" +
            "9 = Wall: Wind Speed in front of facade (m/s)\n" +
            "10 = Wall: Air Temperature in front of facade (°C)\n" +
            "11 = Wall: Shortwave radiation received at facade (W/m2)\n" +
            "12 = Wall: Absorbed direct shortwave radiation (W/m2)\n" +
            "13 = Wall: Incoming longwave radiation (W/m2)\n" +
            "14 = Wall: Reflected shortwave radiation facade (W/m2)\n" +
            "15 = Wall: Sensible Heat transmission coefficient outside (W/m2K)\n" +
            "16 = Wall: Longwave Energy Balance (W/m2)\n" +
            "17 = N.N.\n" +
            "18 = Building: Temperature of building (inside) (°C)\n" +
            "19 = Building: Reflected shortwave radiation (W/m2)\n" +
            "20 = Building: Longwave radiation emitted (W/m2)\n" +
            "21 = Greening: Temperature Leafs (°C)\n" +
            "22 = Greening: Air Temperature Canopy (°C)\n" +
            "23 = Greening: Air Humidity Canopy (g/kg)\n" +
            "24 = Greening: Longwave radiation emitted (two-side) (W/m2)\n" +
            "25 = Greening: Wind Speed in front of greening (m/s)\n" +
            "26 = Greening: Air Temperature in front of greening (°C)\n" +
            "27 = Greening: Shortwave radiation received at greening (W/m2)\n" +
            "28 = Greening: Incoming longwave radiation (two-side) (W/m2)\n" +
            "29 = Greening: Reflected shortwave radiation (W/m2)\n" +
            "30 = Greening: Transpiration Flux (g/s*m3)\n" +
            "31 = Greening: Stomata Resistance (s/m)\n" +
            "32 = Greening: Water access factor ()\n" +
            "33 = Substrate: Temperature Node 1/ outside (°C)\n" +
            "34 = Substrate: Temperature Node 2 (°C)\n" +
            "35 = Substrate: Temperature Node 3 (°C)\n" +
            "36 = Substrate: Temperature Node 4 (°C)\n" +
            "37 = Substrate: Temperature Node 5 (°C)\n" +
            "38 = Substrate: Temperature Node 6 (°C)\n" +
            "39 = Substrate: Temperature Node 7/ inside (°C)\n" +
            "40 = Substrate: Surface humidity (g/kg)\n" +
            "41 = Substrate: Humidity Flux at substrate (g/s*m3)\n" +
            "42 = Substrate: Longwave radiation emitted by substrate (W/m2)\n" +
            "43 = Substrate: Wind Speed in front of substrate (m/s)\n" +
            "44 = Substrate: Air Temperature in front of substrate (°C)\n" +
            "45 = Substrate: Shortwave radiation received at substrate (W/m2)\n" +
            "46 = Substrate: Absorbed direct shortwave radiation (W/m2)\n" +
            "47 = Substrate: Incoming longwave radiation (W/m2)\n" +
            "48 = Substrate: Reflected shortwave radiation substrate (W/m2)", GH_ParamAccess.item);
            pManager.AddBooleanParameter("runIt_", "runIt_", "Set runIt to True to read output.", GH_ParamAccess.item, false);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("fileName", "fileName", "Name of file you are reading.", GH_ParamAccess.tree);
            pManager.AddTextParameter("variableName", "variableName", "Name of variable you are reading.", GH_ParamAccess.tree);
            pManager.AddGenericParameter("analysisResult", "analysisResult", "A numerical data set whose length corresponds to the number of faces in the _inputMesh.  This data will be used to re-color the _inputMesh.\nUse it with LB Recolor Mesh.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> _buildingFiles = new List<string>();
            List<string> _buildingBinaryFiles = new List<string>();
            int _val_ = 0;
            bool runIt_ = false;

            DA.GetDataList(0, _buildingFiles);
            DA.GetDataList(1, _buildingBinaryFiles);
            DA.GetData(2, ref _val_);
            DA.GetData(3, ref runIt_);

            // Unwrap variables
            List<FacadeVariable> variables = BuildingFacadeOutputMapping();

            if (runIt_)
            {
                DataTree<string> fileNameTree = new DataTree<string>();
                DataTree<FacadeVariable> variableTree = new DataTree<FacadeVariable>();
                DataTree<double> dataTree = new DataTree<double>();

                // Warning!
                if (_val_ >= variables.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Variable is out of range, check description of the input.");
                    return;
                }

                for (int i = 0; i < _buildingFiles.Count; i++)
                {
                    GHD.GH_Path pth = new GHD.GH_Path(i);
                    try
                    {
                        string edxName = Path.GetFileNameWithoutExtension(_buildingFiles[i]);
                        string edtName = Path.GetFileNameWithoutExtension(_buildingBinaryFiles[i]);

                        if (edxName == edtName)
                        {
                            List<double> results = BuildingOutput.ParseBinBuilding(_buildingFiles[i], _buildingBinaryFiles[i], (int)variables[_val_], Direction);

                            fileNameTree.Add(Path.GetFileName(_buildingFiles[i]), pth);
                            variableTree.Add(variables[_val_], pth);
                            dataTree.AddRange(results, pth);
                        }
                    }
                    catch
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Something is wrong in output dynamic folder.\nPlease, make sure EDT length is equals to EDX length.");
                        continue;
                    }
                }

                DA.SetDataTree(0, fileNameTree);
                DA.SetDataTree(1, variableTree);
                DA.SetDataTree(2, dataTree);
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
                return Properties.Resources.envimetBuildingsFacadeReadIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3f4084bd-390a-41cc-9a88-880e750d5daa"); }
        }


        private static List<FacadeVariable> BuildingFacadeOutputMapping()
        {
            List<FacadeVariable> availableValues = new List<FacadeVariable>();

            switch (true)
            {
                default:
                    availableValues = Enum.GetValues(typeof(FacadeVariable))
                        .OfType<FacadeVariable>()
                        .ToList();
                    break;
            }

            return availableValues;
        }
    }
}