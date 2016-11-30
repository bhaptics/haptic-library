# tactosy-unity-plugin
## How to install
* from unity asset store 
   * download link from here : 	https://www.assetstore.unity3d.com/#!/content/76647
* download package file
   * https://github.com/bhaptics/tactosy-unity/releases/latest
* from github repository 
    ```
    git clone https://github.com/bhaptics/tactosy-unity.git
    ```

## How to use
* Manage individual motors 
```
// Turn on 1st, 2nd and 3rd motors of left hand tactosy with intensity 100
byte[] bytes = new byte[20];
bytes[0] = 100;
bytes[1] = 100;
bytes[2] = 100;
TactosyManager.SendSignal("dot", PositionType.Left, bytes, 500);
```
* Manage individual motors 
```
// Turn on feedback with logical coordinate [0, 1]
TactosyManager.SendSignal("PathFeedback", PositionType.All, new List<PathPoint>
{
    new PathPoint(0.5, 0.2, 0.5),
    new PathPoint(1, 1, 1)
}, 2000);

```


* Use Custom Signal 
  
  * Make haptic feedback using [tactosy studio](https://studio.bhaptics.com), and download tactosy file
  
   
  * Set up feedback mapping as below at unity inspector
        ![image](https://github.com/bhaptics/tactosy-unity/raw/master/Images/feedback_mapping.png)
 
 
 
   * Play feedback with key
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
