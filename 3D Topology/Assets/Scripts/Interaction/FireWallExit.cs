using EnvironmentGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    /// <summary>
    /// Interactable class allowing to leave leave a firewall and teleport somewhere else
    /// </summary>
    public class FireWallExit : Interactable
    {
        /// <summary>
        /// The network to teleport to when leaving
        /// </summary>
        NetworkNode _network;
        public NetworkNode Network
        {
            get => _network;
            set => _network = value;
        }

        FireWall _fireWall;
        public FireWall FireWall
        {
            get => _fireWall;
            set=>_fireWall=value;
        }

        /// <summary>
        /// The text indicating the destination of the exit
        /// </summary>
        [SerializeField] TMPro.TextMeshPro _description;
        public string Description
        {
            set => _description.text = value;
        }

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

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Teleports the player to the network
        /// </summary>
        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            XRRig rig = FindObjectOfType<XRRig>();
            Assistant assistant = FindObjectOfType<Assistant>();
            NetworkDoor door;
            if (this == _network.FrontDoor.FireWallExit)
            {
                door = _network.FrontDoor;
            }
            else
            {
                door = _network.BackDoor;
            }

            //teleportation a little bit in front of the door
            rig.transform.position = new Vector3(door.UserSpawn.position.x, rig.transform.position.y, door.UserSpawn.position.z) + door.UserSpawn.transform.right;
            rig.transform.forward = door.UserSpawn.transform.forward;

            assistant.Agent.ResetPath();
            assistant.Agent.Warp(door.AssistantSpawn.position);
            assistant.CurrentRoom = door.Network;
            assistant.RoomChanged = true;
        }
    }
}