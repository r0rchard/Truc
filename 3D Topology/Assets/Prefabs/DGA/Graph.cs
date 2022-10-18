using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GraphGestion
{
    [Serializable]
    public class Graph<TNodeType, TEdgeType>
    {
        public List<Node<TNodeType>> Nodes;
        public List<Edge<TEdgeType, TNodeType>> Edges;
        public Graph()
        {
            Nodes = new List<Node<TNodeType>>();
            Edges = new List<Edge<TEdgeType, TNodeType>>();
        }

    }
    [Serializable]
    public class Node<TNodeType>
    {
        public Color NodeColor;
        public TNodeType Position;

        public string Description;
        public string Name;
        public string Identifier;
        public List<Node<TNodeType>> Parents;

        public List<Node<TNodeType>> Children;

    }
    [Serializable]
    public class Edge<TEdgeType, TNodeType>
    {
        public Color EdgeColor;
        public TEdgeType Value;
        public Node<TNodeType> From;
        public Node<TNodeType> To;
    }
}