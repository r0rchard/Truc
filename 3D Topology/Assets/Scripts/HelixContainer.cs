using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelixContainer : MonoBehaviour
{
    PhotonView _view;
    [SerializeField] List<Slider> _sliders;
    bool _sendUpdate = true;

    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
        for(int i = 0; i < _sliders.Count; i++)
        {
            int test = i;
            _sliders[i].onValueChanged.AddListener(delegate { SendSliderUpdate(test); });
            if (!PhotonNetwork.IsMasterClient)
            {
                SyncSliderToMaster(i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SendSliderUpdate(int index)
    {
        if (_sendUpdate)
        {
            _view.RPC("UpdateSliderValue", RpcTarget.Others, index, _sliders[index].value);
        }
    }

    [PunRPC]
    public void UpdateSliderValue(int index, float value)
    {
        _sendUpdate = false;
        _sliders[index].value = value;
        _sendUpdate = true;
    }

    [PunRPC]
    void SyncWithInstance(Player player, int index)
    {
        _view.RPC("UpdateSliderValue", player, index, _sliders[index].value);
    }

    void SyncSliderToMaster(int index)
    {
        _view.RPC("SyncWithInstance", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, index);
    }
}
