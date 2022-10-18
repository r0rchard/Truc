using EnvironmentGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    /// <summary>
    /// An interactable object allowing to teleport out of a network
    /// </summary>
    public class NetworkDoor : Interactable
    {
        //the text appearing on the inside of the door
        [SerializeField] TMPro.TextMeshPro _exitDescription;
        [SerializeField] Transform _userSpawn;
        public Transform UserSpawn
        {
            get => _userSpawn;
        }
        [SerializeField] Transform _assistantSpawn;
        public Transform AssistantSpawn
        {
            get => _assistantSpawn;
        }
        [SerializeField] Transform _otherSide;
        public Transform OtherSide
        {
            get => _otherSide;
        }

        public string ExitDescription
        {
            set => _exitDescription.text = value;
        }
        //the text appearing on the outside of the door
        [SerializeField] TMPro.TextMeshPro _entryDescription;
        public string EntryDescription
        {
            set => _entryDescription.text = value;
        }

        //the destination of the door
        private FireWallExit _fireWallExit;
        public FireWallExit FireWallExit
        {
            get => _fireWallExit;
            set => _fireWallExit = value;
        }

        private NetworkNode _network;
        public NetworkNode Network
        {
            get => _network;
            set => _network = value;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            //teleporting the XR rig
            XRRig rig = FindObjectOfType<XRRig>();
            Assistant assistant = FindObjectOfType<Assistant>();
            rig.transform.position = _fireWallExit.UserSpawn.position;
            rig.transform.forward = _fireWallExit.UserSpawn.forward;

            assistant.Agent.ResetPath();
            assistant.Agent.Warp(_fireWallExit.AssistantSpawn.position);
            assistant.CurrentRoom = _fireWallExit.FireWall;
            assistant.RoomChanged = true;
        }
    }
}