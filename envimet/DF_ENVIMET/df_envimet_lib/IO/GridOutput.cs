using df_envimet_lib.Geometry;
using df_envimet_lib.Utility;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace df_envimet_lib.IO
{
    #region Grid Output Folder
    public class GridOutputFolderType
    {
        public const string ATMOSPHERE = "atmosphere";
        public const string POLLUTANTS = "pollutants";
        public const string RADIATION = "radiation";
        public const string SOIL = "soil";
        public const string SURFACE = "surface";
        public const string SOLAR_ACCESS = "solaraccess";
        public const string VEGETATIONS = "vegetation";
    }
    #endregion

    #region Grid Output Extension
    public enum GridOutputExtension
    {
        Standard,
        Binary
    }
    #endregion

    #region Grid Atmosphere
    public enum AtmosphereOutput
    {
        FlowU = 1,
        FlowV,
        FlowW,
        WindSpeed,
        WindSpeedChange,
        WindDirection,
        PressurePerturbation,
        AirTemperature,
        AirTemperatureDelta,
        AirTemperatureChange,
        SpecHumidity,
        RelativeHumidity,
        TKE,
        Dissipation,
        VerticalExchangeCoef,
        HorizontalExchangeCoef,
        VegetationLAD,
        DirectSwRadiation,
        DiffuseSwRadiation,
        ReflectedSwRadiation,
        TemperatureFlux,
        VapourFlux,
        WaterOnFlux,
        LeafTemperature,
        LocalMixingLength,
        MeanRadTemperature,
        TKEnormalized,
        DissipationNormalized,
        KmNormalized,
        TKEmechanicalTurbolence,
        StomataResistence,
        CO2mgm3,
        CO2ppm,
        PlantCO2flux,
        DivRlwTempChange
    }
    #endregion

    #region Grid Pollutants
    public enum PollutantsOutput
    {
        Objects = 1,
        FlowU,
        FlowV,
        FlowW,
        WindSpeed,
        WindSpeedChange,
        WindDirection,
        PotTemperature,
        SpecHumidity,
        RelativeHumidity,
        VerticalExchangeCoefImpuls,
        HorizontalExchangeCoef,
        PM25Concentration,
        PM25Source,
        PM25DepositionVelocity,
        PM25TotalDeposedMass,
        PM25ReactionTerm
    }
    #endregion

    #region Grid Radiation
    public enum RadiationOutput
    {
        QSwDirect = 1,
        QSwDirectRelative,
        QSwDiffuse,
        QSwReflectedUpperHemisphere,
        QSwReflectedLowerHemisphere,
        QLwUpperHemisphere,
        QLwLowerHemisphere,
        ViewFactorUpSky,
        ViewFactorUpBuildings,
        ViewFactorUpVegetation,
        ViewFactorUpSoil,
        ViewFactorDownSky,
        ViewFactorDownBuildings,
        ViewFactorDownVegetation,
        ViewFactorDownSoil
    }
    #endregion

    #region Grid Soil
    public enum SoilOutput
    {
        Temperature,
        VolumeticWaterContent,
        RelativeSoilWetnessRelatedToSat,
        LocalRAD,
        LocalRADOwner,
        RootWaterUptake
    }
    #endregion

    #region Grid Surface
    public enum SurfaceOutput
    {
        IndexSurfaceGrid,
        SoilProfileType,
        Ztopo,
        SurfaceInclination,
        SurfaceExposition,
        ShadowFlag,
        Tsurface,
        TsurfaceDiff,
        TsurfaceChange,
        Qsurface,
        UvAboveSurface,
        SensibleHeatFlux,
        ExchangeCoeffHeat,
        LatentHeatFlux,
        SoilHeatFlux,
        QswDirect,
        QswDirectHorizontal,
        QswDiffuseHorizontal,
        QswReflectedReceivedHorizontal,
        LambertFactor,
        QLwemitted,
        QLwbudget,
        QLwfromSky,
        QLwfromBuildings,
        QLwfromVegetation,
        QLwSumAllFluxes,
        WaterFlux,
        SkyViewFaktor,
        BuildingHeight,
        SurfaceAlbedo,
        DepositionSpeed,
        MassDeposed,
        ZnodeBiomet,
        ZBiomet,
        TAirBiomet,
        QAirBiomet,
        TMRTBiomet,
        WindSpeedBiomet,
        MassBiomet,
        Receptors
    }
    #endregion

    #region Solar Access
    public enum SolarAccessOutput
    {
        Ztopo,
        Buildings,
        BuildingHeight,
        IndexZnodeTerrain,
        IndexZnodeBiomet,
        ZbiometAbsolute,
        SkyViewFactor,
        SunHoursTerrainLevel,
        ShadowHoursTerrainLevel,
        SunHoursBiometLevel,
        ShadowHoursBiometLevel
    }
    #endregion

    public class GridOutput : Facade
    {
        private const string GRID_EXTENSION = "EDX";
        private const string GRID_BINARY_EXTENSION = "EDT";

        public static IEnumerable<string> GetAllGridFiles(string targetDirectory, GridOutputExtension extension)
        {
            List<string> fileNames = new List<string>();
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            string extensionType = (extension == GridOutputExtension.Binary) ? GRID_BINARY_EXTENSION : GRID_EXTENSION;

            foreach (string fileName in fileEntries)
            {
                if (fileName.EndsWith(extensionType))
                    fileNames.Add(fileName);
            }

            return fileNames;
        }

        public static string GetReport(string edx)
        {
            Dictionary<string, string> outputKeys = GetOutputKeys(edx);

            string report = String.Format("Location Name: {0}\nNum.X cells:{1}; Num.Y cells:{2}; Num.Z cells:{3}\n",
                outputKeys["locationname"],outputKeys["nr_xdata"], outputKeys["nr_ydata"], outputKeys["nr_zdata"]);

            return report;
        }

        public static List<double> ParseBinGrid(string edx, string edt, int variable, int location, FaceDirection dir)
        {
            List<double> results = new List<double>();

            Dictionary<string, string> outputKeys = GetOutputKeys(edx);
            var numX = Convert.ToInt32(outputKeys["nr_xdata"]);
            var numY = Convert.ToInt32(outputKeys["nr_ydata"]);
            var numZ = Convert.ToInt32(outputKeys["nr_zdata"]);

            if (dir == FaceDirection.X)
            {
                if (location > numX)
                    return null;
                results = GetNumberXDirection(edt, variable, outputKeys, location);
            }
            else if (dir == FaceDirection.Y)
            {
                if (location > numY)
                    return null;
                results = GetNumberYDirection(edt, variable, outputKeys, location);
            }
            else
            {
                if (location > numZ)
                    return null;
                results = GetNumberZDirection(edt, variable, outputKeys, location);
            }

            return results;
        }

        #region Private Method Results
        private static Dictionary<string, string> GetOutputKeys(string edx)
        {
            Read edxFile = new Read() { Metaname = edx };
            string directoryName = Path.GetDirectoryName(edx);

            string cleanFile = edxFile.WriteCleanXml(directoryName, fileType: ".xml");
            var outputKeys = Read.GetDictionaryFromEdx(cleanFile);

            try { File.Delete(cleanFile); }
            catch { }

            return outputKeys;
        }

        private static List<double> GetNumberZDirection(string edt, int variable, Dictionary<string, string> outputKeys, int location)
        {
            int numX = Convert.ToInt16(outputKeys["nr_xdata"]);
            int numY = Convert.ToInt16(outputKeys["nr_ydata"]);
            int numZ = Convert.ToInt16(outputKeys["nr_zdata"]);

            List<double> results = new List<double>();
            int bufferSize = 4 * numX * numY * numZ;
            int offset = location * 4 * numX * numY;

            using (FileStream SourceStream = File.Open(edt, FileMode.Open))
            {
                BinaryReader binReader = new BinaryReader(SourceStream);

                binReader.BaseStream.Position = bufferSize * variable + offset;
                byte[] dateArray = binReader.ReadBytes(bufferSize - offset);

                for (int i = 0; i < 4 * numX * numY; i = i + 4)
                {
                    float number = BitConverter.ToSingle(dateArray, i);
                    results.Add(Math.Round(number, 4));
                }
            }

            return results;
        }

        private static List<double> GetNumberYDirection(string edt, int variable, Dictionary<string, string> outputKeys, int location)
        {
            int numX = Convert.ToInt16(outputKeys["nr_xdata"]);
            int numY = Convert.ToInt16(outputKeys["nr_ydata"]);
            int numZ = Convert.ToInt16(outputKeys["nr_zdata"]);

            List<double> results = new List<double>();
            int bufferSize = 4 * numX * numY * numZ;

            using (FileStream SourceStream = File.Open(edt, FileMode.Open))
            {
                BinaryReader binReader = new BinaryReader(SourceStream);

                binReader.BaseStream.Position = bufferSize * variable + (location * 4 * numX);
                byte[] dateArray = binReader.ReadBytes(bufferSize);

                int count = 0;
                for (int i = 0; i < dateArray.Length; i = i + 4)
                {
                    float number = BitConverter.ToSingle(dateArray, i);
                    count++;
                    if (count - (numX) == 0)
                    {
                        i += 4 * (numX * (numY - 1));
                        count = 0;
                    }
                    results.Add(Math.Round(number, 4));
                }
            }

            return results;
        }

        private static List<double> GetNumberXDirection(string edt, int variable, Dictionary<string, string> outputKeys, int location)
        {
            int numX = Convert.ToInt16(outputKeys["nr_xdata"]);
            int numY = Convert.ToInt16(outputKeys["nr_ydata"]);
            int numZ = Convert.ToInt16(outputKeys["nr_zdata"]);

            List<double> results = new List<double>();
            int bufferSize = 4 * numX * numY * numZ;

            using (FileStream SourceStream = File.Open(edt, FileMode.Open))
            {
                BinaryReader binReader = new BinaryReader(SourceStream);

                binReader.BaseStream.Position = bufferSize * variable + (location * 4);
                byte[] dateArray = binReader.ReadBytes(bufferSize);

                for (int i = 0; i < dateArray.Length; i = i + 4)
                {
                    float number = BitConverter.ToSingle(dateArray, i);
                    results.Add(Math.Round(number, 4));
                    i += (numX - 1) * 4;
                }
            }

            return results;
        }
        #endregion

        public static Mesh GetAnalysisMesh(Grid grid, FaceDirection dir)
        {
            if (dir == FaceDirection.X)
                return ConvertToRhinoYZMesh(grid);
            else if (dir == FaceDirection.Y)
                return ConvertToRhinoXZMesh(grid);
            else
                return ConvertToRhinoXYMesh(grid);
        }

        public static Vector3d GetVectorPlaneFromEdx(string edx, FaceDirection dir, int location)
        {
            Dictionary<string, string> outputKeys = GetOutputKeys(edx);

            List<double> sequenceX = outputKeys["spacing_x"].Split(',')
                .ToList().ConvertAll(v => Convert.ToDouble(v));
            List<double> sequenceY = outputKeys["spacing_y"].Split(',')
                .ToList().ConvertAll(v => Convert.ToDouble(v));
            List<double> sequenceZ = outputKeys["spacing_z"].Split(',')
                .ToList().ConvertAll(v => Convert.ToDouble(v));

            List<double> accumulateX = Util.Accumulate(sequenceX).ToList();
            List<double> accumulateY = Util.Accumulate(sequenceY).ToList();
            List<double> positionZ = Util.Accumulate(sequenceZ).ToList().Zip(sequenceZ, (a, b) => a - (b / 2)).ToList();

            if (dir == FaceDirection.X)
                return new Vector3d(accumulateX[location], 0, 0);
            else if (dir == FaceDirection.Y)
                return new Vector3d(0, accumulateY[location], 0);
            else
                return new Vector3d(0, 0, positionZ[location]);
        }

        private static Mesh ConvertToRhinoXYMesh(Grid grid)
        {
            Mesh result = new Mesh();
            Facade face = new Facade();

            int numX = grid.NumY;
            int numY = grid.NumX;

            for (int i = 0; i < numX; i++)
                for (int j = 0; j < numY; j++)
                {
                    PixelCoordinate pix = new PixelCoordinate() { I = j, J = i, K = 0 };
                    face = GenerateZfacade(grid, pix);

                    result.Append(face.Geometry);
                }

            return result;
        }

        private static Mesh ConvertToRhinoYZMesh(Grid grid)
        {
            Mesh result = new Mesh();

            Facade face = new Facade();
            var height = grid.Height;
            int numY = grid.NumY;

            for (int k = 0; k < height.Length; k++)
                for (int j = 0; j < numY; j++)
                {
                    PixelCoordinate pix = new PixelCoordinate() { I = 0, J = j, K = k };
                    face = GenerateXfacade(grid, pix);

                    result.Append(face.Geometry);
                }

            return result;
        }

        private static Mesh ConvertToRhinoXZMesh(Grid grid)
        {
            Mesh result = new Mesh();

            Facade face = new Facade();
            var height = grid.Height;
            int numX = grid.NumX;

            for (int k = 0; k < height.Length; k++)
                for (int i = 0; i < numX; i++)
                {
                    PixelCoordinate pix = new PixelCoordinate() { I = i, J = 0, K = k };
                    face = GenerateYfacade(grid, pix);

                    result.Append(face.Geometry);
                }

            return result;
        }

    }

}
