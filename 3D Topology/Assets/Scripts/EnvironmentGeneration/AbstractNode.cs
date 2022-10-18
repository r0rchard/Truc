using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentGeneration
{
    /// <summary>
    /// Abstract class encapsulating both networks and nodes
    /// </summary>
    public abstract class AbstractNode : MonoBehaviour
    {
        public float Size = 0;

        //used for A*
        public float Cost = 0;
        public float HeursisticCost = 0;
        public AbstractNode PreviousInPath;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}