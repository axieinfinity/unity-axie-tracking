# unity-axie-tracking
Tracking SDK for Unity Editor

DemoScene.cs

```csharp
private IEnumerator Start()
{
    Score = 0;

    string apiKey = "a66b0798-e3f4-4a7d-8f75-77f905000c02"; //YOUR API KEY, GET IT ON: https://developers.skymavis.com/console/app-tracking/
    AnalyticManager.InitManager(apiKey);
    yield return new WaitForEndOfFrame();

    AnalyticManager.IdentifyLocalUser(); //INIT IDENTIFY LOCAL USER
    yield return new WaitForEndOfFrame();

    AnalyticManager.AddEvent(EventTypes.Screen, new { @event = "demo_screen" }); //LOG EVENT OPEN SCREEN     
}


public void GameOver()
{
    gameOver.SetActive(true);

    //TRACKING EVENT GAME OVER
    var jObject = new JObject();
    jObject.Add(AnalyticConst.Event, "game_over");
    var properties = new JObject();
    properties.Add("score", score);
    jObject.Add(AnalyticConst.ACTION_PROPERTIES, properties);
    AnalyticManager.AddEvent(EventTypes.Track, jObject);
}
```

DemoScene Preview
![Untitled](https://github.com/axieinfinity/unity-axie-tracking/assets/128013742/2d3929f2-672b-4c04-b3c2-3a490fe61867)
![Untitled (1)](https://github.com/axieinfinity/unity-axie-tracking/assets/128013742/d4f48f58-3fb1-4ea7-b949-5aba5cf48046)

