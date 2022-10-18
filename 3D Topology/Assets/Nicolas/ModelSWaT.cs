using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModelSWaT
{
    [Serializable]
    public class Sensor
    {
        public string Name;
        public float[] Values;

        public Sensor(string name, float[] values)
        {
            Name = name;
            Values = values;
        }
    }

    [Serializable]
    public class AlertsTimelines
    {
        public DateTime Start;
        public bool[] Alerts;
    }
    [Serializable]
    public class ListSensors
    {
        public List<Sensor> Sensors;
    }
    
}
