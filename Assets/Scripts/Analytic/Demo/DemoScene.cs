using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Analytic.Demo
{
    public class DemoScene : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private GameObject gameOver;
        private int score;

        public int Score
        {
            set
            {
                score = value;
                scoreText.text = "Score: " + value.ToString();
            }
            get
            {
                return score;
            }
        }


        private IEnumerator Start()
        {
            Score = 0;

            string apiKey = "a66b0798-e3f4-4a7d-8f75-77f905000c02"; //YOUR API KEY, GET IT ON: https://developers.skymavis.com/console/app-tracking/
            AnalyticManager.InitManager(apiKey);
            yield return new WaitForEndOfFrame();

            string message = "";
            string signature = "";
            UserVertification.GetArgVertification(out signature, out message);
            var userData = UserVertification.Vertification(signature, message);
            if (userData != null)
            {
                AnalyticManager.IdentifyUserData(userData); //INIT IDENTIFY FROM USER DATA (MavisHub)
            }
            else
            {
                AnalyticManager.IdentifyLocalUser(); //INIT IDENTIFY LOCAL USER  
            }

            yield return new WaitForEndOfFrame();



            AnalyticManager.AddEvent(EventTypes.Screen, new { @event = "demo_screen" });
        }


        public void GameOver()
        {
            gameOver.SetActive(true);

            //TRACKING
            var jObject = new JObject();
            jObject.Add(AnalyticConst.Event, "game_over");
            var properties = new JObject();
            properties.Add("score", score);
            jObject.Add(AnalyticConst.ACTION_PROPERTIES, properties);
            AnalyticManager.AddEvent(EventTypes.Track, jObject);
        }
    }
}


