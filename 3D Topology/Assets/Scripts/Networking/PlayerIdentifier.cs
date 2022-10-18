using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking {

    /// <summary>
    /// Class allowing to identify a player on the network
    /// </summary>
    public class PlayerIdentifier : MonoBehaviour
    {
        PhotonView _view;
        public PhotonView View
        {
            get => _view;
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
    }
}