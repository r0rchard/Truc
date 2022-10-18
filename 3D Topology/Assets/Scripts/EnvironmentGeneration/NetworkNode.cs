using Interaction;
using Parsing.CyberRangeSerialization;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace EnvironmentGeneration
{
    /// <summary>
    /// A generic class describing a network
    /// </summary>
    public class NetworkNode : AbstractNode
    {
        /// <summary>
        /// The network asset from the JSON file associated with this node
        /// </summary>
        NetworksSpecsAsset _networkAsset;
        public NetworksSpecsAsset NetworkAsset
        {
            get => _networkAsset;
            set => _networkAsset = value;
        }

        /// <summary>
        /// The IP address of this network
        /// </summary>
        IPAddress _ipAddress;
        public IPAddress IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        /// <summary>
        /// The name of this network
        /// </summary>
        string _networkName;
        public string NetworkName
        {
            get => _networkName;
            set => _networkName = value;
        }

        string _identifier;
        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        /// <summary>
        /// The first door leading out of this network
        /// </summary>
        public NetworkDoor FrontDoor;
        /// <summary>
        /// The second door leading out of this network
        /// </summary>
        public NetworkDoor BackDoor;

        /// <summary>
        /// The list of host connected to this network
        /// </summary>
        Dictionary<string, HostNode> _connected = new Dictionary<string, HostNode>();
        public Dictionary<string, HostNode> Connected
        {
            get => _connected;
        }

        private NetworkNode _linked;
        public NetworkNode Linked
        {
            get => _linked;
            set => _linked = value;
        }

        /// <summary>
        /// Adds a host to the list of connected hosts
        /// </summary>
        /// <param name="connected">The host to add</param>
        public void AddConnected(HostNode connected, string identifier)
        {
            _connected.Add(identifier, connected);
        }
    }
}