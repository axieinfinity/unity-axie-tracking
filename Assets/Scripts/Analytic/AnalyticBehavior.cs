using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticBehavior : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
