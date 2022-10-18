using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModelSWaT;
using UnityEngine.UI;

public class baseHelice : MonoBehaviour
{
    [HideInInspector]
    public Sensor sensor;
    public Text nameSensor;
    public Canvas canva;

    private void OnEnable()
    {
        StartCoroutine(WaitForSensor());    
    }

    IEnumerator WaitForSensor()
    {
        yield return new WaitUntil(() => sensor != null);
        GetComponentInChildren<HelicoShaderCubes>().sensorValues = sensor.Values;
        GetComponentInChildren<HelicoShaderCubes>().name = sensor.Name;

        nameSensor.text = "Connections to foreign entities";
        Camera c = Camera.main;
        canva.worldCamera = c;
    }
}
