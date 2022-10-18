using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Interaction
{
    /// <summary>
    /// Class used to identify the floor for teleportation
    /// </summary>
    [RequireComponent(typeof(NavMeshSurface))]
    public class FloorInteraction : Interactable
    {
        Assistant _assistant;
        public Assistant Assistant
        {
            get => _assistant;
            set => _assistant = value;
        }

        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            if (_assistant == null)
            {
                _assistant = FindObjectOfType<Assistant>();
            }
            if (!_assistant.IsGuiding)
            {
                _assistant.Agent.SetDestination(activationLocation);
            }
        }
    }
}