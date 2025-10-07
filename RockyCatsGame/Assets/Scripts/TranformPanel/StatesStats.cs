using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public class StatesStats : MonoBehaviour
{
    public Dictionary<string, int> magmaStats = new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 5},
        {"pressure", 2},
        {"time", 1}

    };

    public Dictionary<string, int> IgneaStats = new Dictionary<string, int>()
    {
        {"water", 1},
        {"temperature", 4},
        {"pressure", 4},
        {"time", 2}

    };

    public Dictionary<string, int> SedimentaryStats = new Dictionary<string, int>()
    {
        {"water", 4},
        {"temperature", 2},
        {"pressure", 5},
        {"time", 4}

    };
    



}