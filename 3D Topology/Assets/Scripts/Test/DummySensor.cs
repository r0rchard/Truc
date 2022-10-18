using ModelSWaT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySensor : Sensor
{
    public DummySensor() : base("name", null)
    {
        Values = new float[33000];
        for(int i = 0; i< 33000; i++)
        {
            Values[i] = Random.Range(0f, 1f);
        }
    }
}
