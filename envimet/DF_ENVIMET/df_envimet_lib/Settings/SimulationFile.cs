using System;
using System.Collections.Generic;


namespace df_envimet_lib.Settings
{
    public class MainSettings
    {
        private int simulationDuration;
        private double specificHumidity;
        private double relativeHumidity;
        public string SimName { get; set; }
        public string INXfileAddress { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public int SimDuration
        {
            get {return simulationDuration;}
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("You cannot insert negative numbers");
                }
                simulationDuration = value;
            }
        }
        public double WindSpeed { get; set; }
        public double WindDir { get; set; }
        public double Roughness { get; set; }
        public double InitialTemperature { get; set; }
        public double SpecificHumidity
        {
            get
            { return specificHumidity; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("You cannot insert negative numbers");
                }
                specificHumidity = value;
            }
        }
        public double RelativeHumidity
        {
            get
            { return relativeHumidity; }
            set
            {

                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Relative humidity go from 0 to 100.");
                }
                relativeHumidity = value;
            }
        }
    }

    public class SampleForcingSettings
    {
        private string _temperature;
        private string _relativeHumidity;
        private int _totNumbers;

        public string Temperature { get { return _temperature; } }
        public string RelativeHumidity { get { return _relativeHumidity; } }
        public int TotNumbers { get { return _totNumbers; } }

        public SampleForcingSettings(List<double> temperature, List<double> relativeHumidity)
        {
            this._temperature = String.Join(",", temperature);
            this._relativeHumidity = String.Join(",", relativeHumidity);
            this._totNumbers = temperature.Count;
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

    public class BuildingSetting
    {
        public double IndoorTemp { get; set; }
        public double IndoorConst { get; set; }
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

    public class OutputTiming
    {
        private int mainFileTiming;
        private int textFileTiming;

        public int MainFiles
        {
            get {return mainFileTiming; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Value has to be greater than 0");
                }
                mainFileTiming = value;
            }
        }
        public int TextFiles
        {
            get { return textFileTiming; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("You cannot insert negative numbers");
                }
                textFileTiming = value;
            }
        }
        public int InclNestingGrids { get; set; }
    }
}
