using System;
using System.Linq;


namespace envimentIntegration
{
    public class LocationFromLB
    {
        private string location;

        private const char SeparateByChar = '!';
        private const string Approximate6digit = "n6";

        public string LocationName { get; }
        public string Latitude { get; }
        public string Longitude { get; }
        public string TimeZone { get; }
        public int ModelRotation { get; }

        public LocationFromLB(string location, int modelRotation)
        {
            this.location = location;
            this.ModelRotation = modelRotation;

            // split string
            string[] locationStr = location.Split('\n');
            string newLocStr = "";

            foreach (string line in locationStr)
            {
                string cLine;
                if (line.Contains(SeparateByChar))
                {
                    cLine = line.Split(SeparateByChar)[0];
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
            this.Latitude = latitude.ToString(Approximate6digit);
            this.Longitude = longitude.ToString(Approximate6digit);
            this.TimeZone = timeZoneEnvimet;

        }
    }
}