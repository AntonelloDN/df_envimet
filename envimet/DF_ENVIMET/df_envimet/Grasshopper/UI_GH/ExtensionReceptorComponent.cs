using System;
using Grasshopper.Kernel;
using System.Windows.Forms;
using df_envimet_lib.IO;

namespace df_envimet.Grasshopper.UI_GH
{
    public abstract class ExtensionReceptorComponent : GH_Component
    {
        protected string _value = ReceptorFileType.ATMOSPHERE;

        public ExtensionReceptorComponent(string name, string nickName, string description, string category, string subCategory)
            : base(name, nickName, description, category, subCategory)
        {
        }
        
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem atmosphere = Menu_AppendItem(menu, "Atmosphere", SetValueToAtmosphere, true);
            ToolStripMenuItem soil = Menu_AppendItem(menu, "Soil", SetValueToSoil, true);
            ToolStripMenuItem flux = Menu_AppendItem(menu, "Flux", SetValueToFlux, true);
            // Specifically assign a tooltip text to the menu item.
            atmosphere.ToolTipText = "Click on it to select only atmosphere output.";
            soil.ToolTipText = "Click on it to select only soil output.";
            flux.ToolTipText = "Click on it to select only flux output.";
        }

        private void SetValueToAtmosphere(object sender, EventArgs e)
        {
            _value = ReceptorFileType.ATMOSPHERE;
            ExpireSolution(true);
        }

        private void SetValueToSoil(object sender, EventArgs e)
        {
            _value = ReceptorFileType.SOIL;
            ExpireSolution(true);
        }

        private void SetValueToFlux(object sender, EventArgs e)
        {
            _value = ReceptorFileType.FLUX;
            ExpireSolution(true);
        }

    }
}