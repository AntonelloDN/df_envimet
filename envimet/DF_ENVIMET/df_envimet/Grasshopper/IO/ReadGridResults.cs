using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using df_envimet.Grasshopper.UI_GH;
using df_envimet_lib.IO;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using GHD = Grasshopper.Kernel.Data;

namespace df_envimet.Grasshopper.IO
{
    public class ReadGridResults : ComponentWithDirectionLogic
    {
        /// <summary>
        /// Initializes a new instance of the ReadGrid class.
        /// </summary>
        public ReadGridResults()
          : base("DF Envimet Read Grid Results", "DFReadGridResults",
              "Use this component to read grid results.\nConnect output of this component to LB Recolor Mesh to visualize data.\n" +
                "If there are some difects, please open a discussion on Envimet Board or Ladybug Tools forum.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nMAR_07_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_gridFiles", "_gridFiles", "Grid files. Connect 'DF Envimet Grid Folder' output here.", GH_ParamAccess.list);
            pManager.AddTextParameter("_gridBinaryFiles", "_gridBinaryFiles", "Grid binary files. Connect 'DF Envimet Grid Folder' output here.", GH_ParamAccess.list);
            pManager.AddTextParameter("_type", "_type", "Output type from 'DF Envimet Grid Folder'.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("_variable_", "_variable_", "Connect an integer:\n0 = Flow u(m / s)\n" +
                "1 = Flow v (m/s)\n" +
                "2 = Flow w (m/s)\n" +
                "3 = Wind Speed (m/s)\n" +
                "4 = Wind Speed Change ()\n" +
                "5 = Wind Direction (deg)\n" +
                "6 = Pressure Perturbation (Diff)\n" +
                "7 = Air Temperature (C)\n" +
                "8 = Air Temperature Delta (K)\n" +
                "9 = Air Temperature Change (K/h)\n" +
                "10 = Spec. Humidity (g/kg)\n" +
                "11 = Relative Humidity ()\n" +
                "12 = TKE (m/m)\n" +
                "13 = Dissipation (m/m)\n" +
                "14 = Vertical Exchange Coef. Impuls (m/s)\n" +
                "15 = Horizontal Exchange Coef. Impuls (m/s)\n" +
                "16 = Vegetation LAD (m/m)\n" +
                "17 = Direct Sw Radiation (W/m)\n" +
                "18 = Diffuse Sw Radiation (W/m)\n" +
                "19 = Reflected Sw Radiation (W/m)\n" +
                "20 = Temperature Flux (Km/s)\n" +
                "21 = Vapour Flux (g/kgm/s)\n" +
                "22 = Water on Leafes (g/m)\n" +
                "23 = Leaf Temperature (C)\n" +
                "24 = Local Mixing Length (m)\n" +
                "25 = Mean Radiant Temp. (C)\n" +
                "26 = TKE normalised 1D ( )\n" +
                "27 = Dissipation normalised 1D ( )\n" +
                "28 = Km normalised 1D ( )\n" +
                "29 = TKE Mechanical Turbulence Prod. ( )\n" +
                "30 = Stomata Resistance (s/m)\n" +
                "31 = CO2 (mg/m3)\n" +
                "32 = CO2 (ppm)\n" +
                "33 = Plant CO2 Flux (mg/kgm/s)\n" +
                "34 = Div Rlw Temp change (K/h)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("_x_", "_x_", "Connect and integer to set position of the section plane.", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("runIt_", "runIt_", "Set runIt to True to read output.", GH_ParamAccess.item, false);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("report", "report", "Some information about grid.", GH_ParamAccess.item);
            pManager.AddVectorParameter("vector", "vector", "offset distance of analysis plane.", GH_ParamAccess.item);
            pManager.AddGenericParameter("fileName", "fileName", "Name of file you are reading.", GH_ParamAccess.tree);
            pManager.AddTextParameter("variableName", "variableName", "Name of variable you are reading.", GH_ParamAccess.tree);
            pManager.AddGenericParameter("analysisResult", "analysisResult", "A numerical data set whose length corresponds to the number of faces in the _inputMesh.  This data will be used to re-color the _inputMesh.\nUse it with LB Recolor Mesh.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> gridFiles = new List<string>();
            List<string> gridBinaryFiles = new List<string>();
            string type = GridOutputFolderType.ATMOSPHERE;
            int var = 0;
            int loc = 0;
            bool runIt = false;

            DA.GetDataList(0, gridFiles);
            DA.GetDataList(1, gridBinaryFiles);
            DA.GetData(2, ref type);
            DA.GetData(3, ref var);
            DA.GetData(4, ref loc);
            DA.GetData(5, ref runIt);

            GetNameOfPlaneInput();
            ChangeVariableDescription(type);


            if (runIt)
            {
                var variables = GetVariable();
                int selectedVariable = 0;
                object varName = null;
                string report = String.Empty;

                switch (type)
                {
                    case GridOutputFolderType.ATMOSPHERE:
                        var atmosphere = variables[0] as List<AtmosphereOutput>;
                        // Warning
                        if (var >= atmosphere.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Atmosphere variable is out of range, check description of the input.");
                            return;
                        }
                        selectedVariable = (int)(atmosphere)[var];
                        varName = atmosphere[var];
                        break;
                    case GridOutputFolderType.POLLUTANTS:
                        var pollutant = variables[1] as List<PollutantsOutput>;
                        // Warning
                        if (var >= pollutant.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Pollutants variable is out of range, check description of the input.");
                            return;
                        }
                        selectedVariable = (int)(pollutant)[var];
                        varName = pollutant[var];
                        break;
                    case GridOutputFolderType.RADIATION:
                        var radiation = variables[2] as List<RadiationOutput>;
                        // Warning
                        if (var >= radiation.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Radiation variable is out of range, check description of the input.");
                            return;
                        }
                        selectedVariable = (int)(radiation)[var];
                        varName = radiation[var];
                        break;
                    case GridOutputFolderType.SOIL:
                        var soil = variables[3] as List<SoilOutput>;
                        // Warning
                        if (var >= soil.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Soil variable is out of range, check description of the input.");
                            return;
                        }
                        selectedVariable = (int)(soil)[var];
                        varName = soil[var];
                        break;
                    case GridOutputFolderType.SURFACE:
                        var surface = variables[4] as List<SurfaceOutput>;
                        // Warning
                        if (var >= surface.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Soil variable is out of range, check description of the input.");
                            return;
                        }
                        FlatResults();
                        loc = 0;
                        selectedVariable = (int)(surface)[var];
                        varName = surface[var];
                        break;
                    case GridOutputFolderType.SOLAR_ACCESS:
                        var solar = variables[5] as List<SolarAccessOutput>;
                        // Warning
                        if (var >= solar.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Solar access variable is out of range, check description of the input.");
                            return;
                        }
                        FlatResults();
                        selectedVariable = (int)(solar)[var];
                        varName = solar[var];
                        break;
                    case GridOutputFolderType.VEGETATIONS:
                        var vegetation = variables[6] as List<VegetationOutput>;
                        // Warning
                        if (var >= vegetation.Count)
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Vegetation variable is out of range, check description of the input.");
                            return;
                        }
                        selectedVariable = (int)(vegetation)[var];
                        varName = vegetation[var];
                        break;
                }

                DataTree<string> fileNameTree = new DataTree<string>();
                DataTree<object> variableTree = new DataTree<object>();
                DataTree<double> dataTree = new DataTree<double>();

                for (int i = 0; i < gridFiles.Count; i++)
                {
                    GHD.GH_Path pth = new GHD.GH_Path(i);
                    try
                    {
                        string edxName = Path.GetFileNameWithoutExtension(gridFiles[i]);
                        string edtName = Path.GetFileNameWithoutExtension(gridBinaryFiles[i]);

                        if (edxName == edtName)
                        {
                            List<double> results = GridOutput.ParseBinGrid(gridFiles[i], gridBinaryFiles[i], selectedVariable, loc, Direction);


                            if (results == null)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Location of plane is out of range.");
                                return;
                            }

                            report = GridOutput.GetReport(gridFiles[i]);
                            DA.SetData(0, report);
                            Vector3d vector = GridOutput.GetVectorPlaneFromEdx(gridFiles[i], Direction, loc);
                            DA.SetData(1, vector);

                            fileNameTree.Add(Path.GetFileName(gridFiles[i]), pth);
                            variableTree.Add(varName, pth);
                            dataTree.AddRange(results, pth);
                        }
                        else
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please, check output folder you are reading. EDT file name and EDX file name should be the same.");
                        }
                    }
                    catch
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Something is wrong in output dynamic folder.\nPlease, make sure EDT length is equals to EDX length.");
                        continue;
                    }
                }

