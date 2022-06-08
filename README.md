# AdofaiWeb

Adofai Websocket

A simple ADOFAI mod which creates a websocket that you can listen to

**WARNING: THIS MOD DOES NOTHING ON IT'S OWN**

### Instalation

I've created a few example layouts in the [examples](https://github.com/thijnmens/AdofaiWeb/tree/master/Examples) folder, you can choose one from here or simply make your own with the instructions down below.

### Websocket

The websocket is hosted on http://127.0.0.1:420/server and sends data in 1 of the following formats

#### Default

-   When?
    -   when in any other scene than the scnEditor scene
-   Frequenty
    -   Every 10 frames

```json
{
	"type": "default"
}
```

#### LevelImage

-   When?
    -   When loading custom level
-   Frequenty
    -   once

```json
{
	"type": "levelImage",
	"data": {
		"previewImage": string,
		"previewImageExtension": string
	}
}
```

#### Update

-   When?
    -   When in custom level
-   Frequenty
    -   Every 10 frames

```json
{
	"type": "update",
	"data": {
        "paused": bool,
        "checkpoints": int,
        "deaths": int,
        "attempts": int,
        "speed": double,
        "percentComplete": float,
        "tooEarly": int,
        "veryEarly": int,
        "earlyPerfect": int,
        "perfect": int,
        "latePerfect": int,
        "veryLate": int,
        "tooLate": int,
    }
}
```

#### LoadLevel

-   When?
    -   When loading custom level
-   Frequenty
    -   once

```json
{
	"type": "loadLevel",
	"data": {
		"calibration_i": float,
        "calibration_v": float,
        "beatNumber": int,
        "angleData": float[],
        "artist": string,
        "artistLinks": string,
        "artistPermission": string,
        "author": string,
        "backgroundColor": string,
        "bgFitScreen": bool,
        "bgImage": string,
        "bgImageColor": string,
        "bgLockRot": bool,
        "bgLooping": bool,
        "bgParallax": string,
        "bgShowDefaultBGIfNoImage": bool,
        "bgTiling": bool,
        "bgVideo": string,
        "bpm": float,
        "camPosition": string,
        "camRelativeTo": string,
        "camRotation": float,
        "camZoom": float,
        "countdownTicks": int,
        "difficulty": int,
        "floorIconOutlines": bool,
        "fullCaption": string,
        "fullCaptionTagged": string,
        "Hash": string,
        "hitsound": string,
        "hitsoundVolume": int,
        "isOldLevel": bool,
        "legacyFlash": bool,
        "levelDesc": string,
        "levelTags": string[],
        "offset": int,
        "oldCameraFollowStyle": bool,
        "pathData": string,
        "pitch": int,
        "planetEase": string,
        "planetEaseParts": int,
        "previewIcon": string,
        "previewIconColor": string,
        "previewSongDuration": int,
        "previewSongStart": int,
        "requiredDLC": string,
        "secondaryTrackColor": string,
        "seizureWarning": bool,
        "separateCountdownTime": bool,
        "song": string,
        "songFilename": string,
        "specialArtistType": string,
        "stickToFloors": bool,
        "trackAnimation": string,
        "trackBeatsAhead": float,
        "trackBeatsBehind": float,
        "trackColor": string,
        "trackColorAnimDuration": float,
        "trackColorPulse": string,
        "trackColorType": string,
        "trackDisappearAnimation": string,
        "trackPulseLength": int,
        "trackStyle": string,
        "unscaledSize": float,
        "version": int,
        "volume": int
	}
}
```

### Developing

Want to help develop this project? Clone the project, install all the packages and fix any refrence errors and you should be good to go.
