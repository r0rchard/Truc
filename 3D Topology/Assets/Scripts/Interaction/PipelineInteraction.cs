using DataTransmission;
using HSVPicker;
using Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(Pipeline))]
    public class PipelineInteraction : Interactable
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] TMPro.TextMeshPro _tmPro;
        Pipeline _pipeline;
        ColorPicker _picker;
        InteractionRay _rayToFollow;
        bool _makeHostFollow;
        float _originalHostHeight;
        float _originalNetworkHeight;


        protected override void Start()
        {
            base.Start();
            _pipeline = GetComponent<Pipeline>();
            _originalHostHeight = _pipeline.HostHeight;
            _originalNetworkHeight = _pipeline.NetworkHeight;
        }

        // Update is called once per frame
        void Update()
        {
            if (_canvas.isActiveAndEnabled)
            {
                _tmPro.text = "Transfer speed : " + _pipeline.TransferSpeed.ToString()
                    + "\nTransfer size : " + _pipeline.TransferSize.ToString();
            }
            if (_rayToFollow)
            {
                InclinePipeLine();
            }
        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            if (_canvas.isActiveAndEnabled)
            {
                _canvas.gameObject.SetActive(false);
            }
            else
            {
                _canvas.transform.position = activationLocation;
                Vector3 direction = _canvas.transform.position - Camera.main.transform.position;
                direction.y = 0;
                _canvas.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                _canvas.gameObject.SetActive(true);
            }
        }

        public override void Grab(Transform newParent, Vector3 grabPoint)
        {
            if (_canBeGrabbed && !_isGrabbed)
            {
                _isGrabbed = true;
                _rayToFollow = newParent.GetComponent<InteractionRay>();
                _makeHostFollow = (grabPoint - _pipeline.Host.transform.position).magnitude < (grabPoint - _pipeline.Network.transform.position).magnitude;
                Vector3 pipeDirection = (_pipeline.Network.transform.position - _pipeline.Host.transform.position).normalized;
                Vector3 projectedGrab = Vector3.Dot(grabPoint, pipeDirection) * pipeDirection;
            }
        }

        public override void Release()
        {
            _isGrabbed = false;
            _rayToFollow = null;
        }

        private void InclinePipeLine()
        {
            Vector3 normalVector = Vector3.Cross(transform.up, Vector3.up);

            float d = Vector3.Dot(_pipeline.Host.transform.position - _rayToFollow.RayPosition, normalVector);
            float denominator = Vector3.Dot(_rayToFollow.transform.forward, normalVector);

            if (denominator != 0)
            {
                d /= denominator;
            }
            else 
            { 
                return;
            }

            Vector3 intersection = _rayToFollow.RayPosition + _rayToFollow.transform.forward * d;

            if (_makeHostFollow)
            {
                Vector3 slopeDirection = intersection - _pipeline.Network.transform.position;
                float slope = slopeDirection.y - _pipeline.NetworkHeight;
                Vector3 hostToNetwork = _pipeline.Network.transform.position - _pipeline.Host.transform.position;
                slopeDirection.y = 0;
                slope /= slopeDirection.magnitude;

                float newHeight = hostToNetwork.magnitude * slope + _pipeline.NetworkHeight;

                if (newHeight < 10 && newHeight > 0)
                {
                    PipelineManager.Instance.NetworkUpdateHostHeight(_pipeline.Identifier, newHeight);
                }
            }
            else
            {
                Vector3 slopeDirection = intersection - _pipeline.Host.transform.position;
                float slope = slopeDirection.y - _pipeline.HostHeight;
                Vector3 networkToHost = _pipeline.Host.transform.position - _pipeline.Network.transform.position;
                slopeDirection.y = 0;
                slope /= slopeDirection.magnitude;

                float newHeight = networkToHost.magnitude * slope + _pipeline.HostHeight;

                if (newHeight < 10 && newHeight > 0)
                {
                    PipelineManager.Instance.NetworkUpdateNetworkHeight(_pipeline.Identifier, newHeight);
                }
            }
        }

        public void ResetPipelineHeight()
        {
            PipelineManager.Instance.NetworkResetHeight(_pipeline.Identifier);
        }
    }
}