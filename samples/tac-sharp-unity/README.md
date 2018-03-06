# Unity plugin
This project will help to integrate bHaptics' haptic devices into Unity environments. 

## Prerequisite
* bHaptics Player needs to be installed (Windows)
   * The app can be found at <br/>
   bHaptics webpage: [http://www.bhaptics.com](http://bhaptics.com/download/)

## How to install
### Download from the Unity Asset Store 
* https://www.assetstore.unity3d.com/en/#!/content/76647

### Download the package file, then import it into a Unity Project
* https://github.com/bhaptics/tac-sharp/releases
  
### Clone from the github repository, then open it in Unity

```
git clone https://github.com/bhaptics/tac-sharp.git
```

## Tutorial Videos
* Tutorials : https://www.youtube.com/playlist?list=PLfaa78_N6dlvd0Ha0s0Y_LT62-Oqp8N2A
	
## How to use
* Default Test Scene 

```
>Go to Assets > bHapticsManager > Examples > open 1. Simple Example with TactSouce.scene
Select the [bHaptics Manager] Prefab in the scene.
Some example feedback effects are automatically loaded, ready for testing.
You can check each feedback effect by pushing the corresponding button while playing in the editor.
```

* To apply to your own project, just add the [bHaptics Manager] Prefab to your scene.

* Then add TactSource to the GameObject in the inspector.


## Options in [bHapticsManager]
### visualizeFeedbacks 
* Enable/disable visualization of haptic feedback

### LaunchPlayerIfNotRunning
* Enable/disable launching bHaptics Player if it is installed and it is not running.

## Notes
* Migration to 1.3.0

```
// from 
BhapticsManager.HapticPlayer.SubmitRegistered("BowShoot");

// to 
BhapticsManager.HapticPlayer.SubmitRegistered(BhapticsManager.GetFeedbackId("BowShoot"));
```


* Migration to 1.2.2 

```
// from 
SubmitRegistered(string key, TransformOption option)

// to 
SubmitRegisteredVestRotation(string key, RotationOption)

// from
SubmitRegistered(string key, float intensityRatio, float durationRatio)

// to
SubmitRegistered(string key, ScaleOption option) 
```



* Migration from 1.0.3 to 1.0.4

```
var hapticPlayer = FindObjectOfType<BhapticsManager>().HapticPlayer();

// To 
var hapticPlayer = BhapticsManager.HapticPlayer;
```


<br>
Copyright 2017 bHaptics Inc.
