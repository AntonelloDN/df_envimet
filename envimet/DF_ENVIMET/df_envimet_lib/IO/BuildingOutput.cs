using System;
using System.Collections.Generic;
using System.IO;

namespace df_envimet_lib.IO
{

    #region Facade Building Output
    public enum FacadeVariable
    {
        WallTemperatureNode1out = 1,
        WallTemperatureNode2 = 2,
        WallTemperatureNode3 = 3,
        WallTemperatureNode4 = 4,
        WallTemperatureNode5 = 5,
        WallTemperatureNode6 = 6,
        WallTemperatureNode7in = 7,
        WallHumidityFlux = 8,
        WallLongwave = 9,
        WallWindSpeed = 10,
        WallAirTemperature = 11,
        WallShortwaveReceived = 12,
        WallShortwaveAbsorbed = 13,
        WallIncomingLongwave = 14,
        WallRefrectedShortwave = 15,
        WallSensibleHeatCoeff = 16,
        WallLongwaveEnergyBalance = 17,
        NN = 18,
        BuildingInsideTemperature = 19,
        BuildingReflectedShortwave = 20,
        BuildingLongwaveRadiation = 21,
        GreeningLeafTemperature = 22,
        GreeningCanopyTemperature = 23,
        GreeningCanopyHumidity = 24,
        GreeningLongwaveBothSide = 25,
        GreeningWindSpeed = 26,
        GreeningAirTemperature = 27,
        GreeningShortwaveReceived = 28,
        GreeningIncomingLongwaveBothSide = 29,
        GreeningReflectedShortwave = 30,
        GreeningTranspirationFlux = 31,
        GreeningStomataResistance = 32,
        GreeningWaterAccessFactor = 33,
        SubstrateTemperatureNode1out = 34,
        SubstrateTemperatureNode2 = 35,
        SubstrateTemperatureNode3 = 36,
        SubstrateTemperatureNode4 = 37,
        SubstrateTemperatureNode5 = 38,
        SubstrateTemperatureNode6 = 39,
        SubstrateTemperatureNode7in = 40,
        SubstrateSurfaceHumidity = 41,
        SubstrateHumidityFlux = 42,
        SubstrateLongwaveEmitted = 43,
        SubstrateWindSpeed = 44,
        SubstrateAirTemperature = 45,
        SubstrateShortwaveReceived = 46,
        SubstrateShortwaveAbsorbed = 47,
        SubstrateIncomingLongwave = 48,
        SubstrateShortwaveReflected = 49
    }
    #endregion

    #region Avg Building Output
    public enum BuldingDatVariable
    {
        Simtime = 0,
        Date = 1,
        Time = 2,
        IndoorTemp = 3,
        Av_Tair_at_facade = 4,
        EnergyfluxToIndoorSum = 5,
        EnergyfluxToIndoorAverage = 6,
        EnergyfluxConductionAverage = 7,
        EnergyfluxSWTransmissionAverage = 8,
        ShortwaveAvailabe = 9,
        ShortwaveAbsorbed = 10,
        LongwaveAvailable = 11,
        LongwaveEmittedFacade = 12,
        Sensibleoutside = 13,
        VapourFromGreening = 14,
        VapourFromSubstrate = 15
    }
    #endregion

    public static class BuildingFileType
    {
        public const string DYN_FOLDER = "dynamic";

        public const string AVG_FILE = "AvgById";
        public const string FACADE_FILE = "Facade";
        public const string FACADE_BIN_FILE = "FacadeBinary";

    }

    public class BuildingOutput
    {
        public const string BUILDING_PATH = "buildings";
        public const string BUILDING_AVG_EXTENSION = "DAT";
        public const string BUILDING_FACADE_EXTENSION = "EDX";
        public const string BUILDING_FACADE_BINARY_EXTENSION = "EDT";

        public static IEnumerable<string> GetAllBuildingDynFiles(string targetDirectory, string fileType)
        {
            List<string> fileNames = new List<string>();
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            string extension = null;
            switch (fileType)
            {
                case BuildingFileType.AVG_FILE:
                    extension = BUILDING_AVG_EXTENSION;
                    break;
                case BuildingFileType.FACADE_FILE:
                    extension = BUILDING_FACADE_EXTENSION;
                    break;
                case BuildingFileType.FACADE_BIN_FILE:
                    extension = BUILDING_FACADE_BINARY_EXTENSION;
                    break;
            }

            foreach (string fileName in fileEntries)
            {
                if (fileName.EndsWith(extension))
                    fileNames.Add(fileName);
            }

            return fileNames;
        }

        public static IEnumerable<string> GetAllBuildingDirectory(string outputDirectory)
        {
            List<string> directories = new List<string>();
            string receptorRoot = String.Join("\\", outputDirectory, BUILDING_PATH);
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

        public static List<double> ParseBinBuilding(string edx, string edt, int variable, FaceDirection dir)
        {
            Read edxFile = new Read() { Metaname = edx };
            string directoryName = Path.GetDirectoryName(edx);

            string cleanFile = edxFile.WriteCleanXml(directoryName, fileType:".xml");
            var outputKeys = Read.GetDictionaryFromEdx(cleanFile);

            try { File.Delete(cleanFile); }
            catch { }

            List<double> results = GetNumberFromBinaryByDirection(edt, variable, dir, outputKeys);

            return results;
        }

        private static List<double> GetNumberFromBinaryByDirection(string edt, int variable, FaceDirection dir, Dictionary<string, string> outputKeys)
        {
            int numX = Convert.ToInt16(outputKeys["nr_xdata"]);
            int numY = Convert.ToInt16(outputKeys["nr_ydata"]);
            int numZ = Convert.ToInt16(outputKeys["nr_zdata"]);

            List<double> results = new List<double>();
            int offsetStart = 4 * numX * numY * numZ;
            int bufferSize = 4 * numX * numY * numZ * 3;

            using (FileStream SourceStream = File.Open(edt, FileMode.Open))
            {
                BinaryReader binReader = new BinaryReader(SourceStream);

                int offset = 0;
                if (dir == FaceDirection.Y)
                    offset = 4;
                else if (dir == FaceDirection.Z)
                    offset = 8;

                binReader.BaseStream.Position = bufferSize * variable + offset + offsetStart;
                byte[] dateArray = binReader.ReadBytes(bufferSize);

                /*
                 *    |----|----|----|
                 */
                for (int i = 0; i < dateArray.Length; i = i + 4)
                {
                    float number = BitConverter.ToSingle(dateArray, i);
                    if (variable != 1)
                    {
                        if (number != -999.0)
                            results.Add(Math.Round(number, 4));
                    }
                    else
                    {
                        if (number != -999.0 && number != 1)
                            results.Add(Math.Round(number, 4));
                    }

                    i += 8;
                }
            }

            return results;
        }
    }
}
