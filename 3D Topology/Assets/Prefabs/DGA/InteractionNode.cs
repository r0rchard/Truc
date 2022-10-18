using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphGestion;
public class InteractionNode : MonoBehaviour
{

    public Node<Vector3> Node { private get; set;}
    public GraphGenerator nodeManager { private get; set;}

    public bool Activate;

    private void Update()
    {
        if(Activate)
        {
            ActivateAsset();
        }
        else
        { 
            DeactivateAsset();
        }
    }
    public void ActivateAsset()
    {
        nodeManager.EscalateAlerte(Node);
    }
    public void DeactivateAsset()
    {
        nodeManager.DeEscalateAlerte(Node);
    }

    public void ChangeColor(Color c)
    {
        GetComponent<Renderer>().material.SetColor("_Color", c);
    }

}
