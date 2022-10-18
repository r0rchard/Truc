using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    public class MapInteractable : Interactable
    {
        Transform _teleportSpot;
        public Transform TeleportSpot
        {
            get => _teleportSpot;
            set => _teleportSpot = value;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            FindObjectOfType<XRRig>().transform.position = _teleportSpot.position;
        }
    }
}