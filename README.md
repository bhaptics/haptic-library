# bHaptics haptic devices C# plugin
This project helps to utilize haptic devices in Unity and other C# based platform.
Current version is 1.0.0

## Prerequisite
* bHaptics Player has to be installed (Windows or Mac)
   * The app can be found in
   bHaptics webpage: [http://www.bhaptics.com](http://bhaptics.com/app.html)

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

## Contact
* Official Website: http://www.bhaptics.com/
* E-mail: contact@bhaptics.com

Last update of README.md: Jun 15th, 2017.


## Websocket Communication V2
* request url : /v2/feedbacks
// Register Remove는 분리 필요하다.. 
### Request
```json
{
    "register" : [{
        "key" : "feedback1",
        "project" : {
                "tracks" : [...],
                "layout" : {...}
		    }
        },{
        "key" : "feedback2",
        "project" : {
            "tracks" : [...],
            "layout" : {...}
		}
	}],
	"submit" : [{
            "type" : "frame",
            "key" : "newFeedback",
            "frame" : {}
        }, {
            "type" : "key",
            "key" : "newFeedback"
        }, {
            "type" : "turnOff",
            "key" : "newFeedback2"
        }, {
            "type" : "turnOffAll"
        },
    ]
}

```

* Response 
```json
{
    "status" : "success",
    "data" : {
        "registered" : ["a", "c"],
        "active" : ["a", "c"],
        "connectedDevices" : 3,
        "connectedPositions" : ["Left", "Right", "VestFront", "VestBack", "Head", "Racket"],
        "activeStatus" : [
            "Left" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
            "Right" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
            "VestFront" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
            "VestBack" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
            "Head" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0],
            "Racket" : [0, 0, 0, 0, 0, 0, 0, 0, 100, 0, ..., 0]
        ]
    }
}
```

```json
{
    "status" : "error",
	"data" : [{
		"type" : "register",
        "source" : "type"
        }
	]
}
```



###### Copyright (c) 2017 bHaptics Inc. All rights reserved.
