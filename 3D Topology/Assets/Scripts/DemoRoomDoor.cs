using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DemoRoomDoor : Interactable
{
    Vector3 _teleportPoint;
    public Vector3 TeleportPoint
    {
        get => _teleportPoint;
        set => _teleportPoint = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate(Vector3 activationLocation, Vector3 normal)
    {
        base.Activate(activationLocation, normal);
        FindObjectOfType<XRRig>().transform.position = _teleportPoint;
    }
}
