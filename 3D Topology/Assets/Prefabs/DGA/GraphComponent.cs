using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphGestion;

public class GraphComponent : MonoBehaviour
{
    private Graph<Vector3, float> graph;
    public GameObject server;


    // Start is called before the first frame update
    void Start()
    {
        graph = new Graph<Vector3, float>();
        var node1 = new Node<Vector3>() { Position = Vector3.zero, NodeColor = Color.red };
        var node2 = new Node<Vector3>() { Position = Vector3.one, NodeColor = Color.green };
        var edge1 = new Edge<float,Vector3>() { Value = 1.0f, From = node1, To=node2, EdgeColor=Color.yellow };

        graph.Nodes.Add(node1);
        graph.Nodes.Add(node2);
        graph.Edges.Add(edge1);
        foreach (var node in graph.Nodes)
        {
            //Gizmos.color = node.NodeColor;
            //Gizmos.DrawSphere(node.Value, 0.125f);

            Instantiate(server, node.Position, Quaternion.identity);
        }
        foreach (var edge in graph.Edges)
        {
            GameObject support = new GameObject();
            var lr = support.AddComponent<LineRenderer>();
            lr.SetPosition(0, edge.From.Position);
            lr.SetPosition(1, edge.To.Position);
        }
    }
}


