using Interaction;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RennesBuilding : MonoBehaviour
{
    [SerializeField] MeshFilter _wallContainer;
    [SerializeField] Transform _playerSpawn;
    [SerializeField] Transform _assistantSpawn;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = transform.position;
        transform.position = Vector3.zero;
        List<CombineInstance> combines = new List<CombineInstance>();
        foreach (Transform childTransform in GetComponentsInChildren<Transform>())
        {
            GameObject child = childTransform.gameObject;
            if(child.name.ToLower().Contains("porte") && child.name != "Porte-fenêtre" && !child.name.ToLower().Contains("transport"))
            {
                child.SetActive(false);
            }
            else if(child.name.ToLower().Contains("mur") || child.name.ToLower().Contains("cloison"))
            {
                CombineInstance combine = new CombineInstance();
                combine.mesh = child.GetComponent<MeshFilter>().mesh;
                combine.transform = child.GetComponent<MeshFilter>().transform.localToWorldMatrix;
                child.SetActive(false);
                combines.Add(combine);
                //child.AddComponent<MeshCollider>();                
            }
            else if(child.name == "Revêtement sol" || child.name.ToLower().Contains("volee"))
            {
                child.AddComponent<MeshCollider>();
                child.AddComponent<NavMeshSurface>();
                child.AddComponent<FloorInteraction>();
            }
        }
        _wallContainer.mesh = new Mesh();
        _wallContainer.mesh.CombineMeshes(combines.ToArray());
        _wallContainer.gameObject.AddComponent<MeshCollider>();
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TeleportToEntry()
    {
        FindObjectOfType<XRRig>().transform.position = _playerSpawn.transform.position;
        FindObjectOfType<Assistant>().Agent.Warp(_assistantSpawn.transform.position);
    }
}
