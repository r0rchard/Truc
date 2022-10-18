using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyRay : DummyTool
{
    LineRenderer _line;

    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.SetPosition(0, transform.position);
        _line.SetPosition(0, transform.position + transform.forward * 20);
        _line.startWidth = 0.05f;
        _line.endWidth = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        _line.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
        {
            _line.SetPosition(1, hit.point);
        }
        else
        {
            _line.SetPosition(1, transform.position + transform.forward * 20);
        }
    }
}
