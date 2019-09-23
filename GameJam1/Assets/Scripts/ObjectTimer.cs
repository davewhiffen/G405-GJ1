using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimer : MonoBehaviour
{

    public const int maxValue = 100;
    public int currentValue = maxValue;

    public void increase(int amnt)
    {
        currentValue += amnt;
    }
}
