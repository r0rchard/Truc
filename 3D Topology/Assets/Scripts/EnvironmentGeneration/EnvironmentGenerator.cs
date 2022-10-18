using DataTransmission;
using Interaction;
using Networking;
using Parsing.CyberRangeSerialization;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Unity.AI.Navigation;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace EnvironmentGeneration
{
    /// <summary>
    /// Class in charge of placing 3D elements in the environment based on the nodes it is given
    /// </summary>
    public class EnvironmentGenerator : MonoBehaviour
    {
        /// <summary>
        /// The prefab to use for the floor tiles (must be square)
        /// </summary>
        [SerializeField] FloorInteraction _floorTile;
        /// <summary>
        /// The prefab to use for the ceiling (must be square)
        /// </summary>
        [SerializeField] GameObject _ceilingTile;
        /// <summary>
        /// The prefab to use for the wall tiles (must be square)
        /// </summary>
        [SerializeField] GameObject _wallTile;
        /// <summary>
        /// The prefab to use for a wall containing a door (must be square)
        /// </summary>
        [SerializeField] NetworkDoor _doorTile;
        /// <summary>
        /// The prefab of a fire wall exit
        /// </summary>
        [SerializeField] FireWallExit _fireWallTile;
        /// <summary>
        /// The prefab of a ceiling light (decorative)
        /// </summary>
        [SerializeField] GameObject _ceilingLight;
        /// <summary>
        /// The size of one tile's side
        /// </summary>
        [SerializeField] float _tileSize = 3.83128f;
        /// <summary>
        /// The prefab to use for a server
        /// </summary>
        [SerializeField] Server _serverPrefab;
        /// <summary>
        /// The prefab to use for a desktop
        /// </summary>
        [SerializeField] Desktop _pcPrefab;
        /// <summary>
        /// The prefab to use for a firewall
        /// </summary>
        [SerializeField] FireWall _fireWallHubPrefab;
        /// <summary>
        /// The prefab to use for an unknown host
        /// </summary>
        [SerializeField] HostNode _defaultHost;
        [SerializeField] Pipeline _pipeLinePrefab;
        [SerializeField] XRRig _rig;
        [SerializeField] Assistant _assistant;

        //singleton
        public static EnvironmentGenerator Instance;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);

            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Generates the 3D environment
        /// </summary>
        /// <param name="nodes">The nodes to use to generate the environment</param>
        public void GenerateEnvironment(HostsSpecsAsset[] hostAssets, NetworksSpecsAsset[] networkAssets)
        {
            //generate parent object
            Environment environment = new GameObject().AddComponent(typeof(Environment)) as Environment;
            environment.name = "Environment";
            environment.TileSize = _tileSize;

            //generate networks
            List<NetworkNode> networks = new List<NetworkNode>();
            foreach (NetworksSpecsAsset network in networkAssets)
            {
                //creation of abstraction
                NetworkNode networkAbstraction = new GameObject(network.attributes.label_text).AddComponent<NetworkNode>();

                networkAbstraction.NetworkAsset = network;
                networkAbstraction.transform.position = new Vector3(network.attributes.x, 0, -network.attributes.y);
                networkAbstraction.IpAddress = IPAddress.Parse(network.ip);
                networkAbstraction.NetworkName = network.name;
                networkAbstraction.Identifier = network.identifier;
                networkAbstraction.transform.parent = environment.transform;

                networks.Add(networkAbstraction);
            }



            //generate hosts
            List<HostNode> hosts = new List<HostNode>();
            foreach (HostsSpecsAsset host in hostAssets)
            {
                HostNode hostAbstraction;
                //the prefab is chosen depending on the name of the image associated with the node in the JSON file
                if (host.attributes.image_source.ToLower().Contains("server"))
                {
                    hostAbstraction = Instantiate(_serverPrefab);
                }
                else if (host.attributes.image_source.ToLower().Contains("desktop") || host.attributes.image_source.ToLower().Contains("laptop"))
                {
                    hostAbstraction = Instantiate(_pcPrefab);
                }
                else if(host.attributes.image_source.ToLower().Contains("firewall"))
                {
                    hostAbstraction = Instantiate(_fireWallHubPrefab);
                }
                else
                {
                    hostAbstraction = Instantiate(_defaultHost);
                }

                hostAbstraction.HostAsset = host;

                //creation of the interaction
                hostAbstraction.Interaction = hostAbstraction.GetComponent<HostInteraction>();
                hostAbstraction.Interaction.Description = host.attributes.label_text;
                hostAbstraction.transform.position = new Vector3(host.attributes.x, 0, -host.attributes.y);
                hostAbstraction.name = host.attributes.label_text;
                hostAbstraction.Identifier = host.identifier;
                hostAbstraction.transform.parent = environment.transform;

                //association of networks and hosts
                foreach (NicsAsset nics in host.nics)
                {
                    hostAbstraction.IpAdresses.Add(IPAddress.Parse(nics.ip));
                    foreach (NetworkNode net in networks)
                    {
                        if (net.NetworkName == nics.network_name || (net.Identifier != "" && net.Identifier == nics.network_identifier) )
                        {
                            net.AddConnected(hostAbstraction, nics.identifier);
                            hostAbstraction.AddConnection(net, nics.identifier);
                            break;
                        }
                    }
                }

                //generation of a sprite from base 64 svg data
                if (host.icon.data != null && host.icon.data != "")
                {
                    Byte[] data = Convert.FromBase64String(host.icon.data.Split(',')[1]);
                    string svg = Encoding.UTF8.GetString(data);
                    while (svg.Substring(0, 4) != "<svg")
                    {
                        svg = svg.Remove(0, 1);
                    }
                    hostAbstraction.Interaction.SetSprite(CreateSVGSprite(Encoding.UTF8.GetString(data)));
                }

                hosts.Add(hostAbstraction);
            }

            environment.networks = networks;
            environment.hosts = hosts;

            //make the environment fit in a square of a given size
            AdjustDistance(networks, hosts, environment);

            //generate the rooms corresponding to each network
            GenerateRooms(networks, hosts, environment);

            //positioning the camera
            _rig.transform.position = networks[0].FrontDoor.UserSpawn.position;
            _rig.transform.forward = networks[0].FrontDoor.UserSpawn.forward;

            _assistant.gameObject.SetActive(true);
            _assistant.GetComponent<NavMeshAgent>().Warp(networks[0].FrontDoor.AssistantSpawn.position);
            _assistant.transform.forward = networks[0].FrontDoor.AssistantSpawn.forward;

            //generate a minimap of the environment
            MapGenerator.Instance.SetUpMiniMap(environment);

            Assistant assistant = FindObjectOfType<Assistant>();
            assistant.Environment = environment;
            assistant.CurrentRoom = networks[0];

            foreach (HostNode host in hosts)
            {
                if (!(host is FireWall))
                {
                    InternalStatesManager.Instance.States.Add(host.Identifier, host.InternalState);
                }
            }

            AlertManager.Instance.Hosts = hosts;
        }

        /// <summary>
        /// Generates room corresponding to each network and populates them with 3D models corresponding to hosts
        /// </summary>
        /// <param name="networks">The networks to generate room from</param>
        /// <param name="hosts">The hosts connected to the networks</param>
        void GenerateRooms(List<NetworkNode> networks, List<HostNode> hosts, Environment environment)
        {
            foreach (NetworkNode network in networks)
            {
                //list of hosts to appear within the room
                List<AbstractNode> room = new List<AbstractNode>();
                //offset is the position at which the room will be placed
                Vector3 offset = Vector3.zero;
                int necessaryDoors = 0;
                foreach (HostNode host in network.Connected.Values)
                {
                    //if the host is connected to a single network, it will appear in the room
                    if (host.Connections.Count == 1)
                    {
                        room.Add(host);
                        offset += host.transform.position;
                    }
                    //else the host will be its own room and a door is necessary for the network
                    else
                    {
                        necessaryDoors += 1;
                    }
                }
                if (room.Count != 0)
                {
                    offset /= room.Count;
                }
                else
                {
                    offset = network.transform.position;
                }
                {
                    //we position two hosts per tile
                    int necessaryTiles = room.Count % 2 == 0 && room.Count != 0 ? room.Count / 2 : room.Count / 2 + 1;
                    network.transform.position = offset;
                    //generate the walls
                    List<Vector2Int> tiles = PlaceTiles(necessaryTiles, necessaryDoors, network);
                    //place the hosts inside the room
                    PlaceNodes(room, tiles, network.gameObject);
                }
            }

            //only the master instantiates the PipelineManager
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.InstantiateRoomObject("PipelineManager", Vector3.zero, Quaternion.identity);
            }

            foreach (HostNode host in hosts)
            {
                if (host is FireWall)
                {
                    //firewall is positioned between the two networks it's connected to
                    Vector3 barycenter = Vector3.zero;
                    foreach (NetworkNode network in host.Connections.Values)
                    {
                        barycenter += network.transform.position;
                    }
                    barycenter /= host.Connections.Count;
                    host.transform.position = barycenter;
                    GenerateFirewalls((FireWall)host);

                }

                foreach (string identifier in host.Connections.Keys)
                {
                    NetworkNode network = host.Connections[identifier];
                    Pipeline pipeline = Instantiate(_pipeLinePrefab, environment.transform);
                    pipeline.Network = network;
                    pipeline.Host = host;
                    pipeline.Identifier = identifier;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        pipeline.TransferSize = UnityEngine.Random.Range(0.5f, 1f);
                        pipeline.TransferSpeed = UnityEngine.Random.Range(1f, 20f);
                    }
                    if (host.Connections.Count == 1)
                    {
                        pipeline.HostHeight = 0;
                        pipeline.ChangeDirection();
                        host.GetComponent<TerminalHostInteraction>().Pipeline = pipeline;
                    }
                    PipelineManager.Instance.AddPipeline(pipeline, identifier);
                }
            }
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        }

        /// <summary>
        /// Generates the room corresponding to a firewall
        /// </summary>
        /// <param name="fireWall">The firewall to generate a room for</param>
        void GenerateFirewalls(FireWall fireWall)
        {
            List<FireWallExit> exits = new List<FireWallExit>();

            //generation of the exits
            foreach (NetworkNode network in fireWall.Connections.Values)
            {
                NetworkDoor door;
                if (network.FrontDoor.FireWallExit == null)
                {
                    door = network.FrontDoor;
                    //orientation of the network room towards the firewall
                    Vector3 vector1 = network.FrontDoor.transform.right;
                    Vector3 vector2 = fireWall.transform.position - network.FrontDoor.transform.position;
                    float rotation = Mathf.Atan2(vector1.z, vector1.x) - Mathf.Atan2(vector2.z, vector2.x) * Mathf.Rad2Deg + 180;
                    int intRotation = (int)rotation / 90 * 90;
                    if (rotation - intRotation > 45)
                    {
                        intRotation += 90;
                    }
                    network.transform.RotateAround(network.FrontDoor.transform.position, Vector3.up, intRotation);
                }
                else
                {
                    door = network.BackDoor;
                }

                //generation of the exit proper
                door.ExitDescription = fireWall.name;
                FireWallExit exit = Instantiate(_fireWallTile, fireWall.transform);
                exit.transform.position = fireWall.transform.position + (door.transform.position - fireWall.transform.position).normalized * 8;
                exit.transform.right = -(door.transform.position - fireWall.transform.position).normalized;
                exit.Network = network;
                exit.Description = network.name;
                exit.FireWall = fireWall;
                door.FireWallExit = exit;
                exits.Add(exit);
            }
            foreach (FireWallExit exit in exits)
            {
                fireWall.Exits.Add(exit);
            }
            LinkFireWalls(exits, fireWall);
        }

        /// <summary>
        /// Creates walls between the different exits of the firewall
        /// </summary>
        /// <param name="exits">The exits from the firewall</param>
        /// <param name="offset">The position of the firewall</param>
        void LinkFireWalls(List<FireWallExit> exits, HostNode fireWall)
        {
            Vector3 offset = fireWall.transform.position;
            //we order the exits along a circle to know which are adjacent
            List<FireWallExit> orderedList = new List<FireWallExit>();
            while (exits.Count > 0)
            {
                float minAngle = Mathf.Infinity;
                FireWallExit minWall = null;
                foreach (FireWallExit wall in exits)
                {
                    Vector3 offsetPos = wall.transform.position - offset;
                    float angle = Mathf.Atan2(offsetPos.z, offsetPos.x);
                    if (minAngle > angle)
                    {
                        minAngle = angle;
                        minWall = wall;
                    }
                }
                orderedList.Add(minWall);
                exits.Remove(minWall);
            }
            for (int i = 0; i < orderedList.Count; i++)
            {
                //the vector connecting the extremities of two adjacent exits
                Vector3 connectingVector;
                //the point between the two adjacent exits
                Vector3 midPoint;
                //the average "right" vector of the exits (corresponds to the front of the door)
                Vector3 averageRight;
                //the distance between the exits
                float distance;
                if (i != orderedList.Count - 1)
                {
                    connectingVector = orderedList[i + 1].transform.position + _tileSize / 2f * orderedList[i + 1].transform.forward
                        - (orderedList[i].transform.position - _tileSize / 2f * orderedList[i].transform.forward);
                    distance = connectingVector.magnitude;
                    midPoint = orderedList[i].transform.position - _tileSize / 2f * orderedList[i].transform.forward + connectingVector / 2f;
                    averageRight = (orderedList[i + 1].transform.forward - orderedList[i].transform.forward) / 2f;
                }
                else
                {
                    connectingVector = orderedList[0].transform.position + _tileSize / 2f * orderedList[0].transform.forward
                        - (orderedList[i].transform.position - _tileSize / 2f * orderedList[i].transform.forward);
                    distance = connectingVector.magnitude;
                    midPoint = orderedList[i].transform.position - _tileSize / 2f * orderedList[i].transform.forward + connectingVector / 2f;
                    averageRight = (orderedList[0].transform.forward - orderedList[i].transform.forward) / 2f;
                }
                //instantiate a wall with the right size, position and orientation
                GameObject connectingWall = Instantiate(_wallTile, orderedList[i].transform);
                connectingWall.transform.position = midPoint;
                connectingWall.transform.right = averageRight;
                connectingWall.transform.localScale = new Vector3(1, 1, distance / _tileSize);
                connectingWall.transform.parent = fireWall.transform;
            }
        }

        /// <summary>
        /// Creates a list of coordinates for the tiles so they can be put in a shape as close to a rectangle as possible,
        /// before placing said tiles in the 3D environment along with walls and a door
        /// </summary>
        /// <param name="necessaryTiles">The number of tiles that need to be generated</param>
        /// <returns>The coordinates of the generated tiles relative to one another</returns>
        List<Vector2Int> PlaceTiles(int necessaryTiles, int necessaryDoors, NetworkNode room)
        {
            int closestPerfectSquare = ClosestInferiorPerfectSquare(necessaryTiles);
            int root = (int)Mathf.Sqrt(closestPerfectSquare);

            List<Vector2Int> tileTable = new List<Vector2Int>();
            for (int i = 0; i < root; i++)
            {
                for (int j = 0; j < root; j++)
                {
                    tileTable.Add(new Vector2Int(i, j));
                }
            }

            for (int i = 0; i < necessaryTiles - closestPerfectSquare; i++)
            {
                if (i < root)
                {
                    tileTable.Add(new Vector2Int(i, root));
                }
                else
                {
                    tileTable.Add(new Vector2Int(root, i - root));
                }
            }

            int lastColumn = root - 1;
            if (closestPerfectSquare != necessaryTiles)
            {
                lastColumn += 1;
            }

            int lastLine = root - 1;
            if (necessaryTiles - closestPerfectSquare > root)
            {
                lastLine += 1;
            }

            Vector3 offset = room.transform.position;
            bool firstDoorPlaced = false;
            foreach (Vector2Int vector in tileTable)
            {
                //positioning the ceiling light
                //Instantiate(_ceilingLight, new Vector3(vector.x * _tileSize + offset.x, _tileSize + offset.y, vector.y * _tileSize + offset.z),
                //Quaternion.identity, room.transform);
                if (vector.x == 0)
                {
                    //instantiation of the front door
                    if (vector.y == 0)
                    {
                        NetworkDoor door = Instantiate(_doorTile, new Vector3(vector.x * _tileSize - 0.5f * _tileSize + offset.x, offset.y, vector.y * _tileSize + offset.z),
                            Quaternion.identity, room.transform);
                        door.EntryDescription = room.name;
                        door.Network = room;
                        necessaryDoors -= 1;
                        firstDoorPlaced = true;
                        room.GetComponent<NetworkNode>().FrontDoor = door;
                    }
                    else
                    {
                        Instantiate(_wallTile, new Vector3(vector.x * _tileSize - 0.5f * _tileSize + offset.x, offset.y, vector.y * _tileSize + offset.z),
                            Quaternion.identity, room.transform);
                    }
                }
                //the test is a bit complex, but the idea is to check whether there's a tile South of this one or not
                if (vector.x == lastLine || lastLine == root - 1 && (vector.y == lastColumn && (vector.x + 1 == necessaryTiles - closestPerfectSquare || vector.x + 1 == root))
                    || lastLine == root && vector.x == root - 1 && vector.y + 1 > necessaryTiles - closestPerfectSquare - root)
                {
                    if (vector.y != 0 || necessaryDoors == 0 || (!firstDoorPlaced && necessaryDoors == 1))
                    {
                        Instantiate(_wallTile, new Vector3(vector.x * _tileSize + 0.5f * _tileSize + offset.x, offset.y, vector.y * _tileSize + offset.z),
                            Quaternion.identity, room.transform);
                    }
                    //instantiation of the backdoor
                    else
                    {
                        NetworkDoor door = Instantiate(_doorTile, new Vector3(vector.x * _tileSize + 0.5f * _tileSize + offset.x, offset.y, vector.y * _tileSize + offset.z),
                            Quaternion.Euler(0, 180, 0), room.transform);
                        necessaryDoors -= 1;
                        door.EntryDescription = room.name;
                        door.Network = room;
                        room.GetComponent<NetworkNode>().BackDoor = door;
                    }
                }
                if (vector.y == 0)
                {
                    Instantiate(_wallTile, new Vector3(vector.x * _tileSize + offset.x, offset.y, vector.y * _tileSize - 0.5f * _tileSize + offset.z),
                        Quaternion.Euler(0, 90, 0), room.transform);
                }
                //here we check whether there is a tile East of this one or not
                if (vector.y == lastColumn || (lastLine == root - 1 && vector.y == root - 1 && necessaryTiles - closestPerfectSquare - vector.x <= 0) || (vector.x == root && vector.y + 1 == necessaryTiles - root * (root + 1)))
                {
                    Instantiate(_wallTile, new Vector3(vector.x * _tileSize + offset.x, offset.y, vector.y * _tileSize + 0.5f * _tileSize + offset.z),
                        Quaternion.Euler(0, 90, 0), room.transform);
                }
            }

            return tileTable;
        }

        /// <summary>
        /// Groups the nodes by two and places them in front of each other on a tile in the 3D environment
        /// </summary>
        /// <param name="nodes">The nodes to place in the environment</param>
        /// <param name="tiles">The coordinates of the tiles (independant of their actual size)</param>
        void PlaceNodes(List<AbstractNode> nodes, List<Vector2Int> tiles, GameObject room)
        {
            Vector3 offset = room.transform.position;
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].transform.parent = room.transform;
                if (i % 2 == 0)
                {
                    nodes[i].transform.position = new Vector3(tiles[i / 2].x * _tileSize + offset.x, offset.y,
                        tiles[i / 2].y * _tileSize + nodes[i].Size / 4 + offset.z);
                }
                else
                {
                    if (nodes[i] is Desktop)
                    {
                        nodes[i].transform.Rotate(0, 180, 0);
                    }
                    nodes[i].transform.position = new Vector3(tiles[i / 2].x * _tileSize + offset.x, offset.y,
                        tiles[i / 2].y * _tileSize - nodes[i].Size / 4 + offset.z);
                }
            }
        }

        /// <summary>
        /// Finds the closest number to N that's both a perfect square and inferior to N
        /// </summary>
        /// <param name="N">The number for which the closest inferior square is needed</param>
        /// <returns></returns>
        int ClosestInferiorPerfectSquare(int N)
        {
            while (Mathf.Sqrt(N) - Mathf.Floor(Mathf.Sqrt(N)) != 0)
            {
                N -= 1;
            }
            return N;
        }

        /// <summary>
        /// Distributes the nodes so they are contained in a square of a given size
        /// </summary>
        /// <param name="networks">The networks to distribute</param>
        /// <param name="hosts">The hosts to distribute</param>
        /// <param name="environment">The environment containing the hosts and networks</param>
        /// <param name="idealMaxDistance">The distance of the square containing the environment</param>
        void AdjustDistance(List<NetworkNode> networks, List<HostNode> hosts, Environment environment, float idealMaxDistance = 150)
        {
            float maxX = 0;
            float maxZ = 0;
            float minX = Mathf.Infinity;
            float minZ = Mathf.Infinity;
            float averageX = 0;
            float averageZ = 0;
            foreach (NetworkNode network in networks)
            {
                if (maxX < network.transform.position.x)
                {
                    maxX = network.transform.position.x;
                }

                if (minX > network.transform.position.x)
                {
                    minX = network.transform.position.x;
                }
                averageX += network.transform.position.x;

                if (minZ > network.transform.position.z)
                {
                    minZ = network.transform.position.z;
                }
                averageZ += network.transform.position.z;
            }

            foreach (HostNode host in hosts)
            {
                if (maxX < host.transform.position.x)
                {
                    maxX = host.transform.position.x;
                }
                averageX += host.transform.position.x;

                if (minX > host.transform.position.x)
                {
                    minX = host.transform.position.x;
                }

                if (minZ > host.transform.position.z)
                {
                    minZ = host.transform.position.z;
                }

                if (maxZ < host.transform.position.z)
                {
                    maxZ = host.transform.position.z;
                }
                averageZ += host.transform.position.z;
            }

            averageX /= networks.Count + hosts.Count;
            averageZ /= networks.Count + hosts.Count;

            foreach (NetworkNode network in networks)
            {
                network.transform.position -= new Vector3(averageX, 0, averageZ);
                network.transform.position = new Vector3(network.transform.position.x * idealMaxDistance / (maxX - minX),
                    network.transform.position.y, network.transform.position.z * idealMaxDistance / (maxZ - minZ));
            }

            foreach (HostNode host in hosts)
            {
                host.transform.position -= new Vector3(averageX, 0, averageZ);
                host.transform.position = new Vector3(host.transform.position.x * idealMaxDistance / (maxX - minX),
                   host.transform.position.y, host.transform.position.z * idealMaxDistance / (maxZ - minZ));
            }

            //we instantiate a single floor and ceiling for all the environment
            FloorInteraction floor = Instantiate(_floorTile, environment.transform);
            floor.transform.localScale = new Vector3(idealMaxDistance / _tileSize, 1, idealMaxDistance / _tileSize);
            floor.Assistant = _assistant;

            /*GameObject ceiling = Instantiate(_ceilingTile, environment.transform);
            ceiling.transform.localScale = new Vector3(idealMaxDistance / _tileSize, 1, idealMaxDistance / _tileSize);
            ceiling.transform.Translate(Vector3.up * _tileSize);*/
        }

        /// <summary>
        /// Creates a sprite from an svg file stored as a string
        /// </summary>
        /// <param name="svg">The svg file to convert to a sprite</param>
        /// <returns></returns>
        Sprite CreateSVGSprite(string svg)
        {
            var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));

            // Tessellate
            var tessOptions = new VectorUtils.TessellationOptions();
            tessOptions.StepDistance = 10f;
            tessOptions.SamplingStepSize = 1000;
            tessOptions.MaxCordDeviation = 1;
            tessOptions.MaxTanAngleDeviation = 5;
            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);

            // Build a sprite
            Sprite sprite = VectorUtils.BuildSprite(geoms, 200.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
            return sprite;
        }
    }
}