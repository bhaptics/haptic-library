# Unity plugin
This project helps to use bhaptics haptic devices in Unity environments. 

## Prerequisite
* bHpatics Player has to be installed (Windows or Mac)
   * The app can be found in
   bHaptics webpage: [http://www.bhaptics.com](http://bhaptics.com/app.html)

## How to install
* Download package file then import it in Unity
    * https://github.com/bhaptics/tac-sharp/releases
  ### or
* Get from github repository then open it in Unity

    ```
    git clone https://github.com/bhaptics/tac-sharp.git
    ```

## Tutorial
* [bHaptics Unity plugin With Code](https://youtu.be/zHoJANhfwpk)	
* [bHaptics Designer To Unity - Tactosy](https://youtu.be/eateHpUKC4s)
* [bHaptics Designer To Unity - Tactal](https://youtu.be/sj7IqgFn_iw)
* [bHaptics Designer To Unity - Tactot](https://youtu.be/MvhrSCwS2Wg)
	
## How to use
* Test Default scene 

    >Go to Assets > bHapticsManager > Examples > open sample.scene
    Select [bHaptics Manager] Prefab.<br/>
    There are feedbacks, which are already set for test.
    You can check it by pushing the buttons

* To use your own scene, just add [bHaptics Manager] Prefab to your scene.

* Import namespaces
    ```
    using Bhaptics.Tac;
    using Bhaptics.Tac.Unity;
    ```
    
* Get HapticPlayer
    ```
    private HapticPlayer HapticPlayer;
	 
    void Start ()
    {
        HapticPlayer = FindObjectOfType<BhapticsManager>().HapticPlayer;
    }
	 
    ```
    
* Apply more feedbacks: with .tactosy file
    
    >You can create Tactosy feebacks via https://studio.bhaptics.com
    .tactosy file is timeline based haptic feedback file.
    For more detail, you can find in http://bhaptics.com/studio.html


* Play feedbacks in C# Script: List of PathPoint
	```
    List<PathPoint> pathPoints = new List<PathPoint>
    {
    	new PathPoint(x_position, y_position, intensity)
        /* x_position, y_position are floats in
        	normalized value (0.0f to 1.0f) */
    };
    HapticPlayer.Submit("Point", PositionType.Right, pathPoints, duration);
    /* duration is a positive integer in milliseconds */
    ```
	
	
* Play feedback with DotPoint
    ```
    HapticPlayer.Submit("space", PositionType.Head, new DotPoint(3, 100), 1000);
    ```


* Play feedbacks in C# Script: Array of byte
	```
    byte[] bytes =
    {
        0, 0, 0, 0, 0,
        0, 0, 0, 0, 0,
        0, 0, 100, 100, 0,
        0, 0, 0, 0, 0
	}; 
    /* Values should be an int (0~100)
    /* Each number is the intensity of the point*/
    HapticPlayer.Submit("Bytes", PositionType.Right, bytes);
    ```

* Play registered .tactosy feedbacks with file name
   * plugin automatically register tactosy files in BhapticsManager's pathPrefix with their file name
    ```
    /* Play from the specific time */
    HapticPlayer.SubmitRegistered("ArrowRelease", .2f);
    /* Just play all feedback of .tactosy file */
    HapticPlayer.SubmitRegistered("Fireball");
    ```

* TurnOff Signal
    ```
    /* Turn off all Haptic feedbacks */
    HapticPlayer.TurnOff();
    /* Turn off the Haptic feedback with the Key string */
    HapticPlayer.TurnOff("Fireball");
    ```

* Check whether Playing signal or not
    ```
    /* Return the bool whether Fireball is playing */
    bool isFireballFeedbackPlaying = HapticPlayer.IsPlaying("Fireball");
    /* Return the bool whether any feedback is playing */
    bool isAnyFeedbackPlaying = HapticPlayer.IsPlaying();
    ```
## Sample Application
There are some demos which contain Haptic feedbacks in Unity based app.
* Tactosy feedback to archery game :
	* https://github.com/codeblv/Bhaptics_Longbow_Archery

<br>
Copyright 2017 bHaptics Inc.
