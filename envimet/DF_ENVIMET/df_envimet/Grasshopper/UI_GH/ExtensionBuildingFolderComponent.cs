using System;
using Grasshopper.Kernel;
using System.Windows.Forms;
using df_envimet_lib.IO;

namespace df_envimet.Grasshopper.UI_GH
{
    public abstract class ExtensionBuildingFolderComponent : GH_Component
    {
        protected string _value = BuildingFileType.AVG_FILE;

        public ExtensionBuildingFolderComponent(string name, string nickName, string description, string category, string subCategory)
            : base(name, nickName, description, category, subCategory)
        {
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem avgOutput = Menu_AppendItem(menu, "Avg Output", SetValueToOneDimensionalResults, true);
            ToolStripMenuItem facadeOuput = Menu_AppendItem(menu, "Facade Output", SetValueToFacedeResults, true);
            // Specifically assign a tooltip text to the menu item.
            avgOutput.ToolTipText = "Click on it to select only avg building results.";
            facadeOuput.ToolTipText = "Click on it to select only file for facade results.";
        }

        private void SetValueToOneDimensionalResults(object sender, EventArgs e)
        {
            _value = BuildingFileType.AVG_FILE;
            ExpireSolution(true);
        }

        private void SetValueToFacedeResults(object sender, EventArgs e)
        {
            _value = BuildingFileType.FACADE_FILE;
            ExpireSolution(true);
        }
    }
}
