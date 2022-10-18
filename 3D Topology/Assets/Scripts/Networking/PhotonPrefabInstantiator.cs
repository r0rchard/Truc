using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// A class allowing to instantiate a prefab on the network when a room is joined
    /// </summary>
    public class PhotonPrefabInstantiator : MonoBehaviourPunCallbacks
    {
        [SerializeField] bool _keepVisible = true;
        [SerializeField] GameObject _prefab;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnJoinedRoom()
        {
            GameObject prefab = PhotonNetwork.Instantiate(_prefab.name, transform.position, transform.rotation);
            prefab.transform.SetParent(transform);
            if (!_keepVisible)
            {
                ChangeLayerRecursively(transform, "NotVisibleInLocal");
            }
        }

        void ChangeLayerRecursively(Transform trans, string name)
        {
            foreach (Transform child in trans)
            {
                child.gameObject.layer = LayerMask.NameToLayer(name);
                ChangeLayerRecursively(child, name);
            }
        }
    }
}