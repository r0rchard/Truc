using EnvironmentGeneration;
using Interaction;
using Parsing.CyberRangeSerialization;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using GraphGestion;

public class GraphGenerator : MonoBehaviour
{
    //public HostNode _infoprefab;
    //public GameObject _networkprefab;


    public static GraphGenerator Instance;


    public Material lrMaterial;

    public GameObject supportGraph;
    private GameObject _dependencyGO;
    private GameObject _networkGO;

    public GameObject dependecyPrefab;
    public GameObject firewallPrefab;
    public GameObject networkPrefab;
    public GameObject materialPrefab;

    private List<Node<Vector3>> _materrialNodesList;
    private List<Node<Vector3>> _networkNodesList;
    private List<Node<Vector3>> _firewallNodesList;

    private Graph<Vector3, float> _networkGraph;
    private Graph<Vector3, float> _dependencyGraph;

    List<Node<Vector3>> _nodeMissionList = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeObjectivesList = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeTaskList = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeUpFunctionList = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeDownFunctionList = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeAssetList = new List<Node<Vector3>>();

    List<Node<Vector3>> _nodeOrList1 = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeOrList2 = new List<Node<Vector3>>();
    List<Node<Vector3>> _nodeOrList3 = new List<Node<Vector3>>();


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public void EscalateAlerte(Node<Vector3> node)
    {
        GameObject.Find(node.Identifier).GetComponent<InteractionNode>().ChangeColor(Color.red);
        foreach (Node<Vector3> parent in node.Parents)
        {
            EscalateAlerteParent(parent);
        }
        foreach(Node<Vector3> child in node.Children)
        {
            EscalateAlerteChild(child);
        }
    }
    public void EscalateAlerteParent(Node<Vector3> node)
    {
        GameObject.Find(node.Identifier).GetComponent<InteractionNode>().ChangeColor(Color.red);
        foreach (Node<Vector3> parent in node.Parents)
        {
            EscalateAlerteParent(parent);
        }
    }
    public void EscalateAlerteChild(Node<Vector3> node)
    {
        GameObject.Find(node.Identifier).GetComponent<InteractionNode>().ChangeColor(Color.red);
        foreach (Node<Vector3> child in node.Children)
        {
            EscalateAlerteChild(child);
        }
    }

    public void DeEscalateAlerte(Node<Vector3> node)
    {

        foreach (Node<Vector3> parent in node.Parents)
        {
            DeEscalateAlerteParent(parent);
        }
        foreach (Node<Vector3> child in node.Children)
        {
            DeEscalateAlerteChild(child);
        }
    }
    public void DeEscalateAlerteParent(Node<Vector3> node)
    {

        foreach (Node<Vector3> parent in node.Parents)
        {
            DeEscalateAlerteParent(parent);
        }
    }
    public void DeEscalateAlerteChild(Node<Vector3> node)
    {

        foreach (Node<Vector3> child in node.Children)
        {
           DeEscalateAlerteChild(child);
        }
    }

    public void GenerateEnvironment(HostsSpecsAsset[] hostAssets, NetworksSpecsAsset[] networkAssets)
    {
        _networkGraph = new Graph<Vector3, float>();
        _dependencyGraph = new Graph<Vector3, float>();

        _dependencyGO = Instantiate(supportGraph);
        _networkGO = Instantiate(supportGraph);
        _networkGO.transform.SetParent(_dependencyGO.transform);
        

       
    

    //generate parent object
    Environment environment = new GameObject().AddComponent(typeof(Environment)) as Environment;
        environment.name = "Environment";



        


        _networkGraph = GenerationNetworkGraph(environment, hostAssets, networkAssets);
        DisplayNetworkGraph(_networkGraph);
        _dependencyGraph = GenerationDependencyGraph();

        _networkGO.transform.position += new Vector3(25, -1, -12);
        _networkGO.transform.Rotate(new Vector3(90, 0, 0), Space.World);

        DisplayDependencyGraph();

        //string dependencyGraph = JsonUtility.ToJson(_dependencyGraph);
        //string networkGraph = JsonUtility.ToJson(_networkGraph); 
        //System.IO.File.WriteAllText("Assets/Prefabs/DGA/JSON" + "/DependencyGraph.json", dependencyGraph);
        //System.IO.File.WriteAllText("Assets/Prefabs/DGA/JSON" + "/NetworkGraph.json", networkGraph);

    }

