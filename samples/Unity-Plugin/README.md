# Unity plugin to activate tactosy
This project is to use Tactosy in unity. 
## Prerequisite
* Tactosy app installed (Window or Mac)
   * [bHaptics](http://bhaptics.com/app.html)

## How to install
* Download package file then import package in unity
   * https://github.com/bhaptics/tactosy-sharp/releases
  ### or
* Get from github repository then open it in unity

    ```
    git clone https://github.com/bhaptics/tactosy-sharp.git
    ```
    
## How to use
* Test Default scene 
    
    >Go to Assets > Tactosy > Example > open Default    
    Select [Tactosy] Prefab<br/>
    There are 2 feedbacks, which already set for test.      
    You can check it by pushing the button
    
* Apply more feedbacks 
    
    >You can create Tactosy feebacks via https://studio.bhaptics.com<br/>
    For more detail, you can find in http://bhaptics.com/studio.html
    
* Initialize TactosyPlayer 
    ```
     public TactosyPlayer TactosyPlayer;
     private ISender sender;
     private ITimer timer;
     
     sender = new WebSocketSender();
     timer = GetComponent<UnityTimer>();
     TactosyPlayer = new TactosyPlayer(sender, timer);
    ```

* Play registered tactosy feedbacks with key
    ```
    TactosyPlayer.SendSignal("ArrowRelease", .2f); // play from specific feedback point
    TactosyPlayer.SendSignal("Fireball");
    ```

* TurnOff Signal 
    ```
    TactosyPlayer.TurnOff(); // turn off all tactosy feedback

    TactosyPlayer.TurnOff("Fireball);  // turn off feedback by key
    ```

* Check if Playing
    ```
    bool isFireballFeedbackPlaying = TactosyPlayer.IsPlaying("Fireball");
    bool isAnyFeedbackPlaying = TactosyPlayer.IsPlaying();
    ```    
## Dependencies 
> websocket sharp : https://github.com/sta/websocket-sharp
>
> json parser : http://www.newtonsoft.com/json
#####

Copyright 2017 bHaptics Co., Ltd.
