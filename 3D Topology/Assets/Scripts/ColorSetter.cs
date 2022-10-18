using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(Renderer))]
public class ColorSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonView view = GetComponent<PhotonView>();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = ColorScheme.Instance.GetPlayerColor(view.Owner.ActorNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
