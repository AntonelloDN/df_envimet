using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace df_envimet_lib.IO
{
    public enum ReceptorAtmosphereVariable
    {
        Date = 0,
        Time = 1,
        Z = 3,
        u = 4,
        v = 5,
        w = 6,
        WindSpeed = 7,
        WindDirection = 8,
        AirTemperature = 9,
        DiffTemperature = 10,
        SpecificHumidity = 11,
        RelativeHumidity = 12,
        VerticalExchangeCoefficient = 13,
        VerticalKmNormed = 14,
        HorizontalExchangeCoefficient = 15,
        TurbulentKineticEnergy = 16,
        DissipationTKE= 17,
        MeanRadiantTemperature = 18,
        LeafAreaDensity = 19,
        LeafFoliageTemperature = 20,
        SensibleHeatFluxFromLeaf = 21,
        LatentHeatFluxFromLeaf = 22,
        StomataResistance = 23,
        CO2 = 24,
        CO2Flux = 25,
        ShortwaveDirectRadiation = 26,
        ShortwaveDiffuseRadiation = 27,
        PressurePerturbation = 28,
        MassConcentration = 29,
        MechanicalProductionTKE = 30,
        AirTemperatureChangeLongwave = 31,
        SkyViewFactorBuilding = 32,
        SkyViewFactorBuildingLeaf = 33
    }

    public enum ReceptorSoilVariable
    {
        Date = 0,
        Time = 1,
        Z = 3,
        Temperature = 4,
        VolumeWaterContent = 5,
        TemperatureDiffusifity = 6
    }

    public enum ReceptorFluxVariable
    {
        Date = 0,
        Time = 1,
        Z = 3,
        SurfaceTemperature = 4,
        ChangeSurfaceTemperature = 5,
        SurfaceHumidity = 6,
        HorizontalWindSpeedaboveSurface = 7,
        VerticalComponentWindSpeedaboveSurfaceZ = 8,
        AirTemperatureofGridPoint = 9,
        SensibleHeatFlux = 10,
        LatentHeatFlux = 11,
        SoilHeatFlux = 12,
        MasSExchangeCoefficient = 13,
        TurbulentExchangeCoefficient = 14,
        MaxDirectShortwaverRadiation = 15,
        MaxDiffuseShortwaveRadiation = 16,
        MaxReflectedShortwaveRadiation = 17,
        LongwaveRadiationBudgetSrf = 18
    }


    public static class ReceptorFileType
    {
        public const string ATMOSPHERE = "ATM";
        public const string SOIL = "SOI";
        public const string FLUX = "FLX";
    }


    public class ReceptorOutput
    {
        public const string RECEPTOR_PATH = "receptors";
        public const string TIME_SERIES_EXTENSION = "1DT";
        public const string NORMAL_EXTENSION = "1DR";

        public static List<string> GetValueFromCsv(string fileName, int index)
        {

            List<string> receptorValues = new List<string>();

            using (var reader = new StreamReader(fileName))
            {
                int count = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (count > 1 && line.Contains("in terrain") && index > 3)
                    {
                        receptorValues.Add("0");
                    }
                    else if (count > 1)
                    {
                        line = Regex.Replace(line, @"\s+", ";");
                        var values = line.Split(';');

                        receptorValues.Add(values[index]);
                    }

                    count++;
                }
            }
            return receptorValues;
        }


        public static IEnumerable<string> GetAllReceptorFiles(string targetDirectory, string fileType)
        {
            List<string> fileNames = new List<string>();
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            string extension = null;
            switch (fileType)
            {
                case ReceptorFileType.ATMOSPHERE:
                case ReceptorFileType.SOIL:
                    extension = NORMAL_EXTENSION;
                    break;
                default:
                    extension = TIME_SERIES_EXTENSION;
                    break;
            }

            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(fileType) && fileName.EndsWith(extension))
                    fileNames.Add(fileName);
            }

            return fileNames;
        }


        public static IEnumerable<string> GetAllReceptorDirectory(string outputDirectory)
        {
            List<string> directories = new List<string>();
            string receptorRoot = String.Join("\\", outputDirectory, RECEPTOR_PATH);
            if (Directory.Exists(receptorRoot))
            {
                string[] fileEntries = Directory.GetDirectories(receptorRoot);
                foreach (string fileName in fileEntries)
                {
                    directories.Add(fileName);
                }
            }
            return directories;
        }

    }
}
