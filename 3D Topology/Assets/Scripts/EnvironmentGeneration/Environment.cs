using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvironmentGeneration
{
    /// <summary>
    /// Class representing the explorable environment
    /// </summary>
    public class Environment : MonoBehaviour
    {
        public List<HostNode> hosts;
        public List<NetworkNode> networks;
        private float _tileSize;
        public float TileSize
        {
            get => _tileSize;
            set => _tileSize = value;
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public List<AbstractNode> AStarSearch(AbstractNode start, AbstractNode destination)
        {
            foreach(HostNode host in hosts)
            {
                host.PreviousInPath = null;
                host.Cost = Mathf.Infinity;
                host.HeursisticCost = Mathf.Infinity;
            }
            foreach (NetworkNode network in networks)
            {
                network.PreviousInPath = null;
                network.Cost = Mathf.Infinity;
                network.HeursisticCost = Mathf.Infinity;
            }

            List<AbstractNode> closedList = new List<AbstractNode>();
            List<AbstractNode> openList = new List<AbstractNode>();
            start.Cost = 0;
            start.HeursisticCost = (start.transform.position - destination.transform.position).magnitude;
            start.PreviousInPath = null;

            openList.Add(start);
            while (openList.Count > 0)
            {
                float minCost = Mathf.Infinity;
                AbstractNode nextNode = openList[0];
                if (openList.Count > 1)
                {
                    foreach (AbstractNode node in openList)
                    {
                        if (node.Cost + node.HeursisticCost < minCost)
                        {
                            minCost = node.Cost + node.HeursisticCost;
                            nextNode = node;
                        }
                    }
                }
                openList.Remove(nextNode);
                if(nextNode == destination)
                {
                    return ReconstructPath(nextNode);
                }
                else if(nextNode is NetworkNode)
                {
                    foreach(HostNode host in ((NetworkNode)nextNode).Connected.Values)
                    {
                        if(host is FireWall)
                        {
                            if (!closedList.Contains(host) && nextNode.Cost + 1 < host.Cost)
                            {
                                host.Cost = nextNode.Cost + 1;
                                host.HeursisticCost = (host.transform.position - destination.transform.position).magnitude / _tileSize;
                                host.PreviousInPath = nextNode;
                                if (!openList.Contains(host))
                                {
                                    openList.Add(host);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (NetworkNode network in ((HostNode)nextNode).Connections.Values)
                    {
                        if (!closedList.Contains(network) && nextNode.Cost + 1 < network.Cost)
                        {
                            network.Cost = nextNode.Cost + 1;
                            network.HeursisticCost = (network.transform.position - destination.transform.position).magnitude / _tileSize;
                            network.PreviousInPath = nextNode;
                            if (!openList.Contains(network))
                            {
                                openList.Add(network);
                            }
                        }
                    }
                }
                closedList.Add(nextNode);
            }
            return null;
        }

        List<AbstractNode> ReconstructPath(AbstractNode end)
        {
            List<AbstractNode> path = new List<AbstractNode>();
            path.Add(end);
            AbstractNode current = end;
            while (current.PreviousInPath)
            {
                path.Insert(0, current.PreviousInPath);
                current = current.PreviousInPath;
            }
            return path;
        }
    }
}