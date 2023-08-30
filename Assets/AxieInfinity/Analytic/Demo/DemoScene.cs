using Newtonsoft.Json.Linq;
using UnityEngine;
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

        void Start()
        {
            Score = 0;

            string apiKey = "a66b0798-e3f4-4a7d-8f75-77f905000c02"; //YOUR API KEY, GET IT ON: https://developers.skymavis.com/console/app-tracking/
            AnalyticManager.InitManager(apiKey);
            AnalyticManager.IdentifyLocalUser(); //INIT IDENTIFY LOCAL USER
        }

        public void GameOver()
        {
            gameOver.SetActive(true);

            //TRACKING
            AnalyticManager.AddEvent(EventTypes.Screen, AnalyticConst.NewScreenEvent("game_over"));
            AnalyticManager.AddEvent(EventTypes.Track, AnalyticConst.NewTrackGameOver(score));
        }
    }
}
