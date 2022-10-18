using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModelSWaT;
public class HelixSensor : MonoBehaviour
{
    public Sensor sensorHelix;

    public Mesh meshHelix;

    public float period;
    public float radius;
    public float high;
    public float numberOfElement;

    [Range(0.0f, 10000.0f)]
    public float timeDisplayed;

    [Range(0.0f, 10000.0f)]
    public float startTimeDisplayed;

    public void SetSensor(Sensor s)
    {
        sensorHelix = s;
    }

    public void SetHelix(float p, float r, float h, float n)
    {
        period = p;
        radius = r;
        high = h;
        numberOfElement = n;
    }

    public void SetTime( float start, float displayed)
    {
        startTimeDisplayed = start;
        timeDisplayed = displayed;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
