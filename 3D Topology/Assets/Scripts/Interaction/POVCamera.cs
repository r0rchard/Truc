using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    [RequireComponent(typeof(PhotonView))]
    public class POVCamera : Interactable
    {
        XRRig _rig;
        string _name;
        public string Name
        {
            get => _name;
        }
        PhotonView _view;
        public PhotonView View
        {
            get => _view;
        }
        [SerializeField] List<Renderer> _renderersToColor = new List<Renderer>();
        [SerializeField] Camera _camera;
        [SerializeField] List<Renderer> _texturePlanes;
        [SerializeField] RenderTexture _referenceTexture;
        ActionBasedSnapTurnProvider _turnProvider;

        protected override void Start()
        {
            base.Start();
            _canBeGrabbed = true;
            _rig = FindObjectOfType<XRRig>();
            _view = GetComponent<PhotonView>();
            foreach (Renderer renderer in _renderersToColor)
            {
                renderer.material.color = ColorScheme.Instance.GetPlayerColor(_view.Owner.ActorNumber);
            }

            RenderTexture renderTexture = new RenderTexture(_referenceTexture);
            foreach(Renderer renderer in _texturePlanes)
            {
                renderer.material.SetTexture("_MainTex", renderTexture);
            }
            _camera.targetTexture = renderTexture;
            StartCoroutine(WaitTwoFrames());

            if (!_view.IsMine)
            {
                FindObjectOfType<Assistant>().OnCameraAdded();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            _rig.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            _rig.transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
        }

        [PunRPC]
        public void SetName(string name)
        {
            _name = name;
        }

        public override void Grab(Transform newParent, Vector3 grabPoint)
        {
            base.Grab(newParent, grabPoint);
            _camera.enabled = true;
            if(_turnProvider == null)
            {
                _turnProvider = FindObjectOfType<ActionBasedSnapTurnProvider>();
            }
            _turnProvider.enableTurnAround = false;
        }

        public override void Release()
        {
            base.Release();
            _camera.enabled = false;
            _turnProvider.enableTurnAround = true;
        }

        IEnumerator WaitTwoFrames()
        {
            for(int i = 0; i < 2; i++)
            {
                yield return null;
            }
            _camera.enabled = false;
        }
    }
}