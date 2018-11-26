# VR Scriptable Framework using Hybrid ECS
This repository is a Unity Framework based on Scriptable Objects, an Event System as seen in [Quill18 video](https://www.youtube.com/watch?v=04wXkgfd9V8) and the Hybrid ECS from Unity3D. It aims to ease the use of Virtual Reality in a project, and to have a light tool for that, while integrating a cross-platform project and some basic VR features. 

## Description
The repository you're currently on is linking the Framework to VR devices and give you access to some features often used in VR. It's an alternative to Libraries like VRTK, apart from the fact that this repository is still a work in progress. Numerous tools and scripts will come in the future, as well as other device supports.


The supported devices for now are :
- The HTC Vive
- The Microsoft Mixed Reality Headset
- The Oculus Rift with Touch Controllers
- The Oculus GO and Gear VR
- A VR Simulator (only recommended for debug)


## Libraries Required
To use this Framework, you gonna need to import the following stuffs :
- **Unity3D 2018.2 or later** : Required to be able to use the ECS Hybrid System.
- **Oculus Integration** : You can find it on the [Unity Asset Store](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) and can only import the basic scripts from Oculus (No need for the textures, models, materials, or Interaction System for example).
- **SteamVR 1.2.3** : For now you need to use the deprecated version of SteamVR, that you can find on [their Github Releases Page](https://github.com/ValveSoftware/steamvr_unity_plugin/releases/tag/1.2.3). This will be updated in the next weeks. You can as well only import the Core Scripts from SteamVR, and uncheck the materials, models, Texture, Interaction System, LongBow, ...
- **The Entities Package** : You can find it in the Package Manager offered by Unity3D (in Unity, Tab Window>Package Manager, in the Packages Window : All > Entities > Install) 
- **The Incremental Compiler** : You can find it in the Package Manager offered by Unity3D (in Unity, Tab Window>Package Manager, in the Packages Window : All > IncrementalCompiler > Install) 

### Oculus GO and Gear VR Specifities
If you need to build for Oculus GO or Gear VR, you need as well to download the Android Building support (File > Build Settings > Android) and to switch the platform to Android.

Once all of that is done, **Restart your project so everything can be recompiled !**


## Credits
The Scriptable Framework in this repository is based on multiple repositories found online, and that's why I would like to thanks the following persons for their work that helped me through the development of this project :
- The work of [Thorsten Jänichen](https://github.com/TJaenichen) and [Thomas Masquart](https://github.com/ThmsMsqrt), co-author of the Scriptable Framework used in this repository. Their work is as well based on the excellent Unite Talk 2017 from Ryan Hipple, [available here](https://youtu.be/raQ3iHhE_Kk), and on its [Scriptable Objects Github repository](https://github.com/roboryantron/Unite2017).
- The EventCallbacks Plugin from [Quill18](https://www.youtube.com/watch?v=04wXkgfd9V8) and the rewriting of it by [CrazyFrog55](https://github.com/crazyfox55) and [FuzzyHobo](https://github.com/FuzzyHobo). I made my own version available [here](https://github.com/Jamy4000/UnityCallbackAndEventTutorial).
- The Vive-Teleporter offered by [FlaFla2](https://github.com/Flafla2/Vive-Teleporter) for the calculation and display of the Arc Teleporter.


## Documentation
For more info about this VR framework, please check the Wiki Section on Github.

For more info about the Scriptable Objects and the Framework created, please check the Github Repository given above as well as the Unite talk and Example project provided by Unity and Ryan Hipple.

For more info about the Event System used, please check the Github Repository and video given above as well as the example project I've created on my Github page.


# Releases
The stable versions are placed in the Release section of this repository. Multiple packages are available, with extensions depending on your use. The only one you absolutely need is the VRSF_Hybrid_Core package.
