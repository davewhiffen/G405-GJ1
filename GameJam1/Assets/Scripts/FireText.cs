using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireText : MonoBehaviour
{
    public static int score;
    Text text1;

    void Awake()
    {
        text1 = GetComponent<Text>();
        score = 0;
    }
    private void Update()
    {
        text1.text = "Safety Check Fire Extinguisher: " + score;
    }
}
