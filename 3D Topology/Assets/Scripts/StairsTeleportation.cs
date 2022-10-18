using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StairsTeleportation : Interactable
{
    public Transform UserSpawn;
    public Transform AISpawn;
    public StairsTeleportation Destination;

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate(Vector3 activationLocation, Vector3 normal)
    {
        base.Activate(activationLocation, normal);
        FindObjectOfType<XRRig>().transform.position = Destination.UserSpawn.position;
        FindObjectOfType<Assistant>().Agent.Warp(Destination.AISpawn.position);
    }
}
