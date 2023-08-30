using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Analytic
{
    public class AnalyticConst
    {
        //public const string Event = "event";
        //public const string ACTION_PROPERTIES = "action_properties";

        public static object NewScreenEvent(string screenName)
        {
            return new { @event = screenName }; 
        }

        public static object NewTrackGameOver(int score)
        {
            return new
            {
                @event = "score",
                action_properties = new
                {
                    score = score
                }
            };
        }

    }
}
