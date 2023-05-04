using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using df_envimet_lib.Geometry;
using System;

namespace df_envimet_lib.IO
{

    public class Read
    {
        public string Metaname { get; set; }

        private string ReadEdxFile()
        {
            string characters = @"[^\s()_<>/,\.A-Za-z0-9=""\P{IsBasicLatin}\p{IsLatin-1Supplement}]+";

            //var isoLatin1 = Encoding.GetEncoding(28591);
            if (!System.IO.File.Exists(Metaname))
                throw new Exception($"{Metaname} not found.");
            string text = System.IO.File.ReadAllText(Metaname);
            string res = Regex.Replace(text, characters, "");

            return res.Replace("<Remark for this Source Type>", "");
        }

        public string WriteCleanXml(string path, string variableName = "ENVI", string fileType = ".EDX")
        {
            string fileNameWithExtension = variableName + fileType;

            string newFile = System.IO.Path.Combine(path, fileNameWithExtension);

            // make a readible version of the xml file
            string metainfo = ReadEdxFile();

            // write file in a new destination
            System.IO.File.WriteAllText(newFile, metainfo);

            return newFile;
        }

        private static string GetValueFromXml(XDocument xml, string keyword)
        {
            var innerText = xml.Descendants(keyword)
              .Select(v => v.Value)
              .ToList()[0].ToString();

            return innerText;
        }

        public static Dictionary<string, string> GetDictionaryFromEdx(string file)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            XDocument xml = XDocument.Load(file); //clean xml

            string projectName = GetValueFromXml(xml, "projectname");
            string locationName = GetValueFromXml(xml, "locationname");
            string nameVariables = GetValueFromXml(xml, "name_variables");


            string spacingX = GetValueFromXml(xml, "spacing_x").Replace(" ", "");
            string spacingY = GetValueFromXml(xml, "spacing_y").Replace(" ", "");
            string spacingZ = GetValueFromXml(xml, "spacing_z").Replace(" ", "");

            string nrXdata = GetValueFromXml(xml, "nr_xdata").Replace(" ", "");
            string nrYdata = GetValueFromXml(xml, "nr_ydata").Replace(" ", "");
            string nrZdata = GetValueFromXml(xml, "nr_zdata").Replace(" ", "");

            values.Add("projectname", projectName);
            values.Add("locationname", locationName);
            values.Add("spacing_x", spacingX);
            values.Add("spacing_y", spacingY);
            values.Add("spacing_z", spacingZ);
            values.Add("nr_xdata", nrXdata);
            values.Add("nr_ydata", nrYdata);
            values.Add("nr_zdata", nrZdata);

            values.Add("name_variables", nameVariables);

            return values;
        }

        public static Grid GetGridFromInx(Dictionary<string, double> modelgeometry)
        {
            Grid grid = new Grid()
            {
                DimX = modelgeometry["dx"],
                DimY = modelgeometry["dy"],
                DimZ = modelgeometry["dz-base"],
                NumX = (int)modelgeometry["grids-I"],
                NumY = (int)modelgeometry["grids-J"],
                NumZ = (int)modelgeometry["grids-Z"],
                Telescope = modelgeometry["verticalStretch"],
                StartTelescopeHeight = modelgeometry["startStretch"],
                CombineGridType = (modelgeometry["useTelescoping_grid"] > 0 && modelgeometry["useSplitting"] > 0) ? true : false
            };

            if (modelgeometry["useTelescoping_grid"] > 0)
                grid.Telescope = modelgeometry["verticalStretch"];
            if (modelgeometry["startStretch"] != 0)
                grid.StartTelescopeHeight = modelgeometry["startStretch"];

            grid.CalculateHeight();

            return grid;
        }

    }
}
