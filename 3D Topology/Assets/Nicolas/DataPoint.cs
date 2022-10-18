using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint : MonoBehaviour
{
    float value;

    public void SetValue(float f)
    {
        value = f;
    }

    public float GetValue()
    {
        return value;
    }
}
