using System;
using Grasshopper.Kernel;
using System.Windows.Forms;
using df_envimet_lib.IO;

namespace df_envimet.Grasshopper.UI_GH
{
    public abstract class ExtensionGridFolderComponent : GH_Component
    {
        protected string _value = GridOutputFolderType.ATMOSPHERE;

        public ExtensionGridFolderComponent(string name, string nickName, string description, string category, string subCategory)
            : base(name, nickName, description, category, subCategory)
        {
        }


        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem atmosphere = Menu_AppendItem(menu, "Atmosphere", SetAtmosphere, true);
            ToolStripMenuItem pollutants = Menu_AppendItem(menu, "Pollutants", SetPollutants, true);
            ToolStripMenuItem radiation = Menu_AppendItem(menu, "Radiation", SetRadiation, true);
            ToolStripMenuItem soil = Menu_AppendItem(menu, "Soil", SetSoil, true);
            ToolStripMenuItem surface = Menu_AppendItem(menu, "Surface", SetSurface, true);
            ToolStripMenuItem solaraccess = Menu_AppendItem(menu, "Solar access", SetSolar, true);
            // Specifically assign a tooltip text to the menu item.
            atmosphere.ToolTipText = "Click on it to select atmosphere results.";
            pollutants.ToolTipText = "Click on it to select pollutants results.";
            radiation.ToolTipText = "Click on it to select radiation results.";
            soil.ToolTipText = "Click on it to select soil results.";
            surface.ToolTipText = "Click on it to select surface results.";
            solaraccess.ToolTipText = "Click on it to select solaraccess results.";
        }

        private void SetAtmosphere(object sender, EventArgs e)
        {
            _value = GridOutputFolderType.ATMOSPHERE;
            ExpireSolution(true);
        }

        private void SetPollutants(object sender, EventArgs e)
        {
            _value = GridOutputFolderType.POLLUTANTS;
            ExpireSolution(true);
        }

        private void SetSoil(object sender, EventArgs e)
        {
            _value = GridOutputFolderType.SOIL;
            ExpireSolution(true);
        }

        private void SetRadiation(object sender, EventArgs e)
        {
            _value = GridOutputFolderType.RADIATION;
            ExpireSolution(true);
        }

        private void SetSurface(object sender, EventArgs e)
        {
            _value = GridOutputFolderType.SURFACE;
            ExpireSolution(true);
        }

        private void SetSolar(object sender, EventArgs e)
        {
            _value = GridOutputFolderType.SOLAR_ACCESS;
            ExpireSolution(true);
        }

    }
}
