using System;
using System.Collections.Generic;


namespace envimetSimulationFile
{
    public class MainSettings
    {
        public string SimName { get; set; }
        public string INXfileAddress { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public int SimDuration { get; set; }
        public double WindSpeed { get; set; }
        public double WindDir { get; set; }
        public double Roughness { get; set; }
        public double InitialTemperature { get; set; }
        public double SpecificHumidity { get; set; }
        public double RelativeHumidity { get; set; }
    }


    public class SampleForcingSettings
    {
        private string temperature;
        private string relativeHumidity;
        private int totNumbers;

        public string Temperature { get { return temperature; } }
        public string RelativeHumidity { get { return relativeHumidity; } }
        public int TotNumbers { get { return totNumbers; } }

        public SampleForcingSettings(List<double> temperature, List<double> relativeHumidity)
        {
            this.temperature = String.Join(",", temperature);
            this.relativeHumidity = String.Join(",", relativeHumidity);
            this.totNumbers = temperature.Count;
        }
    }


    public class TimeStepsSettings
    {
        public double Sunheight_step01 { get; set; }
        public double Sunheight_step02 { get; set; }
        public double Dt_step00 { get; set; }
        public double Dt_step01 { get; set; }
        public double Dt_step02 { get; set; }

    }


    public class Building
    {
        public double IndoorTemp { get; set; }
        public double IndoorConst { get; set; }

    }


    public class BoundaryCondition
    {
        public double LBC_TQ { get; set; }
        public double LBC_TKE { get; set; }

    }


    public class SoilTemp
    {
        public double TempUpperlayer { get; set; }
        public double TempMiddlelayer { get; set; }
        public double TempDeeplayer { get; set; }
        public double TempBedrockLayer { get; set; }
        public double WaterUpperlayer { get; set; }
        public double WaterMiddlelayer { get; set; }
        public double WaterDeeplayer { get; set; }
        public double WaterBedrockLayer { get; set; }

    }

}
