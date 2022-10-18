using DataTransmission;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class TerminalHostInteraction : HostInteraction
    {
        [SerializeField] NodeMenu _menu;

        private Pipeline _pipeline;
        public Pipeline Pipeline
        {
            get => _pipeline;
            set => _pipeline = value;
        }

        [SerializeField] InternalState _internalState;
        [SerializeField] List<Renderer> _renderers;

        protected override void Start()
        {
            base.Start();
            _pipeline.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            if (!_menu.isActiveAndEnabled)
            {
                _menu.transform.position = activationLocation;
                float angle;
                if (Vector3.Dot(_menu.transform.forward, normal) != 0)
                {
                    angle = (Mathf.Atan2(_menu.transform.forward.z, _menu.transform.forward.x) - Mathf.Atan2(normal.z, normal.x)) * Mathf.Rad2Deg;
                }
                else
                {
                    Vector3 cameraDirection = Camera.main.transform.position - _menu.transform.position;
                    angle = (Mathf.Atan2(_menu.transform.forward.z, _menu.transform.forward.x) - Mathf.Atan2(cameraDirection.z, cameraDirection.x)) * Mathf.Rad2Deg;
                }
                _menu.transform.Rotate(0, angle, 0);
                _menu.transform.position += _menu.transform.forward * 0.01f;
                _menu.gameObject.SetActive(true);
            }
            else
            {
                _menu.gameObject.SetActive(false);
            }
        }

        public override void CancelActivation()
        {

        }

        public bool ChangePipeVisibility()
        {
            _pipeline.gameObject.SetActive(!_pipeline.isActiveAndEnabled);
            return _pipeline.isActiveAndEnabled;
        }

        public void ShowOrHideHistograms()
        {
            if (_internalState.isActiveAndEnabled)
            {
                _internalState.gameObject.SetActive(false);
                foreach (Renderer renderer in _renderers)
                {
                    renderer.enabled = true;
                }
            }
            else
            {
                _internalState.gameObject.SetActive(true);
                foreach (Renderer renderer in _renderers)
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}