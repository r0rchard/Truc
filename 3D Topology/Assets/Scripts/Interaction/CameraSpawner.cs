using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    public class CameraSpawner : MonoBehaviour
    {
        [SerializeField] InputActionReference _spawnButton;
        int _nbCameraSpawned = 0;

        // Start is called before the first frame update
        void Start()
        {
            _spawnButton.action.performed += SpawnCamera;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SpawnCamera(InputAction.CallbackContext obj)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonView view = PhotonNetwork.Instantiate("POVCamera", Camera.main.transform.position, Camera.main.transform.rotation).GetPhotonView();
                view.RPC("SetName", RpcTarget.All, view.Owner.NickName + " #"+ _nbCameraSpawned.ToString());
                _nbCameraSpawned += 1;
            }
        }
    }
}