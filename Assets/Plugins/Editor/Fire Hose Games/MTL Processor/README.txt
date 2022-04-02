----------------------------------------
            .MTL Processor
              Version 1.0
   Copyright © 2016 Fire Hose Games  
----------------------------------------

Please visit www.firehosegames.com/mtl-tool for tutorials, examples, and the latest information on the .MTL Processor.

----------------------------------------
              Installation
----------------------------------------

To install, purchase from the Asset Store and import the new package.
This software is only to be distributed under the terms of the Unity Asset Store Editor Extension License
In order to use, you must update your version of Unity to version 5.1.0 or higher


----------------------------------------
               User Guide
----------------------------------------

.MTL Processor was made to help expedite and simplify the importing of .OBJ's by giving a hands-off solution to setting up materials inside Unity. We designed this tool to help with an extremely time consuming part of a pipeline that we felt would benefit from some clever automation. There exist hundreds of thousands of 3D assets that can be used in your Unity projects but that aren't "Unity ready" and don't utilize Unity's Physically Based Shading technology. We wanted access to more than what was available on the Asset Store, and we wanted a way to set up those assets with minimal effort. 


----------------------------------------
               Navigation
----------------------------------------

From the menu bar:
   Window > MTL Processor >

     ..Options 
       Opens Global Options menu in the inspector

     ..MTL Alph-izer 
       Opens Alph-izer Window

       
----------------------------------------
     Auto-Import (default behavior)
----------------------------------------

Once the extension is installed, simply place an .OBJ into your project along with its corresponding .mtl file (and optional texture files) and the .MTL Processor will assign all of the appropriate material data to a new Unity Standard Shader.

This process can be performed on a single asset, or as a batch on a group of assets or your entire project. The method is essentially the same no matter how many assets or materials need to be converted.

If you're installing .MTL Processor for the first time, and you already have a number of .OBJ's in your project, a popup will appear asking if you'd like to process their materials. Click "Yes" to process. Click "No" to ignore these files. (You can always process them later if you'd like.)


----------------------------------------
              Manual Mode
----------------------------------------

By default, MTL Processor will automatically generate the correct mappings and values from .MTL files and place them into the Standard Shader. If, however, you'd like choose which assets are to be processed, you can do so by turning off "Process .MTL's" in the MTL Processor Options menu. When this is off, MTL Processor will no longer attempt to re-assign any values when an .OBJ is imported. To manually reimport the .MTL data on a specific asset, select the asset you'd like to convert, check on "Process .MTL's" found in the inspector, and press the Reimport Materials button.


----------------------------------------
     MTL Processor Options (Local)
----------------------------------------
* Global MTL Options
    This is a simple shortcut to take you to the MTL Processor Options screen where you can adjust the tool's default behavior.

* Process .MTL's (On by default)
    When checked, will transfer and reinterpret .mtl file data to Unity Standard Shader. 

* Override materials (Off by default)
    WARNING: When checked, will overwrite existing changes to materials that have already been converted. This is useful when you've made changes to a material after conversion and like to just get it back to it's original settings when it was imported.

* Reimport Materials (will not reimport mesh)
    This will reimport the original .MTL settings to the new Standard Shader materials, but will save time by not importing the mesh*.
    * If the asset's materials are deleted, you must reimport the mesh as the .MTL file and newly created material will become unlinked. 

    
----------------------------------------
            Options (Global)
----------------------------------------

  * Default MTL Import Settings
  * Process .MTL's
      When checked, will cause all newly imported .OBJ models to convert their materials using the MTL processor
  * Override Materials
      WARNING: When checked, will toggle all newly imported .OBJ models to overwrite existing changes to materials that have already been converted each time they are imported. 
  
  * Apply these settings to all models
      Current settings for this window will be applied to all eligible models
  * Process Materials for all models
      Reimports materials for all models
      
Settings
  * Show Logging Info
      Check this box to display messages from MTL Processor log in the Unity console.
  * Alph-izer File Suffix:
      This is the default suffix used when naming the generated Alph-ized texture. Example: [mytexturename]_ALPHIZED.png for suffix of "_ALPHIZED"


----------------------------------------
               Alph-izer
----------------------------------------

Got an old opacity map? Need a .PNG with an alpha channel? The Alph-izer can take any two textures and combine them into a 32bit .PNG. Select a color source, and an alpha source (b&w opacity map) and the Alph-izer will do the rest!

Open the Alph-izer window [Window > MTL Processor > MTL Alph-izer]

Select the Color Source texture
This is the texture which you want rendered but is missing an alpha channel. 

Select the Alpha Source texture 
This is the texture that will be used to mask the unwanted regions of the Color Source. Often these are labeled "opacity maps" or "transparency maps" in older assets, or assets coming from non-real-time environments like Maya or 3Ds Max.

Checking "White = Opaque" will flip the Alpha Source texture so that white is opaque and black is transparent.
Note that by default:
White = Fully Transparent
Black = Fully Opaque

Select the destination for your new texture.

Press the "Alph-ize!" Button.

The Alph-izer will generate a 32bit .PNG texture with RGBa channels. 

The Alph-izer will also work automatically on import if the .MTL Processor detects a texture map without an alpha channel (like an opacity map) that the .MTL file wants to use for transparency.
