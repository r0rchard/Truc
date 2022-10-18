using Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EnvironmentGeneration
{
    public class FireWall : HostNode
    {
        public List<FireWallExit> _exits = new List<FireWallExit>();
        public List<FireWallExit> Exits
        {
            get => _exits;
            set => _exits = value;
        }
    }
}