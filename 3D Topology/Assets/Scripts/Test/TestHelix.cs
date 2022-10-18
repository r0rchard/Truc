using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHelix : MonoBehaviour
{
    [SerializeField] baseHelice helix;
    [SerializeField] GameObject container;

    // Start is called before the first frame update
    void Start()
    {
        DummySensor sensor = new DummySensor();
        helix.sensor = sensor;
        container.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
