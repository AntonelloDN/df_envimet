using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace df_envimet_lib.IO
{
    class FoxBatch
    {
        public static string GetFoxFile(string epw, string projectFolder, string envimetCustomFolder=null)
        {
            string envimet;
            string root = System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            if (envimetCustomFolder == null)
                envimet = System.IO.Path.Combine(root, Workspace.DEFAULT_FOLDER + "\\win64");
            else
                envimet = System.IO.Path.Combine(envimetCustomFolder, "win64");

            string foxName = System.IO.Path.GetFileNameWithoutExtension(epw) + ".fox";
            string target = System.IO.Path.Combine(projectFolder, foxName);

            string path = System.IO.Path.Combine(projectFolder, foxName + "FOX.bat");

            string batch = "@echo I'm writing FOX file...\n" +
            "@echo off\n" +
            "\"{0}\\foxmanager.exe\" \"{1}\" \"{2}\"\n" +
            "echo If Envimet is not in default unit 'C:\' connect installation folder.\n" +
            "pause\n";

            string[] contentOfBatch = { String.Format(batch, envimet, epw, target) };

            System.IO.File.WriteAllLines(path, contentOfBatch);

            if (!System.IO.File.Exists(target))
                RunBat(path);

            return foxName;
        }

        private static void RunBat(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.Start();
        }
    }
}
