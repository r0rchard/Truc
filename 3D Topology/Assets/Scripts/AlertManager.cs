using EnvironmentGeneration;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    public List<HostNode> Hosts;
    PhotonView _view;

    //singleton
    public static AlertManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void RaiseAlert(int index)
    {
        Hosts[index].RaiseAlert();
    }

    [PunRPC]
    void StopAlert(int index)
    {
        Hosts[index].StopAlert();
    }

    public void NetworkRaiseAlert(HostNode host)
    {
        _view.RPC("RaiseAlert", RpcTarget.All, Hosts.IndexOf(host));
    }

    public void NetworkStopAlert(HostNode host)
    {
        _view.RPC("StopAlert", RpcTarget.All, Hosts.IndexOf(host));
    }
}
