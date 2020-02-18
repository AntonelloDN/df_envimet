using System.Text;
using System.Text.RegularExpressions;

namespace df_envimet_lib.IO
{
    public class Read
    {
        public string Metaname { get; set; }

        private string ReadEnvimetNoBinaryFile()
        {
            string characters = @"[^\s()_<>/,\.A-Za-z0-9=""]+";
            Encoding isoLatin1 = Encoding.GetEncoding(28591); ;
            string text = System.IO.File.ReadAllText(Metaname, isoLatin1);

            Regex.Replace(characters, "", text);

            return text.Replace("&", "").Replace("<Remark for this Source Type>", "");
        }

        public string WriteReadebleEDXFile(string path, string variableName = "ENVI", string fileType = ".EDX")
        {
            string fileNameWithExtension = variableName + fileType;

            string newFile = System.IO.Path.Combine(path, fileNameWithExtension);

            // make a readible version of the xml file
            string metainfo = ReadEnvimetNoBinaryFile();

            // write file in a new destination
            System.IO.File.WriteAllText(newFile, metainfo);

            return newFile;
        }
    }
}
