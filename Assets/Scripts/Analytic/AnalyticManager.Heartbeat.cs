using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Analytic
{
    public partial class AnalyticManager
    {
        static void OnCheckInput()
        {
            bool isPressed = false;
#if UNITY_EDITOR || UNITY_STANDALONE
            isPressed = Input.anyKeyDown;
#else
            int nbTouches = UnityEngine.Input.touchCount;
            if (nbTouches > 0) {
                isPressed = UnityEngine.Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began;
            }
#endif
            if (isPressed)
            {
                heartbeatStats.OnNewPressed();
            }
        }
    }
}