    private void DisplayDependencyGraph()
    {
        float distanceBetweenTwoNodes = 6f;
        float widthGraph = _nodeAssetList.Count; //parce que c'est la plus grande
        float centerGraph = ((float)widthGraph)/2;

        Vector3 positionOr1 = new Vector3((centerGraph - ((float)_nodeOrList1.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*4.8f, 0);
        Vector3 positionMission = new Vector3((centerGraph - ((float)_nodeMissionList.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*4.2f, 0);
        Vector3 positionObjective = new Vector3((centerGraph - ((float)_nodeObjectivesList.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*3.6f, 0);
        Vector3 positiontask = new Vector3((centerGraph - ((float)_nodeTaskList.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*3f, 0);
        Vector3 positionOr2 = new Vector3((centerGraph - ((float)_nodeOrList2.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*2.4f, 0);
        Vector3 positionFunctionUp = new Vector3((centerGraph - ((float)_nodeUpFunctionList.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*1.8f, 0);
        Vector3 positionFunctionDown = new Vector3((centerGraph - ((float)_nodeDownFunctionList.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*1.2f, 0);
        Vector3 positionOr3 = new Vector3((centerGraph - ((float)_nodeOrList3.Count) / 2) * distanceBetweenTwoNodes, distanceBetweenTwoNodes*.6f, 0);
        Vector3 positionAsset = new Vector3((centerGraph - ((float)_nodeAssetList.Count) / 2) * distanceBetweenTwoNodes, 0, 0);


        foreach (Node<Vector3> orNode1 in _nodeOrList1)
        {
            orNode1.Position = positionOr1;
            positionOr1.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> missionNode in _nodeMissionList)
        {
            missionNode.Position = positionMission;
            positionMission.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> objectiveNode in _nodeObjectivesList)
        {
            objectiveNode.Position = positionObjective;
            positionObjective.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> taskNode in _nodeTaskList)
        {
            taskNode.Position = positiontask;
            positiontask.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> orNode2 in _nodeOrList2)
        {
            orNode2.Position = positionOr2;
            positionOr2.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> functionUpNode in _nodeUpFunctionList)
        {
            functionUpNode.Position = positionFunctionUp;
            positionFunctionUp.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> functionDownNode in _nodeDownFunctionList)
        {
            functionDownNode.Position = positionFunctionDown;
            positionFunctionDown.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> orNode3 in _nodeOrList3)
        {
            orNode3.Position = positionOr3;
            positionOr3.x +=distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> assetNode in _nodeAssetList)
        {
            assetNode.Position = positionAsset;
            positionAsset.x += distanceBetweenTwoNodes;
        }
                  


        foreach (var node in _dependencyGraph.Nodes)
        {
            var go = Instantiate(dependecyPrefab, node.Position, Quaternion.identity);
            go.GetComponent<Renderer>().material.SetColor("_Color", node.NodeColor);
            go.transform.SetParent(_dependencyGO.transform);
            go.GetComponent<NodeName>().SetName(node.Name);
            go.name = node.Name;
            go.GetComponent<InteractionNode>().nodeManager = this;
            go.GetComponent<InteractionNode>().Node = node;

        }

        // cette boucle ne devrait pas être là, permet de faire la liaison entre les deux graphs
        foreach (Node<Vector3> firewallNode in _firewallNodesList)
        {
            var go = GameObject.Find(firewallNode.Identifier);
            go.transform.parent = null;
            Vector3 linkPosition = go.transform.TransformPoint(Vector3.zero); ;
            firewallNode.Position = linkPosition;
            _dependencyGraph.Edges.Add(CreationEdge(_nodeAssetList.Find(element => element.Name == "VOIP phone"), firewallNode));
        }

        foreach (var edge in _dependencyGraph.Edges)
        {
            GameObject support = new GameObject();
            var lr = support.AddComponent<LineRenderer>();

            lr.SetPosition(0, edge.From.Position);
            lr.SetPosition(1, edge.To.Position);
            lr.material.color = edge.EdgeColor;
            support.transform.SetParent(_dependencyGO.transform);
            lr.useWorldSpace = false;
            lr.SetWidth(.2f, .05f);
        }
       
    }

    private void DisplayNetworkGraph(Graph<Vector3, float> graph)
    {

        float distanceBetweenTwoNodes = 2.5f;
        float widthGraph = _materrialNodesList.Count; //parce que c'est la plus grande
        float centerGraph = ((float)widthGraph) / 2;

        Vector3 positionNetwork = new Vector3((centerGraph - ((float)_networkNodesList.Count) / 2) * distanceBetweenTwoNodes, 5, 0);
        Vector3 positionFirewall = new Vector3((centerGraph - ((float)_firewallNodesList.Count) / 2) * distanceBetweenTwoNodes,9, 0);
        Vector3 positionMat = new Vector3((centerGraph - ((float)_materrialNodesList.Count) / 2) * distanceBetweenTwoNodes, 1, 0);
       
        foreach (Node<Vector3> networkNode in _networkNodesList)
        {
            networkNode.Position = positionNetwork;
            positionNetwork.x += distanceBetweenTwoNodes;
        }
        foreach (Node<Vector3> firewallNode in _firewallNodesList)
        {
            firewallNode.Position = positionFirewall;
            positionFirewall.x += distanceBetweenTwoNodes;
        }

        foreach (Node<Vector3> materialNode in _materrialNodesList)
        {
            materialNode.Position = positionMat;
            positionMat.x += distanceBetweenTwoNodes;
        }


        foreach (var node in _networkGraph.Nodes)
        {
            if(node.Description=="Firewall")
            {
                var go = Instantiate(firewallPrefab, node.Position, Quaternion.identity);
                //go.GetComponent<Renderer>().material.SetColor("_Color", node.NodeColor);
                go.transform.SetParent(_networkGO.transform);
                go.GetComponent<NodeName>().SetName(node.Name);
                go.name = node.Identifier;
                go.GetComponent<InteractionNode>().nodeManager = this;
                go.GetComponent<InteractionNode>().Node = node;
            }
            else if (node.Description == "Server")
            {
                var go = Instantiate(materialPrefab, node.Position, Quaternion.identity);
                //go.GetComponent<Renderer>().material.SetColor("_Color", node.NodeColor);
                go.transform.SetParent(_networkGO.transform);
                go.GetComponent<NodeName>().SetName(node.Name);
                go.name = node.Identifier;
                go.GetComponent<InteractionNode>().nodeManager = this;
                go.GetComponent<InteractionNode>().Node = node;
            }
            else
            {
                var go = Instantiate(networkPrefab, node.Position, Quaternion.identity);
                //go.GetComponent<Renderer>().material.SetColor("_Color", node.NodeColor);
                go.transform.SetParent(_networkGO.transform);
                go.GetComponent<NodeName>().SetName(node.Name);
                go.name = node.Identifier;
                go.GetComponent<InteractionNode>().nodeManager = this;
                go.GetComponent<InteractionNode>().Node = node;
            }
            
        }
        foreach (var edge in _networkGraph.Edges)
        {
            GameObject support = new GameObject();
            var lr = support.AddComponent<LineRenderer>();
            lr.SetPosition(0, edge.From.Position);
            lr.SetPosition(1, edge.To.Position);
            lr.material.color = edge.EdgeColor;
            support.transform.SetParent(_networkGO.transform);
            lr.useWorldSpace = false;
            lr.SetWidth(.2f, .05f);
        }
    }

    private Graph<Vector3, float> GenerationNetworkGraph(Environment env,HostsSpecsAsset[] hostsSpecsAsset,NetworksSpecsAsset[] networksSpecsAsset)
    {
        //generate networks
        Graph<Vector3, float> graph = new Graph<Vector3, float>();
        List<NetworkNode> networks = new List<NetworkNode>();

        _materrialNodesList = new List<Node<Vector3>>();
        _networkNodesList = new List<Node<Vector3>>();
        _firewallNodesList = new List<Node<Vector3>>();

        foreach (NetworksSpecsAsset network in networksSpecsAsset)
        {
            //creation of abstraction
            NetworkNode networkAbstraction = new GameObject(network.attributes.label_text).AddComponent<NetworkNode>();

            networkAbstraction.NetworkAsset = network;
            networkAbstraction.transform.position = new Vector3(network.attributes.x, 0, -network.attributes.y);
            networkAbstraction.IpAddress = IPAddress.Parse(network.ip);
            networkAbstraction.NetworkName = network.name;
            networkAbstraction.Identifier = network.identifier;
            networkAbstraction.transform.parent = env.transform;

            networks.Add(networkAbstraction);

            var node_network = new Node<Vector3>() { NodeColor = Color.green, Description = IPAddress.Parse(network.ip).ToString(), Name = network.name, Identifier = network.identifier, Children = new List<Node<Vector3>>(), Parents = new List<Node<Vector3>>() };
            _networkNodesList.Add(node_network);
            graph.Nodes.Add(node_network);
        }



        //generate hosts
        List<HostNode> hosts = new List<HostNode>();
        foreach (HostsSpecsAsset host in hostsSpecsAsset)
        {
            var node = new Node<Vector3>() { NodeColor = Color.red, Description = "Server", Name = host.attributes.label_text, Identifier = host.identifier, Children = new List<Node<Vector3>>(), Parents = new List<Node<Vector3>>() };
            HostNode hostAbstraction = new HostNode();
            
            ////the prefab is chosen depending on the name of the image associated with the node in the JSON file
            if (host.attributes.image_source.ToLower().Contains("firewall"))
            {
                _firewallNodesList.Add(node);
                node.NodeColor = Color.blue;
                node.Description = "Firewall";
                graph.Nodes.Add(node);

            }

            hostAbstraction.HostAsset = host;

            //creation of the interaction
            //hostAbstraction.Interaction = hostAbstraction.GetComponent<HostInteraction>();
            //hostAbstraction.Interaction.Description = host.attributes.label_text;
            //hostAbstraction.transform.position = new Vector3(host.attributes.x, 0, -host.attributes.y);
            //hostAbstraction.name = host.attributes.label_text;
            //hostAbstraction.Identifier = host.identifier;
            //hostAbstraction.transform.parent = environment.transform;      



            //association of networks and hosts
            foreach (NicsAsset nics in host.nics)
            {
                hostAbstraction.IpAdresses.Add(IPAddress.Parse(nics.ip));
                foreach (NetworkNode net in networks)
                {

                    if (net.NetworkName == nics.network_name || (net.Identifier != "" && net.Identifier == nics.network_identifier))
                    {
                        net.AddConnected(hostAbstraction, nics.identifier);
                        hostAbstraction.AddConnection(net, nics.identifier);

                        var node_network = _networkNodesList.Find(element => element.Identifier == net.Identifier);


                        node_network.Children.Add(node);
                        node.Parents.Add(node_network);



                        break;
                    }
                }
            }


            hosts.Add(hostAbstraction);
            

        }

        env.networks = networks;
        env.hosts = hosts;

        foreach (Node<Vector3> networkNode in _networkNodesList)
        {
            foreach (Node<Vector3> materialNode in networkNode.Children)
            {
                var edge = new Edge<float, Vector3>() { Value = 1.0f, From = networkNode, To = materialNode, EdgeColor = Color.blue };
                graph.Edges.Add(edge);

                // ici pour que les matos soient proches de leur network dans la visu mais on met pas les firewalls pour qu'ils n'apparaissent pas plusieurs fois
                if (materialNode.NodeColor!=Color.blue)
                { 
                    _materrialNodesList.Add(materialNode);
                    graph.Nodes.Add(materialNode);
                }
            }
        }

        return graph;
    }
             

    private Graph<Vector3,float> GenerationDependencyGraph()
    {
        Graph<Vector3, float> dependecyGraph = new Graph<Vector3, float>();

        List<List<string>> dependanceListList = new List<List<string>>();
        List<string> nameDepandanceList = new List<string>();
        List<List<Node<Vector3>>> nodeListList = new List<List<Node<Vector3>>>();

        List<string> missionList= new List<string> { "Medic (Land)", "Medevac (Air)", "Ratm Rescue", "Medevac (SEA)", "Rapid Deploy Medic (LAND)" };
        List<string> objectivesList= new List<string> { "Search and Rescue", "Deploy", "Plan" };
        List<string> taskList= new List<string> { "Establish Data Comms", "Obtain Target Position", "Establish Voice Comms", "Obtain Weither Information","Navigate the Aircraft", "Evaluate Air Traffic","Receive ATO","Obtain Weather Forcast" };
        List<string> upFunctionList= new List<string> { "EMAIL service","Air chat","VOIP service","UHF radio","VHF radio","COP service","Weather service","NIRIS","ATO service","Weather Forecast service","CDSAS","PNT service" };
        List<string> downFunctionList = new List<string> { "CHAT service","FFT service","Sea chat","Wideband satellite service","ATC radio","Land chat","ICC service","RADAR service","RAP","Or(11)","Or(12)"};
        List<string> assetList= new List<string> { "Email server 3","Email server 1","Email server 2","FFT server","WB SATCOM","Air deployable chat server","ATC radio 1","Sea deployable chat server","Land deployable chat server","VOIP phone","ICC server","UHF radio 2","UHF radio 1","VHF radio 2","VHF radio 1","Weather RADAR","Omni RADAR antenna","Directed RADAR antenna","GPS receiver","GPS constellation","Galileo receiver","Galileo constellation" };

        List<string> orList1 = new List<string> { "Or" };
        List<string> orList2 = new List<string> { "Or(1)", "Or(2)", "Or(3)" };
        List<string> orList3 = new List<string> { "Or(4)", "Or(5)", "Or(6)", "Or(7)", "Or(8)", "Or(9)","Or(10)","And(1)","And(2)" };

        dependanceListList.AddRange(new List<List<string>> { orList1,missionList, objectivesList, taskList, orList2, upFunctionList, downFunctionList, orList3,assetList});
        nameDepandanceList.AddRange(new List<string> { "orList1","missionList", "objectivesList", "taskList", "orList2", "upFunctionList", "downFunctionList", "orList3","assetList"});

        _nodeMissionList = new List<Node<Vector3>>();
        _nodeObjectivesList = new List<Node<Vector3>>();
        _nodeTaskList = new List<Node<Vector3>>();
        _nodeUpFunctionList = new List<Node<Vector3>>();
        _nodeDownFunctionList = new List<Node<Vector3>>();
        _nodeAssetList = new List<Node<Vector3>>();

        _nodeOrList1 = new List<Node<Vector3>>();
        _nodeOrList2 = new List<Node<Vector3>>();
        _nodeOrList3 = new List<Node<Vector3>>();

       

        nodeListList.AddRange(new List<List<Node<Vector3>>> { _nodeOrList1, _nodeMissionList, _nodeObjectivesList, _nodeTaskList, _nodeOrList2, _nodeUpFunctionList, _nodeDownFunctionList, _nodeOrList3,_nodeAssetList });

        for (int k = 0; k <dependanceListList.Count;k++)
        {
            foreach (string dependance in dependanceListList[k])
            {
                var node = CreationNode(dependance, dependance, nameDepandanceList[k]);
                dependecyGraph.Nodes.Add(node);
                nodeListList[k].Add(node);
            }
        }

        //foreach (Node<Vector3> element in _nodeOrList1)
        //{
        //    print(element.Name);
        //}


        dependecyGraph.Edges.AddRange(new List<Edge<float, Vector3>> { 
            CreationEdge(_nodeOrList1.Find(element => element.Name == "Or"), _nodeMissionList.Find(element => element.Name == "Medic (Land)")),
            CreationEdge(_nodeOrList1.Find(element => element.Name == "Or"), _nodeMissionList.Find(element => element.Name == "Medevac (Air)")),
            CreationEdge(_nodeOrList1.Find(element => element.Name == "Or"), _nodeMissionList.Find(element => element.Name == "Medevac (SEA)")),
            CreationEdge(_nodeOrList1.Find(element => element.Name == "Or"), _nodeMissionList.Find(element => element.Name == "Rapid Deploy Medic (LAND)")),
            
            CreationEdge(_nodeMissionList.Find(element => element.Name == "Ratm Rescue"),_nodeOrList1.Find(element => element.Name == "Or")),
            CreationEdge(_nodeMissionList.Find(element => element.Name == "Medevac (Air)"),_nodeObjectivesList.Find(element => element.Name == "Search and Rescue")),
            CreationEdge(_nodeMissionList.Find(element => element.Name == "Medevac (Air)"),_nodeObjectivesList.Find(element => element.Name == "Deploy")),
            CreationEdge(_nodeMissionList.Find(element => element.Name == "Medevac (Air)"),_nodeObjectivesList.Find(element => element.Name == "Plan")),

            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Search and Rescue"),_nodeTaskList.Find(elemet => elemet.Name == "Establish Data Comms")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Search and Rescue"),_nodeTaskList.Find(elemet => elemet.Name == "Obtain Target Position")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Search and Rescue"),_nodeTaskList.Find(elemet => elemet.Name == "Establish Voice Comms")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Search and Rescue"),_nodeTaskList.Find(elemet => elemet.Name == "Obtain Weither Information")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Search and Rescue"),_nodeTaskList.Find(elemet => elemet.Name == "Navigate the Aircraft")),

            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Deploy"),_nodeTaskList.Find(elemet => elemet.Name == "Establish Voice Comms")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Deploy"),_nodeTaskList.Find(elemet => elemet.Name == "Evaluate Air Traffic")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Deploy"),_nodeTaskList.Find(elemet => elemet.Name == "Obtain Weither Information")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Deploy"),_nodeTaskList.Find(elemet => elemet.Name == "Navigate the Aircraft")),

            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Plan"),_nodeTaskList.Find(elemet => elemet.Name == "Obtain Target Position")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Plan"),_nodeTaskList.Find(elemet => elemet.Name == "Evaluate Air Traffic")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Plan"),_nodeTaskList.Find(elemet => elemet.Name == "Receive ATO")),
            CreationEdge(_nodeObjectivesList.Find(element => element.Name == "Plan"),_nodeTaskList.Find(elemet => elemet.Name == "Obtain Weather Forcast")),
            
        });

        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Establish Data Comms", _nodeTaskList, new List<string> { "Wideband satellite service" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Obtain Target Position", _nodeTaskList, new List<string> { "Or(1)", "Or(2)" }, _nodeOrList2));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Obtain Target Position", _nodeTaskList, new List<string> { "COP service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Obtain Target Position", _nodeTaskList, new List<string> { "FFT service" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Establish Voice Comms", _nodeTaskList, new List<string> { "Or(3)" }, _nodeOrList2));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Establish Voice Comms", _nodeTaskList, new List<string> { "ATC radio" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Obtain Weither Information", _nodeTaskList, new List<string> { "Weather service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Navigate the Aircraft", _nodeTaskList, new List<string> { "Weather service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Navigate the Aircraft", _nodeTaskList, new List<string> { "Weather service","PNT service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Navigate the Aircraft", _nodeTaskList, new List<string> { "RADAR service","RAP" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Evaluate Air Traffic", _nodeTaskList, new List<string> { "RAP" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Receive ATO", _nodeTaskList, new List<string> { "ATO service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Obtain Weather Forcast", _nodeTaskList, new List<string> { "Weather Forecast service" }, _nodeUpFunctionList));

        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(1)", _nodeOrList2, new List<string> { "EMAIL service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(1)", _nodeOrList2, new List<string> { "CHAT service" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(2)", _nodeOrList2, new List<string> { "VOIP service" ,"UHF radio","VHF radio"}, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(3)", _nodeOrList2, new List<string> { "VOIP service" ,"UHF radio","VHF radio"}, _nodeUpFunctionList));

        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("EMAIL service", _nodeUpFunctionList, new List<string> { "Or(4)"}, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Air chat", _nodeUpFunctionList, new List<string> { "Or(6)" }, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneChildren(new List<string> { "EMAIL service", "Air chat", "VOIP service","COP service" }, _nodeUpFunctionList, "Wideband satellite service", _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("VOIP service", _nodeUpFunctionList, new List<string> { "VOIP phone" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("UHF radio", _nodeUpFunctionList, new List<string> { "Or(9)" }, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("VHF radio", _nodeUpFunctionList, new List<string> { "Or(10)" }, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("COP service", _nodeUpFunctionList, new List<string> { "Or(11)" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Weather service", _nodeUpFunctionList, new List<string> { "Weather RADAR" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("NIRIS", _nodeUpFunctionList, new List<string> { "RAP" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("PNT service", _nodeUpFunctionList, new List<string> { "Or(12)" }, _nodeDownFunctionList));
        
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("CHAT service", _nodeDownFunctionList, new List<string> { "Or(5)" }, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("FFT service", _nodeDownFunctionList, new List<string> { "FFT server" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Sea chat", _nodeDownFunctionList, new List<string> { "Or(7)" }, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Wideband satellite service", _nodeDownFunctionList, new List<string> { "WB SATCOM" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("ATC radio", _nodeDownFunctionList, new List<string> { "ATC radio 1" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Land chat", _nodeDownFunctionList, new List<string> { "Or(8)" }, _nodeOrList3));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("ICC service", _nodeDownFunctionList, new List<string> { "ICC server" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("RADAR service", _nodeDownFunctionList, new List<string> { "Omni RADAR antenna" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("RADAR service", _nodeDownFunctionList, new List<string> { "Directed RADAR antenna" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(11)", _nodeDownFunctionList, new List<string> { "CDSAS", "PNT service" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(12)", _nodeDownFunctionList, new List<string> { "And(1)", "And(2)" }, _nodeOrList3));

        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(4)", _nodeOrList3, new List<string> { "Email server 3", "Email server 1", "Email server 2" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(5)", _nodeOrList3, new List<string> { "Air chat" }, _nodeUpFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(5)", _nodeOrList3, new List<string> { "Sea chat", "Land chat" }, _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(6)", _nodeOrList3, new List<string> { "Air deployable chat server"}, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneChildren(new List<string> { "Or(6)", "Or(7)", "Or(8)" }, _nodeOrList3, "ICC service", _nodeDownFunctionList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(7)", _nodeOrList3, new List<string> { "Sea deployable chat server" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(8)", _nodeOrList3, new List<string> { "Land deployable chat server" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(9)", _nodeOrList3, new List<string> { "UHF radio 2", "UHF radio 1" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("Or(10)", _nodeOrList3, new List<string> { "VHF radio 2", "VHF radio 1" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("And(1)", _nodeOrList3, new List<string> { "GPS receiver", "GPS constellation" }, _nodeAssetList));
        dependecyGraph.Edges.AddRange(CreationAllEdgeOneParent("And(2)", _nodeOrList3, new List<string> { "Galileo receiver", "Galileo constellation" }, _nodeAssetList));

        

        return dependecyGraph;
    }

    private void LinkDependencyAndNetwork()
    {
       
    }
    private void CreationDependencyEdges()
    {
        
    }

    private List<Edge<float, Vector3>> CreationAllEdgeOneParent(string nameParent, List<Node<Vector3>> parentList, List<string> nameChildrenList, List<Node<Vector3>> childrenList)
    {
        List<Edge<float,Vector3>> edgeList = new List<Edge<float, Vector3>>();

        foreach(string child in nameChildrenList)
        {
            edgeList.Add(CreationEdge(parentList.Find(element => element.Name == nameParent), childrenList.Find(elemet => elemet.Name == child)));
        }

        return edgeList;
    }
    private List<Edge<float, Vector3>> CreationAllEdgeOneChildren( List<string> nameParentList, List<Node<Vector3>> parentList, string nameChildren, List<Node<Vector3>> childrenList)
    {
        List<Edge<float,Vector3>> edgeList = new List<Edge<float, Vector3>>();

        foreach(string parent in nameParentList)
        {
            edgeList.Add(CreationEdge(parentList.Find(element => element.Name == parent), childrenList.Find(elemet => elemet.Name == nameChildren)));
        }

        return edgeList;
    }


    private Node<Vector3> CreationNode(string name, string identifier, string description)
    {
        // name : name of the node
        // identifier : unique ID
        // description : categorie of dependece
        // color : status of the node
        return new Node<Vector3>() { NodeColor = Color.green, Description = description, Name = name, Identifier = identifier,Children = new List<Node<Vector3>>(), Parents = new List<Node<Vector3>>() };
    }

    private Edge<float,Vector3> CreationEdge(Node<Vector3> Origin, Node<Vector3> End)
    {
        

        var edge = new Edge<float, Vector3>() { Value = 1f, From = Origin, To = End, EdgeColor = Color.gray };

        Origin.Children.Add(End);
        End.Parents.Add(Origin);
        return edge;
    }

    private Edge<float, Vector3> CreationEdgeBetweenGraphs(Node<Vector3> Origin, Node<Vector3> End)
    {

        Origin.Position.y -= 1f;
        var edge = new Edge<float, Vector3>() { Value = 1f, From = Origin, To = End, EdgeColor = Color.gray };

        Origin.Children.Add(End);
        End.Parents.Add(Origin);
        return edge;
    }

}
