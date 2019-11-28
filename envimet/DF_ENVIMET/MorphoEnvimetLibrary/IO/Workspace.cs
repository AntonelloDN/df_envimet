using System;

namespace MorphoEnvimetLibrary.IO
{
    public class Workspace
    {
        public const string DEFAULT_FOLDER = "ENVImet444";

        private string _workspaceFolder;
        private string _projectFolderName;
        private string _fileNamePrj;
        private string _iniFileName;
        private string _worspaceName;
        private string _projectName;
        private string _edbFileName;

        public static string ENVImetInstallFolder { get; set; }

        public Workspace(string workspaceFolder, string projectFolderName)
        {
            this._workspaceFolder = workspaceFolder;
            this._projectFolderName = projectFolderName;
            this._fileNamePrj = System.Environment.MachineName + ".projects";
            this._iniFileName = "envimet.ini";
            this._worspaceName = "workspace.infoX";
            this._projectName = "project.infoX";
            this._edbFileName = "projectdatabase.edb";
        }

        public string WorkspaceFolderLBwrite(string mainDirectory)
        {
            // date
            var now = DateTime.Now;

            // complete path
            string fullFolder = System.IO.Path.Combine(this._workspaceFolder, this._projectFolderName);

            // check folder
            if (!(System.IO.Directory.Exists(fullFolder)))
            {
                System.IO.Directory.CreateDirectory(fullFolder);
            }

            // create project
            string prjFile = System.IO.Path.Combine(mainDirectory, this._fileNamePrj);
            System.IO.File.WriteAllText(prjFile, fullFolder);


            // INI and workspace file
            string iniFile = System.IO.Path.Combine(mainDirectory, this._iniFileName);
            string workspaceXml = System.IO.Path.Combine(mainDirectory, this._worspaceName);
            string projectFileInFolder = System.IO.Path.Combine(fullFolder, this._projectName);
            string edbFileInFolder = System.IO.Path.Combine(fullFolder, this._edbFileName);

            // iniFile
            string[] textIniFile = { "[projectdir]", String.Format("dir={0}", this._workspaceFolder) };
            System.IO.File.WriteAllLines(iniFile, textIniFile);

            // workspaceXml
            string[] textWorkspaceXml = {"<ENVI-MET_Datafile>", "<Header>", "<filetype>workspacepointer</filetype>",
        "<version>6811715</version>", String.Format("<revisiondate>{0}</revisiondate>", now.ToString("yyyy-MM-dd HH:mm:ss")),
        "<remark></remark>", "<encryptionlevel>5150044</encryptionlevel>", "</Header>",
        "<current_workspace>", String.Format("<absolute_path> {0} </absolute_path>", this._workspaceFolder),
        String.Format("<last_active> {0} </last_active>", this._projectFolderName), "</current_workspace>", "</ENVI-MET_Datafile>"};
            System.IO.File.WriteAllLines(workspaceXml, textWorkspaceXml);

            // projectFileInFolder
            string[] textProjectFileInFolder = {"<ENVI-MET_Datafile>", "<Header>", "<filetype>infoX ENVI-met Project Description File</filetype>",
        "<version>4240697</version>", String.Format("<revisiondate>{0}</revisiondate>", now.ToString("yyyy-MM-dd HH:mm:ss")),
        "<remark></remark>", "<encryptionlevel>5220697</encryptionlevel>", "</Header>",
        "<project_description>", String.Format("<name> {0} </name>", this._projectFolderName),
        "<description>  </description>", "<useProjectDB> 1 </useProjectDB>", "</project_description>", "</ENVI-MET_Datafile>"};
            System.IO.File.WriteAllLines(projectFileInFolder, textProjectFileInFolder);

            // edbFileInFolder
            if (!System.IO.File.Exists(edbFileInFolder))
            {
                string[] textEdbFileInFolder = {"<ENVI-MET_Datafile>", "<Header>", "<filetype>DATA</filetype>",
        "<version>1</version>", String.Format("<revisiondate>{0}</revisiondate>", now.ToString("yyyy-MM-dd HH:mm:ss")),
        "<remark>Envi-Data</remark>", "<encryptionlevel>1701377</encryptionlevel>",
        "</Header>", "</ENVI-MET_Datafile>"};
                System.IO.File.WriteAllLines(edbFileInFolder, textEdbFileInFolder);
            }
            return fullFolder;
        }

        public static string FindENVI_MET()
        {

            string root = System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string directory = System.IO.Path.Combine(root, DEFAULT_FOLDER + "\\sys.basedata\\");

            // custom forlder
            if (Workspace.ENVImetInstallFolder != null)
            {
                directory = System.IO.Path.Combine(ENVImetInstallFolder, "sys.basedata\\");
            }

            if (System.IO.Directory.Exists(directory))
            {
                return directory;
            }
            else
            {
                return null;
            }
        }

        public static string CreateFolderWithBase(string mainFolder, string sysBase)
        {
            string envimetMainFolder = mainFolder.Replace("sys.basedata", "");

            return System.IO.Path.Combine(envimetMainFolder, sysBase);
        }

        public static void WriteBatchFile(string mainFolderWithBase, string projectPath, string simulationPath)
        {
            // gen elements
            string simulationFileName = System.IO.Path.GetFileName(simulationPath);
            string projectFolderName = System.IO.Path.GetFileName(projectPath);

            // batch abs path
            string fullPathBatch = GetBatchFilePath(projectPath);

            string[] contentOfBatch = { String.Format("cd {0}\nenvimet4_console.exe {1} {1} {2}\npause", mainFolderWithBase, projectFolderName, simulationFileName) };

            System.IO.File.WriteAllLines(fullPathBatch, contentOfBatch);

        }

        public static string GetBatchFilePath(string projectPath)
        {
            return System.IO.Path.Combine(projectPath, "envimetSimulation.bat");
        }

        public static void DeleteBatchFile(string projectPath)
        {
            // delete file
            System.IO.File.Delete(GetBatchFilePath(projectPath));

        }

        public static string CreateDestinationFolder()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string destinationFolder = System.IO.Path.Combine(appDataFolder, "DragonflyEnvimet");

            // create folder if not exist
            if (!(System.IO.Directory.Exists(destinationFolder)))
                System.IO.Directory.CreateDirectory(destinationFolder);

            return destinationFolder;
        }
    }
}
