using System;
using Grasshopper.Kernel;
using df_envimet_lib.IO;
using System.Collections.Generic;
using df_envimet.Grasshopper.UI_GH;
using GHD = Grasshopper.Kernel.Data;
using Grasshopper;

namespace df_envimet.Grasshopper.IO
{
    public class ReadReceptorResults : ExtensionReceptorComponent
    {
        private const string SOIL_NULL = "---";
        private const string ZERO = "0";

        /// <summary>
        /// Initializes a new instance of the ReadReceptorResults class.
        /// </summary>
        public ReadReceptorResults()
          : base("DF Envimet Read Receptor Results", "DFReadReceptorResults",
              "Use this component to read receptor results. Right click on icon of component to set output type:\nAtmosphere;\nSoil;\nFlux.",
              "Dragonfly", "3 | Envimet")
        {
            this.Message = "VER 0.0.03\nFEB_02_2020";
        }

        public override GH_Exposure Exposure => GH_Exposure.quinary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_receptorPath", "_receptorPath", "Output folder of each simulated receptor. Connect 'DF Envimet Receptor Folder' output here.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("_receptorHeightIndex_", "_receptorHeightIndex_", "Use this index to a get value at different height. Value depends on your model.", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("_atmosphereVariable_", "_atmosphereVariable_", "Connect a number:\n0 = WindSpeed m/s\n" +
                "1 = WindDirection °\n" +
                "2 = AirTemperature °C\n" +
                "3 = DiffTemperature °C/h\n" +
                "4 = SpecificHumidity g/kg\n" +
                "5 = RelativeHumidity %\n" +
                "6 = VerticalExchangeCoefficient m²/s\n" +
                "7 = VerticalKmNormed -\n" +
                "8 = HorizontalExchangeCoefficient m²/s\n" +
                "9 = TurbulentKineticEnergy m²/s²\n" +
                "10 = DissipationTKE m³/s²\n" +
                "11 = MeanRadiantTemperature °C\n" +
                "12 = LeafAreaDensity m²/m³\n" +
                "13 = LeafFoliageTemperature °C\n" +
                "14 = SensibleHeatFluxFromLeaf W/m²2\n" +
                "15 = LatentHeatFluxFromLeaf W/m²2\n" +
                "16 = StomataResistance m/s\n" +
                "17 = CO2 mg/m³\n" +
                "18 = CO2Flux mg/(kg*s)\n" +
                "19 = ShortwaveDirectRadiation W/m²\n" +
                "20 = ShortwaveDiffuseRadiation W/m²\n" +
                "21 = PressurePerturbation Pa\n" +
                "22 = MassConcentration mg/m³\n" +
                "23 = MechanicalProductionTKE -\n" +
                "24 = AirTemperatureChangeLongwave K/h\n" +
                "25 = SkyViewFactorBuilding\n -" +
                "26 = SkyViewFactorBuildingLeaf -", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("runIt_", "runIt_", "Set runIt to True to read output.", GH_ParamAccess.item, false);
            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("receptorHeight", "receptorHeight", "Height of measure (m).", GH_ParamAccess.tree);
            pManager.AddGenericParameter("date", "date", "Ouput dates", GH_ParamAccess.tree);
            pManager.AddGenericParameter("time", "time", "Ouput times", GH_ParamAccess.tree);
            pManager.AddGenericParameter("values", "values", "Ouput values", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> _receptorPath = new List<string>();
            int _receptorHeightIndex_ = 0;
            int _val_ = 0;
            bool runIt_ = false;

            DA.GetDataList(0, _receptorPath);
            DA.GetData(1, ref _receptorHeightIndex_);
            DA.GetData(2, ref _val_);
            DA.GetData(3, ref runIt_);

            // Unwrap variables
            var variables = ReceptorOutputTypeMapping(_value);

            if (_value == ReceptorFileType.SOIL)
            {
                Params.Input[1].Name = "_receptorHeightIndex_";
                Params.Input[1].NickName = "_receptorHeightIndex_";
                Params.Input[1].Description = "Use this index to a get value at different height. Value depends on your model.";
                Params.Input[2].Name = "_soilVariable_";
                Params.Input[2].NickName = "_soilVariable_";
                Params.Input[2].Description = "Connect a number:\n0 = Temperature °C\n1= VolumeWaterContent m3/m-3\n2 = TemperatureDiffusifity *10e6";

                Params.Output[0].Description = "Negative height (cm).";
            }
            else if (_value == ReceptorFileType.FLUX)
            {
                Params.Input[1].Name = "-";
                Params.Input[1].NickName = "-";
                Params.Input[1].Description = "-";
                _receptorHeightIndex_ = 0;

                Params.Output[0].Description = "Height of measure (m).";

                Params.Input[2].Name = "_fluxVariable_";
                Params.Input[2].NickName = "_fluxVariable_";
                Params.Input[2].Description = "Connect a number:\n0 = SurfaceTemperature °C\n" +
                                            "1 = ChangeSurfaceTemperature °C/h\n" +
                                            "2 = SurfaceHumidity g/kg\n" +
                                            "3 = HorizontalWindSpeedaboveSurface m/s\n" +
                                            "4 = VerticalComponentWindSpeedaboveSurfaceZ m/s\n" +
                                            "5 = AirTemperatureofGridPoint °C\n" +
                                            "6 = SensibleHeatFlux W/m²\n" +
                                            "7 = LatentHeatFlux W/m²\n" +
                                            "8 = SoilHeatFlux W/m²\n" +
                                            "9 = MasSExchangeCoefficient m²/s\n" +
                                            "10 = TurbulentExchangeCoefficient m²/s\n" +
                                            "11 = MaxDirectShortwaverRadiation W/m²\n" +
                                            "12 = MaxDiffuseShortwaveRadiation W/m²\n" +
                                            "13 = MaxReflectedShortwaveRadiation W/m²\n" +
                                            "14 = LongwaveRadiationBudgetSrf W/m²";
            }
            else
            {
                Params.Input[1].Name = "_receptorHeightIndex_";
                Params.Input[1].NickName = "_receptorHeightIndex_";
                Params.Input[1].Description = "Use this index to a get value at different height. Value depends on your model..";
                Params.Input[2].Name = "_atmosphereVariable_";
                Params.Input[2].NickName = "_atmosphereVariable_";

                Params.Output[0].Description = "Height of measure (m).";

                Params.Input[2].Description = "Connect a number:\n0 = WindSpeed m/s\n" +
                                        "1 = WindDirection °\n" +
                                        "2 = AirTemperature °C\n" +
                                        "3 = DiffTemperature °C/h\n" +
                                        "4 = SpecificHumidity g/kg\n" +
                                        "5 = RelativeHumidity %\n" +
                                        "6 = VerticalExchangeCoefficient m²/s\n" +
                                        "7 = VerticalKmNormed -\n" +
                                        "8 = HorizontalExchangeCoefficient m²/s\n" +
                                        "9 = TurbulentKineticEnergy m²/s²\n" +
                                        "10 = DissipationTKE m³/s²\n" +
                                        "11 = MeanRadiantTemperature °C\n" +
                                        "12 = LeafAreaDensity m²/m³\n" +
                                        "13 = LeafFoliageTemperature °C\n" +
                                        "14 = SensibleHeatFluxFromLeaf W/m²2\n" +
                                        "15 = LatentHeatFluxFromLeaf W/m²2\n" +
                                        "16 = StomataResistance m/s\n" +
                                        "17 = CO2 mg/m³\n" +
                                        "18 = CO2Flux mg/(kg*s)\n" +
                                        "19 = ShortwaveDirectRadiation W/m²\n" +
                                        "20 = ShortwaveDiffuseRadiation W/m²\n" +
                                        "21 = PressurePerturbation Pa\n" +
                                        "22 = MassConcentration mg/m³\n" +
                                        "23 = MechanicalProductionTKE -\n" +
                                        "24 = AirTemperatureChangeLongwave K/h\n" +
                                        "25 = SkyViewFactorBuilding\n -" +
                                        "26 = SkyViewFactorBuildingLeaf -";
            }

            if (runIt_)
            {
                DataTree<string> valueTree = new DataTree<string>();
                DataTree<string> dateTree = new DataTree<string>();
                DataTree<string> timeTree = new DataTree<string>();

                // Warning!
                if (_val_ >= variables.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Variable is out of range, check description of the input.");
                    return;
                }

                double receptorHeight = 0;

                for (int i = 0; i < _receptorPath.Count; i++)
                {
                    IEnumerable<string> files = ReceptorOutput.GetAllReceptorFiles(_receptorPath[i], _value);
                    List<string> selectedValues = new List<string>();

                    GHD.GH_Path pth = new GHD.GH_Path(i);

                    if (_value != ReceptorFileType.FLUX)
                    {
                        foreach (string f in files)
                        {
                            // Warning
                            if (_receptorHeightIndex_ >= ReceptorOutput.GetValueFromCsv(f, (int)ReceptorAtmosphereVariable.Z).Count)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Z Index does not exist.");
                                return;
                            }

                            string val = ReceptorOutput.GetValueFromCsv(f, variables[_val_])[_receptorHeightIndex_];

                            if (val == SOIL_NULL)
                                val = ZERO;
                            selectedValues.Add(val);
                            dateTree.Add(ReceptorOutput.GetValueFromCsv(f, (int)ReceptorAtmosphereVariable.Date)[_receptorHeightIndex_], pth);
                            timeTree.Add(ReceptorOutput.GetValueFromCsv(f, (int)ReceptorAtmosphereVariable.Time)[_receptorHeightIndex_], pth);

                            receptorHeight = Convert.ToDouble(ReceptorOutput.GetValueFromCsv(f, (int)ReceptorAtmosphereVariable.Z)[_receptorHeightIndex_]);
                        }

                        valueTree.AddRange(selectedValues, pth);
                    }
                    else
                    {
                        foreach (string f in files)
                        {
                            valueTree.AddRange(ReceptorOutput.GetValueFromCsv(f, variables[_val_]), pth);
                            dateTree.AddRange(ReceptorOutput.GetValueFromCsv(f, (int)ReceptorAtmosphereVariable.Date), pth);
                            timeTree.AddRange(ReceptorOutput.GetValueFromCsv(f, (int)ReceptorAtmosphereVariable.Time), pth);
                        }
                    }

                }

                DA.SetData(0, receptorHeight);
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
                return Properties.Resources.envimetReadReceptorResultsIcon;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e664abc0-7575-4e75-8921-08fc2c2f1405"); }
        }


        private static List<int> ReceptorOutputTypeMapping(string folderType)
        {
            List<int> availableValues = new List<int>();

            switch (folderType)
            {
                case ReceptorFileType.SOIL:
                    availableValues.AddRange(new List<int>
                    {
                        (int)ReceptorSoilVariable.Temperature,
                        (int)ReceptorSoilVariable.VolumeWaterContent,
                        (int)ReceptorSoilVariable.TemperatureDiffusifity
                    });
                    break;
                case ReceptorFileType.FLUX:
                    availableValues.AddRange(new List<int>
                    {
                        (int)ReceptorFluxVariable.SurfaceTemperature,
                        (int)ReceptorFluxVariable.ChangeSurfaceTemperature,
                        (int)ReceptorFluxVariable.SurfaceHumidity,
                        (int)ReceptorFluxVariable.HorizontalWindSpeedaboveSurface,
                        (int)ReceptorFluxVariable.VerticalComponentWindSpeedaboveSurfaceZ,
                        (int)ReceptorFluxVariable.AirTemperatureofGridPoint,
                        (int)ReceptorFluxVariable.SensibleHeatFlux,
                        (int)ReceptorFluxVariable.LatentHeatFlux,
                        (int)ReceptorFluxVariable.SoilHeatFlux,
                        (int)ReceptorFluxVariable.MasSExchangeCoefficient,
                        (int)ReceptorFluxVariable.TurbulentExchangeCoefficient,
                        (int)ReceptorFluxVariable.MaxDirectShortwaverRadiation,
                        (int)ReceptorFluxVariable.MaxDiffuseShortwaveRadiation,
                        (int)ReceptorFluxVariable.MaxReflectedShortwaveRadiation,
                        (int)ReceptorFluxVariable.LongwaveRadiationBudgetSrf
                    });
                    break;
                default:
                    availableValues.AddRange(new List<int>
                    {
                        (int)ReceptorAtmosphereVariable.WindSpeed,
                        (int)ReceptorAtmosphereVariable.WindDirection,
                        (int)ReceptorAtmosphereVariable.AirTemperature,
                        (int)ReceptorAtmosphereVariable.DiffTemperature,
                        (int)ReceptorAtmosphereVariable.SpecificHumidity,
                        (int)ReceptorAtmosphereVariable.RelativeHumidity,
                        (int)ReceptorAtmosphereVariable.VerticalExchangeCoefficient,
                        (int)ReceptorAtmosphereVariable.VerticalKmNormed,
                        (int)ReceptorAtmosphereVariable.HorizontalExchangeCoefficient,
                        (int)ReceptorAtmosphereVariable.TurbulentKineticEnergy,
                        (int)ReceptorAtmosphereVariable.DissipationTKE,
                        (int)ReceptorAtmosphereVariable.MeanRadiantTemperature,
                        (int)ReceptorAtmosphereVariable.LeafAreaDensity,
                        (int)ReceptorAtmosphereVariable.LeafFoliageTemperature,
                        (int)ReceptorAtmosphereVariable.SensibleHeatFluxFromLeaf,
                        (int)ReceptorAtmosphereVariable.LatentHeatFluxFromLeaf,
                        (int)ReceptorAtmosphereVariable.StomataResistance,
                        (int)ReceptorAtmosphereVariable.CO2,
                        (int)ReceptorAtmosphereVariable.CO2Flux,
                        (int)ReceptorAtmosphereVariable.ShortwaveDirectRadiation,
                        (int)ReceptorAtmosphereVariable.ShortwaveDiffuseRadiation,
                        (int)ReceptorAtmosphereVariable.PressurePerturbation,
                        (int)ReceptorAtmosphereVariable.MassConcentration,
                        (int)ReceptorAtmosphereVariable.MechanicalProductionTKE,
                        (int)ReceptorAtmosphereVariable.AirTemperatureChangeLongwave,
                        (int)ReceptorAtmosphereVariable.SkyViewFactorBuilding,
                        (int)ReceptorAtmosphereVariable.SkyViewFactorBuildingLeaf
                    });
                    break;
            }

            return availableValues;
        }
    }
}