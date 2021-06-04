﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Grasshopper.Kernel;

namespace df_envimet.Grasshopper.IO
{
    public class ReadLibrary : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ReadLibrary class.
        /// </summary>
        public ReadLibrary()
          : base("DF Envimet Read Library", "DFReadLibrary",
              "Use this component to read the library of materials of ENVI_MET.\nUse the \"id\" to change materials to enviment objects.\nUse DF XML Reader to look at details of materials. ",
              "DF-Legacy", "3 | Envimet")
        {
            this.Message = "VER 0.0.04\nJUN_06_2021";
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("_selectMaterial", "_selectMaterial", "Use this component to select materials from ENVI_MET DB. Connect a panel with one of following text:\nMATERIAL\nWALL\nSOIL\nPROFILE\nSOURCE\nPLANT\nPLANT3D\nGREENING.\nDefault is PROFILE.", GH_ParamAccess.item, "PROFILE");
            pManager.AddTextParameter("searchMaterial_", "searchMaterial_", "Add a keyword to seach material you are looking for. For example, sand.", GH_ParamAccess.item);
            pManager.AddTextParameter("envimetFolder_", "envimetFolder_", "Connect It if you want to read the model library.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("profileId", "profileId", "Profile Soil material. Use an ITEM SELECTOR to select material you want.", GH_ParamAccess.list);
            pManager.AddGenericParameter("description", "description", "Read the meaning of the ids.", GH_ParamAccess.list);
            pManager.AddGenericParameter("XML", "XML", "XML file of selected materials. Use DF XML Reader to extract details.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // init
            string _selectMaterial = "PROFILE";
            string searchMaterial_ = null;
            string envimetFolder_ = null;

            DA.GetData(0, ref _selectMaterial);
            DA.GetData(1, ref searchMaterial_);
            DA.GetData(2, ref envimetFolder_);

            // manage uppercase
            string selectedMaterial = _selectMaterial.ToUpper();

            switch (selectedMaterial)
            {
                case "MATERIAL":
                    Params.Output[0].Name = "materialId";
                    Params.Output[0].NickName = "materialId";
                    Params.Output[0].Description = "Construction material. Use an ITEM SELECTOR to select material you want.";
                    break;
                case "WALL":
                    Params.Output[0].Name = "wallMaterialOrRoofMaterial";
                    Params.Output[0].NickName = "wallMaterialOrRoofMaterial";
                    Params.Output[0].Description = "Wall material / Roof material. Use an ITEM SELECTOR to select material you want.";
                    break;
                case "SOIL":
                    Params.Output[0].Name = "soilId";
                    Params.Output[0].NickName = "soilId";
                    Params.Output[0].Description = "Soil material. Use an ITEM SELECTOR to select material you want.";
                    break;
                case "SOURCE":
                    Params.Output[0].Name = "sourceId";
                    Params.Output[0].NickName = "sourceId";
                    Params.Output[0].Description = "Source material. Use an ITEM SELECTOR to select material you want.";
                    break;
                case "PLANT":
                    Params.Output[0].Name = "plantId";
                    Params.Output[0].NickName = "plantId";
                    Params.Output[0].Description = "Plant material. Use an ITEM SELECTOR to select material you want.";
                    break;
                case "PLANT3D":
                    Params.Output[0].Name = "plant3Did";
                    Params.Output[0].NickName = "plant3Did";
                    Params.Output[0].Description = "Plant 3d material. Use an ITEM SELECTOR to select material you want.";
                    break;
                case "GREENING":
                    Params.Output[0].Name = "greeningId";
                    Params.Output[0].NickName = "greeningId";
                    Params.Output[0].Description = "Greening material. Use an ITEM SELECTOR to select material you want.";
                    break;
                default:
                    Params.Output[0].Name = "profileId";
                    Params.Output[0].NickName = "profileId";
                    Params.Output[0].Description = "Profile Soil material. Use an ITEM SELECTOR to select material you want.";
                    break;
            }


            // action
            string mainDirectory = df_envimet_lib.IO.Workspace.FindENVI_MET();

            string dbFile = null, dbName = null;
            if (mainDirectory != null)
            {
                dbFile = System.IO.Path.Combine(mainDirectory, "database.edb");
                dbName = "database";
            }
            else
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Use DF Envimet Installation Directory to set installation folder of envimet.");
                return;
            }

            if (envimetFolder_ != null)
            {
                dbFile = System.IO.Path.Combine(envimetFolder_, "projectdatabase.edb");
                dbName = "userDatabase";
                if (!System.IO.File.Exists(dbFile))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Connect the output of the current mainspace.");
                    return;
                }
            }

            if (mainDirectory != null)
            {
                // obj with property initilizer
                df_envimet_lib.IO.Read metafile = new df_envimet_lib.IO.Read() { Metaname = dbFile };

                // destination file
                string destinationFolder = df_envimet_lib.IO.Workspace.CreateDestinationFolder();
                string readbleXml = metafile.WriteCleanXml(destinationFolder, dbName, ".xml");

                XDocument xml = XDocument.Load(readbleXml);

                // query and workaround for greenings
                string word = (selectedMaterial != "GREENING") ? "Description" : "Name";

                var estrazione = (searchMaterial_ != null) ?
                  from dato in xml.Descendants(selectedMaterial)
                  from description in dato.Descendants(word)
                  from id in dato.Descendants("ID")
                  where description.Value.ToUpper().Contains(searchMaterial_.ToUpper())
                  select Tuple.Create(id.Value.ToUpper(), description.Value.ToUpper(), dato) :
                  from dato in xml.Descendants(selectedMaterial)
                  from descrizione in dato.Descendants(word)
                  from id in dato.Descendants("ID")
                  select Tuple.Create(id.Value.ToUpper(), descrizione.Value.ToUpper(), dato);

                // create lists
                List<string> ids = new List<string>();

                if (selectedMaterial != "PLANT3D")
                {
                    ids = estrazione.Select(e => e.Item1.Replace(" ", ""))
                                        .ToList();
                }
                else
                {
                    ids = estrazione.Select(e => e.Item1.Replace(" ", "") + ",." + e.Item2.Replace(" ", ""))
                        .ToList();
                }

                var descriptions = estrazione.Select(e => e.Item2)
                  .ToList();
                var details = estrazione.Select(e => e.Item3)
                  .ToList();

                // output
                DA.SetDataList(0, ids);
                DA.SetDataList(1, descriptions);
                DA.SetDataList(2, details);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Envimet Main Folder not found!");
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.envimetReadLib;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("efebae6f-cfdb-4111-b9b1-3e56ec606ae7"); }
        }
    }
}