                DA.SetDataTree(2, fileNameTree);
                DA.SetDataTree(3, variableTree);
                DA.SetDataTree(4, dataTree);
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
                return Properties.Resources.envimetGridResults;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("48074d92-c23a-43f6-a1a6-5dd9f4ad3f50"); }
        }


        private void FlatResults()
        {
            Direction = FaceDirection.Z; // Only Z
            Params.Input[4].Name = "_z_";
            Params.Input[4].NickName = "_z_";
        }

        private static List<object> GetVariable()
        {
            List<object> variables = new List<object>();

            List<AtmosphereOutput> atmosphere = Enum.GetValues(typeof(AtmosphereOutput))
                .OfType<AtmosphereOutput>()
                .ToList();
            List<PollutantsOutput> pollutants = Enum.GetValues(typeof(PollutantsOutput))
                .OfType<PollutantsOutput>()
                .ToList();
            List<RadiationOutput> radiation = Enum.GetValues(typeof(RadiationOutput))
                .OfType<RadiationOutput>()
                .ToList();
            List<SoilOutput> soil = Enum.GetValues(typeof(SoilOutput))
                .OfType<SoilOutput>()
                .ToList();
            List<SurfaceOutput> surface = Enum.GetValues(typeof(SurfaceOutput))
                .OfType<SurfaceOutput>()
                .ToList();
            List<SolarAccessOutput> solar = Enum.GetValues(typeof(SolarAccessOutput))
                .OfType<SolarAccessOutput>()
                .ToList();
            List<VegetationOutput> vegetation = Enum.GetValues(typeof(VegetationOutput))
                .OfType<VegetationOutput>()
                .ToList();

            variables.Add(atmosphere);
            variables.Add(pollutants);
            variables.Add(radiation);
            variables.Add(soil);
            variables.Add(surface);
            variables.Add(solar);
            variables.Add(vegetation);

            return variables;
        }

        private void GetNameOfPlaneInput()
        {
            if (Direction == FaceDirection.Y)
            {
                Params.Input[4].Name = "_y_";
                Params.Input[4].NickName = "_y_";
            }
            else if (Direction == FaceDirection.Z)
            {
                Params.Input[4].Name = "_z_";
                Params.Input[4].NickName = "_z_";
            }
            else
            {
                Params.Input[4].Name = "_x_";
                Params.Input[4].NickName = "_x_";
            }
        }

        private void ChangeVariableDescription(string type)
        {
            switch (type)
            {
                case GridOutputFolderType.POLLUTANTS:
                    Params.Input[3].Description = "Connect an integer:\n0 = Objects\n" +
                            "1 = Flow u (m/s)\n" +
                            "2 = Flow v (m/s)\n" +
                            "3 = Flow w (m/s)\n" +
                            "4 = Wind Speed (m/s)\n" +
                            "5 = Wind Speed Change ()\n" +
                            "6 = Wind Direction (deg)\n" +
                            "7 = Pot. Temperature (K)\n" +
                            "8 = Spec. Humidity (g/kg)\n" +
                            "9 = Relative Humidity ()\n" +
                            "10 = Vertical Exchange Coef. Impuls (m/s)\n" +
                            "11 = Horizontal Exchange Coef. Impuls (m/s)\n" +
                            "12 = PM2.5 Concentration (g/m3)\n" +
                            "13 = PM2.5 Source (g/s)\n" +
                            "14 = PM2.5 Deposition velocity  (mm/s)\n" +
                            "15 = PM2.5 Total Deposed Mass (g/m2)\n" +
                            "16 = PM2.5 Reaction Term ()";
                    break;
                 case GridOutputFolderType.RADIATION:
                    Params.Input[3].Description = "Connect an integer:\n0 = Q_sw Direct (W/m)\n" +
                            "1 = Q_sw Direct Relative ()\n" +
                            "2 = Q_sw Diffuse (W/m)\n" +
                            "3 = Q_sw Reflected Upper Hemisphere (W/m)\n" +
                            "4 = Q_sw Reflected Lower Hemisphere (W/m)\n" +
                            "5 = Q_lw  Upper Hemisphere (W/m)\n" +
                            "6 = Q_lw  Lower Hemisphere (W/m)\n" +
                            "7 = ViewFactor Up Sky ( )\n" +
                            "8 = ViewFactor Up Buildings ( )\n" +
                            "9 = ViewFactor Up Vegetation ( )\n" +
                            "10 = ViewFactor Up Soil ( )\n" +
                            "11 = ViewFactor Down Sky ( )\n" +
                            "12 = ViewFactor Down Buildings ( )\n" +
                            "13 = ViewFactor Down Vegetation ( )\n" +
                            "14 = ViewFactor Down Soil ( )";
                    break;
                case GridOutputFolderType.SOIL:
                    Params.Input[3].Description = "Connect an integer:\n0 = Temperature (C)\n" +
                            "1 = Volumetic Water Content (m3 H20/m3)\n" +
                            "2 = Relative Soil Wetness related to sat ()\n" +
                            "3 = Local RAD (normalized) (m/m)\n" +
                            "4 = Local RAD Owner ()\n" +
                            "5 = Root Water Uptake (g H20/m31/s)";
                    break;
                case GridOutputFolderType.SURFACE:
                    Params.Input[3].Description = "Connect an integer:\n0 = Index Surface Grid ()\n" +
                            "1 = Soil Profile Type ()\n" +
                            "2 = z Topo (m)\n" +
                            "3 = Surface inclination ()\n" +
                            "4 = Surface exposition ()\n" +
                            "5 = Shadow Flag ()\n" +
                            "6 = T Surface (C)\n" +
                            "7 = T Surface Diff. (K)\n" +
                            "8 = T Surface Change (K/h)\n" +
                            "9 = q Surface (g/kg)\n" +
                            "10 = uv above Surface (m/s)\n" +
                            "11 = Sensible heat flux (W/m2)\n" +
                            "12 = Exchange Coeff. Heat (m2/s)\n" +
                            "13 = Latent heat flux (W/m2)\n" +
                            "14 = Soil heat Flux (W/m2)\n" +
                            "15 = Q_Sw Direct  (W/m2)\n" +
                            "16 = Q_Sw Direct Horizontal (W/m2)\n" +
                            "17 = Q_Sw Diffuse Horizontal (W/m2)\n" +
                            "18 = Q_Sw Reflected Received Horizontal (W/m2)\n" +
                            "19 = Lambert Factor ()\n" +
                            "20 = Q_Lw emitted (W/m2)\n" +
                            "21 = Q_Lw budget (W/m2)\n" +
                            "22 = Q_Lw from Sky (W/m2)\n" +
                            "23 = Q_Lw from Buildings  (W/m2)\n" +
                            "24 = Q_Lw from Vegetation  (W/m2)\n" +
                            "25 = Q_Lw Sum all Fluxes (W/m2)\n" +
                            "26 = Water Flux (g/(m2s))\n" +
                            "27 = SkyViewFaktor ()\n" +
                            "28 = Building Height (m)\n" +
                            "29 = Surface Albedo ()\n" +
                            "30 = Deposition Speed (mm/s)\n" +
                            "31 = Mass Deposed (g/m2)\n" +
                            "32 = z node Biomet ()\n" +
                            "33 = z Biomet (m)\n" +
                            "34 = T Air Biomet (C)\n" +
                            "35 = q Air Biomet (g/kg)\n" +
                            "36 = TMRT Biomet (C)\n" +
                            "37 = Wind Speed Biomet (m/s)\n" +
                            "38 = Mass Biomet (ug/m3)\n" +
                            "39 = Receptors ()";
                    break;
                case GridOutputFolderType.SOLAR_ACCESS:
                    Params.Input[3].Description = "Connect an integer:\n0 = z Topo (m)\n" +
                            "1 = Buildings (Flag)\n" +
                            "2 = Building Height (m)\n" +
                            "3 = Index z node Terrain ()\n" +
                            "4 = Index z node Biomet ()\n" +
                            "5 = z Biomet absolute (m)\n" +
                            "6 = SkyViewFactor  ()\n" +
                            "7 = Sun hours Terrain level (h)\n" +
                            "8 = Shadow hours Terrain level (h)\n" +
                            "9 = Sun hours Biomet level (h)\n" +
                            "10 = Shadow hours Biomet level (h)";
                    break;
                case GridOutputFolderType.VEGETATIONS:
                    Params.Input[3].Description = "Connect an integer:\n0 = Plant Index ( )\n" +
                            "1 = Plant Type ID ()\n" +
                            "2 = Flow u (m/s)\n" +
                            "3 = Flow v (m/s)\n" +
                            "4 = Flow w (m/s)\n" +
                            "5 = Wind Speed at Vegetation (m/s)\n" +
                            "6 = Wind Speed Change at Vegetation (%)\n" +
                            "7 = Wind Direction at Vegetation (deg)\n" +
                            "8 = Local Drag at Vegetation (N/m³)\n" +
                            "9 = Horizontal Drag at Vegetation (N)\n" +
                            "10 = Horizontal Drag at Vegetation Vertical Sum (N)\n" +
                            "11 = Air Temperature at Vegetation (°C)\n" +
                            "12 = Spec. Humidity at Vegetation (g/kg)\n" +
                            "13 = Relative Humidity at Vegetation (%)\n" +
                            "14 = TKE at Vegetation (m²/m³)\n" +
                            "15 = Vegetation LAD (m²/m³)\n" +
                            "16 = Direct Sw Radiation (W/m²)\n" +
                            "17 = Diffuse Sw Radiation (W/m²)\n" +
                            "18 = Reflected Sw Radiation (W/m²)\n" +
                            "19 = Mean Radiant Temp. (°C)\n" +
                            "20 = Temperature Flux (K*m/s)\n" +
                            "21 = Vapour Flux (g/kg*m/s)\n" +
                            "22 = Water on Leafes (g/m²)\n" +
                            "23 = Leaf Temperature (°C)\n" +
                            "24 = Aerodynamic Resistance (s/m)\n" +
                            "25 = Stomata Resistance (s/m)\n" +
                            "26 = Plant CO2 Flux (mg/kg*m/s)\n" +
                            "27 = Plant Isoprene Flux (mg/cell*h)\n" +
                            "28 = PAR (micro mol m-2 s-1)";
                    break;
                default:
                    Params.Input[3].Description = "Connect an integer:\n0 = Flow u(m / s)\n" +
                            "1 = Flow v (m/s)\n" +
                            "2 = Flow w (m/s)\n" +
                            "3 = Wind Speed (m/s)\n" +
                            "4 = Wind Speed Change ()\n" +
                            "5 = Wind Direction (deg)\n" +
                            "6 = Pressure Perturbation (Diff)\n" +
                            "7 = Air Temperature (C)\n" +
                            "8 = Air Temperature Delta (K)\n" +
                            "9 = Air Temperature Change (K/h)\n" +
                            "10 = Spec. Humidity (g/kg)\n" +
                            "11 = Relative Humidity ()\n" +
                            "12 = TKE (m/m)\n" +
                            "13 = Dissipation (m/m)\n" +
                            "14 = Vertical Exchange Coef. Impuls (m/s)\n" +
                            "15 = Horizontal Exchange Coef. Impuls (m/s)\n" +
                            "16 = Vegetation LAD (m/m)\n" +
                            "17 = Direct Sw Radiation (W/m)\n" +
                            "18 = Diffuse Sw Radiation (W/m)\n" +
                            "19 = Reflected Sw Radiation (W/m)\n" +
                            "20 = Temperature Flux (Km/s)\n" +
                            "21 = Vapour Flux (g/kgm/s)\n" +
                            "22 = Water on Leafes (g/m)\n" +
                            "23 = Leaf Temperature (C)\n" +
                            "24 = Local Mixing Length (m)\n" +
                            "25 = Mean Radiant Temp. (C)\n" +
                            "26 = TKE normalised 1D ( )\n" +
                            "27 = Dissipation normalised 1D ( )\n" +
                            "28 = Km normalised 1D ( )\n" +
                            "29 = TKE Mechanical Turbulence Prod. ( )\n" +
                            "30 = Stomata Resistance (s/m)\n" +
                            "31 = CO2 (mg/m3)\n" +
                            "32 = CO2 (ppm)\n" +
                            "33 = Plant CO2 Flux (mg/kgm/s)\n" +
                            "34 = Div Rlw Temp change (K/h)";
                    break;
            }
        }
    }
}