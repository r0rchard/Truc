using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace EnvironmentGeneration
{
    /// <summary>
    /// An indicator that signals the position of the user on the map
    /// </summary>
    public class YouAreHereIndicator : MonoBehaviour
    {
        private XRRig _rig;
        public XRRig Rig
        {
            get => _rig;
            set => _rig = value;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.localPosition = _rig.transform.localPosition;
            transform.up = transform.parent.up;
            Vector3 cameraDirection = Camera.main.transform.position - transform.position;
            //we want the indicator to face the user while still being perpendicular to the map
            float angle = Mathf.Atan2(transform.forward.z, transform.forward.x) - Mathf.Atan2(cameraDirection.z, cameraDirection.x) * Mathf.Rad2Deg + 90;
            transform.Rotate(0, angle, 0);
        }
    }
}