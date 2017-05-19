# Unity plugin to activate tactosy
This project helps to use Tactosy in Unity environments. 

## Prerequisite
* Tactosy app has to be installed (Windows or Mac)
   * The app can be found in
   bHaptics webpage: [http://www.bhaptics.com](http://bhaptics.com/app.html)

## How to install
* Download package file then import it in Unity
    * https://github.com/bhaptics/tactosy-sharp/releases
  ### or
* Get from github repository then open it in Unity

    ```
    git clone https://github.com/bhaptics/tactosy-sharp.git
    ```

## How to use
* Test Default scene 

    >Go to Assets > Tactosy > Example > open Default.scene
    Select [Tactosy] Prefab.<br/>
    There are 2 feedbacks, which are already set for test.
    You can check it by pushing the buttons.

* Import namespaces
```
using System.IO;
using Tactosy.Common;
using Tactosy.Common.Sender;
```
    
* Initialize TactosyPlayer in C# script
    ```
     public TactosyPlayer TactosyPlayer;
     private ISender sender;
     private ITimer timer;

     sender = new WebSocketSender();
     timer = GetComponent<UnityTimer>();
     TactosyPlayer = new TactosyPlayer(sender, timer);
    ```
    
* Apply more feedbacks: with .tactosy file
    
    >You can create Tactosy feebacks via https://studio.bhaptics.com
    .tactosy file is timeline based haptic feedback file.
    For more detail, you can find in http://bhaptics.com/studio.html

* Apply more feedbacks: in C# script
	> There are two ways to create haptic feedbacks in C# script.
	> One uses a List of Point object,
	> the other uses an byte array composed of 20 numbers(0-100).

* Play feedbacks in C# Script: List of Point
	```
    List<Point> pathPoints = new List<Point>
    {
    	new Point(x_position, y_position, intensity)
        /* x_position, y_position are floats in
        	normalized value (0.0f to 1.0f) */
    };
    TactosyPlayer.SendSignal("Point", PositionType.Right, pathPoints, duration);
    /* duration is a positive integer in milliseconds */
    ```
* Play feedbacks in C# Script: Array of byte
	```
    byte[] bytes =
                {
                    0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0,
                    0, 0, 100, 100, 0,
                    0, 0, 0, 0, 0
                }; /* Values should be an int (0~100)
      /* Each number is the intensity of the point*/
     TactosyPlayer.SendSignal("Bytes", bytes);

    ```

* Play registered .tactosy feedbacks with key string
    ```
    /* Play from the specific time */
    TactosyPlayer.SendSignal("ArrowRelease", .2f);
    /* Just play all feedback of .tactosy file */
    TactosyPlayer.SendSignal("Fireball");
    ```

* TurnOff Signal
    ```
    /* Turn off all Tactosy feedbacks */
    TactosyPlayer.TurnOff();
    /* Turn off the Tactosy feedback with the Key string */
    TactosyPlayer.TurnOff("Fireball");
    ```

* Check whether Playing signal or not
    ```
    /* Return the bool whether Fireball is playing */
    bool isFireballFeedbackPlaying = TactosyPlayer.IsPlaying("Fireball");
    /* Return the bool whether any feedback is playing */
    bool isAnyFeedbackPlaying = TactosyPlayer.IsPlaying();
    ```
## Sample Application
There are some demos which contain Tactosy feedbacks in Unity based app.
* Tactosy feedback to archery game :
	* https://github.com/codeblv/Bhaptics_Longbow_Archery

<br>
Copyright 2017 bHaptics Inc.
