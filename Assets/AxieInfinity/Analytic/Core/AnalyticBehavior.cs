using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Analytic
{
    public class AnalyticBehavior : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
