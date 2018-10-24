# Unity plugin
* This project will help to integrate bHaptics' haptic devices into Unity environments. 
* Current version is 1.4.1

## 1. Prerequisite
### bHaptics Player needs to be installed (Windows 8.1, 10, Android)
* The apps can be found at <br/>
   bHaptics webpage: [http://www.bhaptics.com](http://bhaptics.com/download/)
* Android Appstore : https://play.google.com/store/apps/details?id=com.bhaptics.player

## 2. How to install
### (1) Download from the Unity Asset Store 
* https://www.assetstore.unity3d.com/en/#!/content/76647

### (2) Download the package file, then import it into a Unity Project
* https://github.com/bhaptics/tac-sharp/releases
  
### (3) Clone from the github repository, then open it in Unity

```
git clone https://github.com/bhaptics/tac-sharp.git
```

## 3. Tutorial Videos
* Tutorials : https://www.youtube.com/playlist?list=PLfaa78_N6dlvd0Ha0s0Y_LT62-Oqp8N2A
	
## 4. How to use
### (1) Default Test Scene 

```
>Go to Assets > bHapticsManager > Examples > open 1. Simple Example with TactSouce.scene
Select the [bHaptics Manager] Prefab in the scene.
Some example feedback effects are automatically loaded, ready for testing.
You can check each feedback effect by pushing the corresponding button while playing in the editor.
```

### (2) To apply to your own project, just add the [bHaptics Manager] Prefab to your scene.

### (3) Then add TactSource to the GameObject in the inspector.


## 5. Options in [bHapticsManager]
### (1) visualizeFeedbacks 
* Enable/disable visualization of haptic feedback

### (2) LaunchPlayerIfNotRunning (Windows only)
* Enable/disable launching bHaptics Player if it is installed and it is not running.


## 6. UWP Issues
* Please check uwp-issue.pdf


## 7. Android Issues
### (1) Prerequisite
* Make sure that all the android setting with unity must be fininshed.
* https://unity3d.com/kr/learn/tutorials/topics/mobile-touch/building-your-unity-game-android-device-testing
* THe version of the Android device must be higher or equals to 4.3(API level 18) - https://developer.android.com/guide/topics/connectivity/bluetooth-le

### (2) If there is not a paired device, then the SDK will do nothing.

### (3) setting for AndroidManifest.xml 
* Please refer to AndroidManifest-bhaptics.xml
* If your project contains a custom AndroidManifest.xml file, copy 

```
      <service android:name="com.bhaptics.tact.ble.BhapticsService">
        <intent-filter>
            <action android:name="com.bhaptics.player.Service" />
            <action android:name="android.intent.action.MAIN" />
            <category android:name="android.intent.category.DEFAULT" />
        </intent-filter>
      </service>
	  
	  <activity android:name="com.bhaptics.tact.unity.HapticPlayerWrapper"
                android:label="@string/app_name"
                android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen">
      </activity>
```

* Otherwise, copy this file to this location in your project:
```
        Assets/Plugins/Android/AndroidManifest.xml.
```

### (4) Checkout how unity manage AndroidMenifest.xml 
* https://docs.unity3d.com/2018.1/Documentation/Manual/android-manifest.html

### (5)How to install the bhaptics Player at Daydream standalone devices.
* https://uploadvr.com/android-daydream-app-standalone-how-to/


## 8. Notes
### (1) Migration to 1.3.1

* namespace changed from Bhaptics.Tac to Bhaptics.Tact
   
```
Bhaptics.Tac --> Bhaptics.Tact 
Bhaptics.Tac.Unity --> Bhaptics.Tact.Unity
```


### (2) Migration to 1.3.0

```
// from 
BhapticsManager.HapticPlayer.SubmitRegistered("BowShoot");

// to 
BhapticsManager.HapticPlayer.SubmitRegistered(BhapticsManager.GetFeedbackId("BowShoot"));
```


### (3) Migration to 1.2.2 

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



### (4) Migration from 1.0.3 to 1.0.4

```
var hapticPlayer = FindObjectOfType<BhapticsManager>().HapticPlayer();

// To 
var hapticPlayer = BhapticsManager.HapticPlayer;
```


<br>
Last update of README.md: Oct 24th, 2018.
<br>
Copyright 2017 bHaptics Inc.
