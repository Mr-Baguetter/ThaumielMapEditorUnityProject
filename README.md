# ThaumielMapEditorUnityProject

A visual map editor and schematic builder for Unity. This tool allows you to build custom map schematics visually in the Unity Editor and export them for use in your SCP: SL server.

## Installation

1. Download and install [Unity Hub](https://unity.com/download) - (https://unity.com/download).
2. Install **Unity 6000.4.0f1** via Unity Hub.
3. Download this repository: click the green **Code** button at the top of the page, then select **Download ZIP**.
4. Unzip the downloaded folder to a location of your choice.
5. Open Unity Hub, click **Add**, and select the unzipped folder to import the project.

## Configuration

Before exporting your first schematic, you can configure your builder settings directly inside the Unity Editor.

1. On the top toolbar in Unity, navigate to **SchematicManager > Builder Config**.
2. A configuration window will appear with the following options:
   * **Export Path:** Choose where your files will be saved. By default, this will create a folder at `Desktop\ThaumielMapEditor`.
   * **Compress Export:** Check this box to automatically zip your compiled schematics.
   * **Open Export:** Check this box to automatically open the folder in your file explorer once the build is complete.
3. **PMER Users:** If you have an old PMER JSON config, you can click **Import PMER Settings** to automatically transfer your old settings over.
4. Click **Save Config** when you are done.

## Creating Your First Schematic

1. Use the prefabs in `Assets/Parts/...` to add objects to the scene.
2. Make sure all your objects are grouped under an object with the `Builder` script in the hierarchy.

## Compiling and Exporting

Once your map is complete and ready to go:

1. Go to **SchematicManager > Compile** or press **Ctrl + Shift + D**.
2. The editor will parse your scene and generate the schematic files.
3. If you left the settings at default, your new schematic will be waiting for you in the `ThaumielMapEditor` folder on your desktop!
4. To import the new schematic into your SCPSL server put the exported yml file in the **LabApi/configs/Thaumiel/Schematics**

## Decompiling Existing Schematics

Need to edit an already compiled schematic? You can easily decompile existing `.yml` files back into a Unity scene!

1. Go to **SchematicManager > Decompile Schematic** or press **Ctrl + Shift + E**.
2. A file explorer window will pop up. Select the `.yml` schematic file you want to edit.
3. The editor will automatically reconstruct the map in your scene, complete with all objects, doors, lockers, and specific tools.
4. The newly generated root object will automatically have the `Builder` script attached, so it is immediately ready to be edited and re-compiled!

## Community & Support

Need help, want to share your creations, or looking to report a bug? Join our community!
* **Discord:** [ThaumielMapEditor Guild](https://discord.gg/N8qrNHf4s9) - (https://discord.gg/N8qrNHf4s9)