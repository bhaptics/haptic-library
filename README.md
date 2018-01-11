# bHaptics haptic devices C# plugin
This project helps to utilize haptic devices in Unity and other C# based platform.
Current version is 1.2.2

## Prerequisite
* bHaptics Player has to be installed (Windows)
   * The app can be found in 
   bHaptics webpage: [http://www.bhaptics.com](http://bhaptics.com/)

## How to use codes
* You can download library (Bhaptics.Tac) with nuget manager [Bhaptics.Tac](https://www.nuget.org/packages/Bhaptics.Tac/)
* To install Bhaptics.Tac, run the following command in the Package Manager Console
```
PM> Install-Package Bhaptics.Tac
```
#### or 
    
* Clone the git then apply dll in src folder
```
$ git clone https://github.com/bhaptics/tac-sharp.git
```

## Sample (Unity plug-in)
* Sample source codes for Unity is already available now. 
* For more detail, you can find in [unity-plugin](https://github.com/bhaptics/tactosy-sharp/tree/master/samples/tac-sharp-unity)


## Websocket Communication V2

* request url : /v2/feedbacks

### How to use
* Check out ProjectTest.cs in Bhaptics.Tac.Tests project
```C#
var hapticPlayer = new HapticPlayer((connected) =>
{
    Debug.WriteLine("Connected");
});
hapticPlayer.Register(key, "BowShoot.tact");

hapticPlayer.StatusReceived += feedback =>
{
    if (feedback.ActiveKeys.Count <= 0)
    {
        return;
    }
};

Thread.Sleep(100);
hapticPlayer.SubmitRegistered(key);
Thread.Sleep(1000);
hapticPlayer.Dispose();
```
 
### Request
```json
{
    "Register" : [{
        "Key" : "feedback1",
        "Project" : {
                "tracks" : [...],
                "layout" : {...}
		    }
        },{
        "Key" : "feedback2",
        "Project" : {
            "tracks" : [...],
            "layout" : {...}
		}
	}],
	"Submit" : [{
            "Type" : "frame",
            "Key" : "newFeedback",
            "Frame" : {}
        }, {
            "Type" : "key",
            "Key" : "newFeedback"
            "Parameters" : {
                "durationRatio" : 0.5,
                "intensityRatio" : 0.5
            }
        }, {
            "Type" : "turnOff",
            "Key" : "newFeedback2"
        }, {
            "Type" : "turnOffAll"
        },
    ]
}

```

* Response when there is change
```json
{
    "RegisteredKeys" : ["a", "c"],
    "ActiveKeys" : ["a", "c"],
    "ConnectedDeviceCount" : 3,
    "Status" : [
        "Left" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
        "Right" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
        "VestFront" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
        "VestBack" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
        "Head" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
        "Racket" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0]
    ]
}
```

## Contact
* Official Website: http://www.bhaptics.com/
* E-mail: developer@bhaptics.com
* Issues : https://github.com/bhaptics/tac-sharp/issues/new

Last update of README.md: Dec 13th, 2017.

###### Copyright (c) 2017 bHaptics Inc. All rights reserved.
