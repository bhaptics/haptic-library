# Unity plugin
* This project will help integrate bHaptics' haptic devices into Unity environments. 
* Current version is 1.4.11

## 1. Prerequisite (Only for Windows)
### The bHaptics Player needs to be installed
* The apps can be found at <br/>
   bHaptics homepage: [http://www.bhaptics.com](http://bhaptics.com/download/)

## 2. How to install
### (1) Download from the Unity Asset Store 
* https://assetstore.unity.com/packages/tools/integration/bhaptics-haptic-plugin-76647

### (2) Download the package file, then import it into a Unity Project
* https://github.com/bhaptics/haptic-library/releases


## 3. Tutorial Videos
* Tutorials : https://www.youtube.com/playlist?list=PLfaa78_N6dlvd0Ha0s0Y_LT62-Oqp8N2A
 
## 4. How to use 
* Please refer to the examples scenes in Assets/bHapticsManager/Examples/Scenes/
### 4.1 [bHaptics Manager] Prefab
>Add the [bHaptics Manager] Prefab to your scene.<br>
[bHaptics Manager] is located in Assets/bHapticsManager/Prefabs

![image](https://user-images.githubusercontent.com/1837913/60635096-6c1c2e00-9e4c-11e9-8aff-4ae28e72f235.png)

### 4.2 TactSource
>Add TactSource to the GameObject in the inspector<br>
You can select FeedbackType in the inspector

#### 4.2.1 FeedbackType(DotMode, PathMode)
  * Specify position, motors to be vibrated and duration in milliseconds. <br>
  
![image](https://user-images.githubusercontent.com/1837913/60635673-89ea9280-9e4e-11e9-8a8e-475acf57d40e.png)

#### 4.2.2 FeedbackType(TactFile)
  * Specify tact file(generated from bHaptics Designer)
  * Duration Multiplier: change duration of haptic feedback dynamically.
  * Intensity Multiplier: change intensity of haptic feedback dynamically.
  * Angle(X) and Offset(Y): change location of haptic feedback dynamically. <br>
      This allows you to make only one tact file effect and then reuse it at any location. <br>
      Only for the Tactot. <br>
      
![image](https://user-images.githubusercontent.com/1837913/60635769-e8b00c00-9e4e-11e9-9115-d783ce8c1027.png)

### (3) How to use it in your script.
```
GetComponent<TactSource>().Play();
```

## 5. Options in [bHapticsManager]
### (1) visualizeFeedback
* Enable/disable visualization of haptic feedback (Recommended only for dev)

### (2) LaunchPlayerIfNotRunning (Windows only)
* Enable/disable launching bHaptics Player if it is installed and it is not running.

### (3) IsActivateWidget (Android Only)
* This widget is equivalent to the bHaptics Player for maintaining pairing devices.
* If you click or touch the logo, the widget will be activated for maintaining pairing.

![gif](https://user-images.githubusercontent.com/1837913/60641460-d17d1880-9e66-11e9-8171-74d1709dfb67.gif)

## 6. Android Issues
### (1) Prerequisite
* Make sure that all the android settings with unity are finished.
* https://unity3d.com/kr/learn/tutorials/topics/mobile-touch/building-your-unity-game-android-device-testing
* The version of the Android device must be higher or equal to 4.3(API level 18) 
   - https://developer.android.com/guide/topics/connectivity/bluetooth-le
* For Oculus Quest, check this [documentation]( https://github.com/bhaptics/haptic-library/wiki/Getting-Started-(Unity---Oculus-Quest))

### (2) AndroidManifest.xml for permission
* Add the following permissions into the AndroidManifest.xml.

```xml
<manifest>
    <!--Bluetooth related permissions to connect bHaptics devices. -->
    <uses-permission android:name="android.permission.BLUETOOTH" />
    <uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    
     <!--File related permissions share paring device information with other apps. This is not necessary -->
     <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
     <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
</manifest>
```

* Otherwise, copy AndroidManifest-bhaptics.xml into your project
>Assets/Plugins/Android/AndroidManifest.xml.

### (3) Samples 
* https://github.com/bhaptics/unity-examples
* sample game demo apk: http://release.bhaptics.com/oculus-quest/latest-solodemo-oculusquest


## 7. Notes
### Overview
* For migration, remove Assets/bHapticsManager/ folder and import latest plugin. 
   * If you want to use haptic feedback files, just delete all except the feedback file's folder
* After importing files, just press Unity editor's play button once, and feedback files will be restored.

### (1) Migration from 1.4.4
* Tact File setting may be broken. Please reconnect the tact file in TactSource.

![image](https://user-images.githubusercontent.com/1837913/56008174-39afd880-5d16-11e9-8453-a88258296df6.png)


### (2) Migration from 1.3.1

* Namespace changed from Bhaptics.Tac to Bhaptics.Tact
```
Bhaptics.Tac --> Bhaptics.Tact 
Bhaptics.Tac.Unity --> Bhaptics.Tact.Unity
```

### (3) Migration from 1.3.0
```
// from 
BhapticsManager.HapticPlayer.SubmitRegistered("BowShoot");

// to 
BhapticsManager.HapticPlayer.SubmitRegistered(BhapticsManager.GetFeedbackId("BowShoot"));
```

### (4) Migration from 1.2.2 

```
// from 
SubmitRegistered(string key, TransformOption option)

// to 
SubmitRegisteredVestRotation(string key, RotationOption option)

// from
SubmitRegistered(string key, float intensityRatio, float durationRatio)

// to
SubmitRegistered(string key, ScaleOption option) 
```


### (5) Migration from 1.0.3 to 1.0.4
```
var hapticPlayer = FindObjectOfType<BhapticsManager>().HapticPlayer();

// To 
var hapticPlayer = BhapticsManager.HapticPlayer;
```

##### Last update of README.md: Dec 17th, 2019.

##### Copyright 2017~19 bHaptics Inc.
