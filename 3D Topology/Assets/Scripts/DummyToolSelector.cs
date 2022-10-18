using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DummyToolSelector : MonoBehaviour
{
    [SerializeField] List<DummyTool> _dummyTools;
    PhotonView _view;

    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
        if (_view.IsMine)
        {
            _dummyTools[0].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void SelectTool(int index)
    {
        for(int i = 0; i<_dummyTools.Count; i++)
        {
            _dummyTools[i].gameObject.SetActive(i == index);
        }
    }

    public void NetworkSelectTool(int index)
    {
        _view.RPC("SelectTool", RpcTarget.Others, index);
    }
}
