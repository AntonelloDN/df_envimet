Here you can find a beta version of dragonfly components. They will be released in official Dragonfly Legacy as soon as they are tested enough.

## Installation

For those people who want to try them, please follow these steps:
<ol>
  <li>Download "pluginBeta.zip".</li>
  <li>Check if downloaded .zip file has been blocked: right click on it, and choose Properties. If there is an Unblock button click on it, otherwise it is OK. Unzip it.</li>
  <li>Go to Libraries folder of Grasshopper: Open Grasshopper and go to File->Special Folders->Components Folder.</li>
  <li>Copy and paste "Dragonfly" folder wherever you want on your machine in order to make a backup copy of it.</li>
  <li>Replace the files with new ones within "pluginBeta".</li>
  <li>Restart Rhino</li>
</ol>

What will happen to your existing gh files? Some components are the same of the previous versions, other required a replacement. I suggest save your workflows in the same folder of backup of plugin.

## New features

[2019/01/21]:
- Timesteps settings for simulation file;
- Building indoor temperature for simulation file;
- Terrain (early adopter);
- Move building component for terrain;

[2019/01/23]:
- LBC settings for simulation file;

[2019/02/09]:
- Soil init condition for simulation file;

[2019/02/20]:
- Edit building materials (E.g. for windows);<br>
![Alt Text](https://github.com/AntonelloDN/df_envimet/blob/master/envimet/EditBuildingMaterialComponent.png)

[2019/02/24]:
- Horizontal overhang;

[2019/03/06]:
- Run simulation directly with Grasshopper;
- Output file timing settings;
- LBC settings for simulation file - removed because of latest ENVI_MET 4.4.1;
