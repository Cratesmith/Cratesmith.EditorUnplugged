
# Editor Unplugged
![image](https://user-images.githubusercontent.com/4616107/67143439-c8e20e00-f2ae-11e9-9853-6c1e900ae5f6.png)A battery saving toolkit to make the Unity3d editor more laptop battery friendly.


## Quick Start 

### Installation


Add the following lines to your project's `manifest.json` (in the packages folder of your project)

    "scopedRegistries": [
        {
          "name": "Cratesmith",
          "scopes": [
            "com.cratesmith"
          ],
          "url": "http://upm.cratesmith.com:4873"
        }
      ]

Your `manifest.json` should look something like this

    {
      "scopedRegistries": [
        {
          "name": "Cratesmith",
          "scopes": [
            "com.cratesmith"
          ],
          "url": "http://upm.cratesmith.com:4873"
        }
      ],
    
      "dependencies": {
        "com.unity.2d.sprite": "1.0.0",
        "com.unity.2d.tilemap": "1.0.0",
        "com.unity.cinemachine": "2.3.4",
        ... etc ....
        }
    }

 Select **Window > Package Manager** from the menu
 
 Find **Cratesmith.Editor Unplugged** in the list (you may need to wait for it to refresh packages first)  and click **Install**
![editorunplugged-packagewindow](https://user-images.githubusercontent.com/4616107/67143273-25442e00-f2ad-11e9-9fa5-6463a23cbd17.gif)
## What EditorUnplugged does
It adds a nifty toolbar into your scene view that helps you track battery usage and control features that affect battery life!



### Session time counter
![image](https://user-images.githubusercontent.com/4616107/67143849-1f514b80-f2b3-11e9-84f2-b374db761419.png)
This shows how long unity has been running since startup.



### Battery status
![editorunplugged-battery](https://user-images.githubusercontent.com/4616107/67143886-9b4b9380-f2b3-11e9-82ce-0935cc07e625.gif)
Information about how the battery's current status and usage trends.
Check the tooltips for detailed descriptions on each field.





### FPS limiter (recommended at all times!)
![image](https://user-images.githubusercontent.com/4616107/67143897-ccc45f00-f2b3-11e9-8073-6db5d8978efb.png)
Restricts the game's framerate in the Unity editor if enabled. 
*(Setting this to 15 or 30 will significantly reduce GPU related battery use!)*







### Manual Reload/Recompile (recommended when coding!)
![image](https://user-images.githubusercontent.com/4616107/67143917-f7aeb300-f2b3-11e9-8b90-757814a17357.png)
Adds a toggle for easily switching Auto-Refresh assets on and off. It also shows how many times scripts have been reloaded this session, and how long the last script reload took.

When auto is disabled press **ctrl-r** (or press the button) to recompile or load changed assets.

*(Setting disabling auto significantly cuts down on CPU usage from unnecessary reloads when editing scripts)*

### Closes the package manager window when reloading scripts 
This is an odd one... but currently the package manager window in 2018/19 has a habit of making reload times much longer if it's open (even in a tab!). So just as a safety it'll now automatically be closed the moment a compile starts.



### When to compress assets ('On Build' recommended!)
![image](https://user-images.githubusercontent.com/4616107/67143932-2fb5f600-f2b4-11e9-8deb-3767f982b546.png)
This drop down lets you easily control when assets get compressed for builds.
*(Setting 'On Build' will make build times longer if there are new assets since the last build, but saves quite a lot of CPU usage)*






### Lightmapping ('On Demand' is strongly recommended!)
![image](https://user-images.githubusercontent.com/4616107/67143946-6b50c000-f2b4-11e9-9424-60f566b23259.png)
This drop down allows you to quickly switch between different lightmapping behaviours. 
*(Setting 'On Demand' prevents the lightmapping engine recalculating Baked/Realtime GI when the scene is changed, which can be **very** GPU and CPU intensive)*


## What EditorUnplugged doesn't do
![windows-battery](https://user-images.githubusercontent.com/4616107/67144008-1ceff100-f2b5-11e9-8d42-f2bee71bb52b.gif)

This plugin doesn't alter your power settings or other applications at all. To get any real savings you should close any programs you aren't using (especially anything using the GPU!)


It also doesn't do anything at all outside of the editor. (Optimizing your game for battery is up to you!)

