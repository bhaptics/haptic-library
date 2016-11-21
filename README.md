# tactosy-unity-plugin
## How to install
* from unity asset store 
   * download link from here : 	https://www.assetstore.unity3d.com/#!/content/76647
* download package file
   * https://github.com/bhaptics/tactosy-unity/releases/download/0.0.1/Tactosy-Unity.unitypackage
* from github repository 
```
git clone https://github.com/bhaptics/tactosy-unity.git
```

## How to use
* Manage individual motors 
```
// Turn on 0, 1, 2 motor with intensity 100
byte[] bytes = new byte[20];
bytes[0] = 100;
bytes[1] = 100;
bytes[2] = 100;
TactosyManager.SendSignal("dot", bytes, 500);
```

* Use Custom Signal 
   * Tactosy File from https://studio.bhaptics.com
   * Download tactosy file
```
TactosyManager.SendSignal("reload", Intensity, Duration);
TactosyManager.SendSignal("Electricgun");
```
* TurnOff Signal 
```
TactosyManager.TurnOff();
TactosyManager.TurnOff("reload);
```

* Check if Playing
```
bool isReloadFeedbackPlaying = TactosyManager.IsPlaying("reload");
bool isAnyFeedbackPlaying = TactosyManager.IsPlaying();
```

## Dependencies 
* websocket sharp : https://github.com/sta/websocket-sharp
* json parser : http://www.newtonsoft.com/json
