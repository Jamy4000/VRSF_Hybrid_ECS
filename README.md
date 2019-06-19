# VR Scriptable Framework using Hybrid ECS
This repository is a Unity Framework using Scriptable Objects and the Unity Input Manager for Inputs and Interaction, an Event System as seen in [Quill18 video](https://www.youtube.com/watch?v=04wXkgfd9V8) and the Hybrid ECS from Unity3D. It aims to ease the use of Virtual Reality in a project, and to have a light tool for that, while integrating a cross-platform project and some basic VR features. 

**Warning** The current Wiki documentation is not up-to-date. I need to take some time to explain the different features and how to use them, but as no request were made, I'm not gonna do that as top prio.
If you have any in question on how to use this tool, I'll be glad to answer you, just send me an email to arnaudbriche1994@gmail.com ! :)


## Description
The repository you're currently is a Crossfplatform, Lightweight VR framework giving you access to some basic features often used in VR. It's an alternative to Libraries like VRTK, that was way too big for me when I first used it. Numerous tools and scripts will come in the future, as well as other device supports.


The supported devices for now are :
- The HTC Vive
- The Microsoft Mixed Reality Headset
- The Oculus Rift with Touch Controllers
- The Oculus GO
- The Gear VR
- The Oculus Quest
- A VR Simulator (only recommended for debug)


# Releases
The stable versions are placed in the Releases section of this repository. Multiple packages are available, with extensions depending on your use. The only one you absolutely need is the VRSF_Hybrid_Core package.
**Warning** Depending on your Unity version, you must use one of those packages :
- [Unity 2018.x](https://github.com/Jamy4000/VRSF_Hybrid_ECS/releases/tag/v2.3 "VRSF 2.3")
- [Unity 2019.x](https://github.com/Jamy4000/VRSF_Hybrid_ECS/releases/tag/v2.4 "VRSF 2.4")
Those are the last releases for this repository, as we're now switching to the DOTS Workflow, meaning that we gonna recreate a new repository. More info to come ! 

## Libraries Required
To use this Framework, you gonna need to import the following stuffs :
- **Unity3D 2018.x or later** : Required to be able to use the ECS Hybrid System. An error occur in the Haptic Systems in 2019.x, you just need to change the IsInvalid parameter to isInvalid (Thanks Unity for this lovely change).
- **The Entities Package 0.0.12-Preview.21** : You can find it in the Package Manager from Unity (in Unity, Tab Window > Package Manager, in the Packages Window click on Advanced > Show Preview Packages, and then : All Packages > Entities 0.0.12-preview.21 > Install). I need to update the systems, as for now the project is only working with an old version of this package.
- **VR Support** : In the Player Settings Window (Edit > Project Settings > Player), go to the last tab called XR Settings, set the Virtual Reality Supported toggle to true, and add the Oculus, OpenVR and None SDKs to the list.
- **Scripting Runtime Version** : This one is normally set by default in the last versions of Unity, but we never know :  still in the Player Settings Window, go to the Other Settings tab and set the Scripting Runtime version to .NET 4.x Equivalent.

## Optional Libraries
You still need to import some VR Packages, depending on your needs, to use this framework :
- **Oculus for Desktop** : If you want to use the Rift or Rift S Support
- **Oculus for Desktop** : If you want to use the Oculus Go, Gear VR or Oculus Quest Support
- **Windows Mixed Reality** : If you want to use the WMR Headset
- **OpenVR for Desktop** : If you want to use the HTC Vive or HTC Focus (**WARNING :** MODELs FOR CONTROLLERS NOT PROVIDED)

### Oculus GO, Oculus Quest, Gear VR and HTC Focus Specifities
If you need to build for a mobile platform, you need as well to download the Android Building support (File > Build Settings > Android) and to switch the platform to Android.


Once all of that is done, **Restart your project so everything can be recompiled !**

## Basic Setup :

1. Import the different packages listed above
2. Relaunch the Editor to fix a bug sometimes appearing with the Entity Package
3. Import the VRSF_Core package
4. Import the other packages you need
5. Add SetupVR in your scene
6. Go to Edit > ProjectSettings > Input and use the Preset button to set the Inputs to the preset included in the Core Package from VRSF
7. Go to Edit > Player > Project Settings > XR Settings and tick the Virtual Reality Supported checkbox
8. Add, in this order, the Oculus SDK, OpenVR SDK, amnd None (For the Simulator)
9. Set the Start Position of your CameraRig using the CameraRig object
10. You should be good to go !

If you want to add anything more in your scene (Movements, UI, Gaze, ...), just check the prefabs in the different Extension Packages, or check the different scenes in the VRSF.Samples folder of this repository :)


## Credits
This repository is based on multiple repositories found online, and that's why I would like to thanks the following persons for their work that helped me through the development of this project :
- The work of [Thorsten Jänichen](https://github.com/TJaenichen) and [Thomas Masquart](https://github.com/ThmsMsqrt), co-author of the Scriptable Framework used in this repository. Their work is as well based on the excellent Unite Talk 2017 from Ryan Hipple, [available here](https://youtu.be/raQ3iHhE_Kk), and on its [Scriptable Objects Github repository](https://github.com/roboryantron/Unite2017).
- The EventCallbacks Plugin from [Quill18](https://www.youtube.com/watch?v=04wXkgfd9V8) and the rewriting of it by [CrazyFrog55](https://github.com/crazyfox55) and [FuzzyHobo](https://github.com/FuzzyHobo). I made my own version available [here](https://github.com/Jamy4000/UnityCallbackAndEventTutorial).
- The Vive-Teleporter offered by [FlaFla2](https://github.com/Flafla2/Vive-Teleporter) for the calculation and display of the Arc Teleporter.


## Documentation
For more info about this VR framework,please send me a message, as the Wiki is still a work in progress.

For more info about the Scriptable Objects and the Framework created, please check the Github Repository given above as well as the Unite talk and Example project provided by Unity and Ryan Hipple.

For more info about the Event System we are using, please check the Github Repository and video given above as well as the example project I've created on my Github page.
