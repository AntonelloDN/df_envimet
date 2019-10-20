using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorphoEnvimetLibrary.Settings
{
    public class Location
    {
        private string _location;

        private const char SEPARATE_BY_CHAR = '!';
        private const string SIX_DIGIT_FORMATTING = "n6";

        public string LocationName { get; }
        public string Latitude { get; }
        public string Longitude { get; }
        public string TimeZone { get; }
        public int ModelRotation { get; }

        public Location(string location, int modelRotation)
        {
            this._location = location;
            this.ModelRotation = modelRotation;

            // split string
            string[] locationStr = location.Split('\n');
            string newLocStr = "";

            foreach (string line in locationStr)
            {
                string cLine;
                if (line.Contains(SEPARATE_BY_CHAR))
                {
                    cLine = line.Split(SEPARATE_BY_CHAR)[0];
                    newLocStr = newLocStr + cLine.Replace(" ", "");
                }
                else
                {
                    newLocStr = newLocStr + line;
                    newLocStr = newLocStr.Replace(";", "");
                }
            }

            string[] locationInfo = newLocStr.Split(',');

            string locationName = locationInfo[1];
            double latitude = Convert.ToDouble(locationInfo[2]);
            double longitude = Convert.ToDouble(locationInfo[3]);
            string timeZone = locationInfo[4];

            string timeZoneEnvimet;

            // timezone
            double num = Convert.ToDouble(timeZone);
            if (num > 0)
            {
                timeZoneEnvimet = "UTC+" + Convert.ToString((int)num);
            }
            else if (num < 0)
            {
                timeZoneEnvimet = "UTC-" + Convert.ToString((int)num);
            }
            else
            {
                timeZoneEnvimet = "GMT";
            }

            // 6 digits
            this.LocationName = locationName;
            this.Latitude = latitude.ToString(SIX_DIGIT_FORMATTING);
            this.Longitude = longitude.ToString(SIX_DIGIT_FORMATTING);
            this.TimeZone = timeZoneEnvimet;

        }
    }
}
