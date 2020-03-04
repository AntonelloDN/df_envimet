using System;
using System.Collections.Generic;
using System.IO;

namespace df_envimet_lib.IO
{

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


    public static class BuildingFileType
    {
        public const string AVG_FILE = "AvgById";
        public const string DYN_FOLDER = "dynamic";
    }

    public class BuildingOutput
    {
        public const string BUILDING_PATH = "buildings";
        public const string BUILDING_AVG_EXTENSION = "DAT";

        public static IEnumerable<string> GetAllBuildingDatFiles(string targetDirectory, string fileType)
        {
            List<string> fileNames = new List<string>();
            string[] fileEntries = Directory.GetFiles(targetDirectory);

            string extension = null;
            switch (fileType)
            {
                case BuildingFileType.AVG_FILE:
                    extension = BUILDING_AVG_EXTENSION;
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
    }
}
