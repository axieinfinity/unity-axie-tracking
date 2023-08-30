# unity-axie-tracking
Tracking SDK for Unity Editor

## Installation
Download and setup `unity-axie-tracking` package at this **[LINK](https://github.com/axieinfinity/unity-axie-tracking/releases/)**
**Note**: We will use the `Import Asset Package` option in the Unity Engine, follow the steps described in the [official documentation](https://docs.unity3d.com/560/Documentation/Manual/AssetPackages.html).

## Quick Implementation
#### 1. Create API_KEY
- Create new App on [this](https://developers.skymavis.com/console/applications/)
- Fill info, app logo in Information section
- Request `App Tracking` on `App Permission` then wait for approve.
- After that you can generate new `API KEY` from App Tracking/Setting

#### 2. Initializing Client & Identify
With this you can see `Overview` data like DAU, Retention
![DAU](images/dau.png?raw=true "DAU")
```csharp
AnalyticManager.InitManager(YOUR_API_KEY);
AnalyticManager.IdentifyLocalUser(); //INIT IDENTIFY LOCAL USER
// You can custom user data by use AnalyticManager.IdentifyCustomUser
```
#### 3. Screen
You can you this for see: how players experience your game, where they stuck,...
![Screen](images/screen.png?raw=true "Screen")
```csharp
AnalyticManager.AddEvent(EventTypes.Screen, AnalyticConst.NewScreenEvent("game_over"));
```

### 4. Other Track
Use this for advanced data that depends on each project.

![Track](images/track.png?raw=true "Track")
```csharp
AnalyticManager.AddEvent(EventTypes.Track, AnalyticConst.NewTrackGameOver(score));
```
