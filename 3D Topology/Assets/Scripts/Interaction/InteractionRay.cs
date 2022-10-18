using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;


namespace Interaction
{
    /// <summary>
    /// A ray attached to a controller to interact with the world's objects
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class InteractionRay : InteractionTool
    {
        /// <summary>
        /// The visual representation of the ray
        /// </summary>
        LineRenderer _line;
        /// <summary>
        /// Max interaction distance of the ray
        /// </summary>
        public float maxDistance = 20;
        /// <summary>
        /// The action to perform to interact with a hovered object
        /// </summary>
        [SerializeField] InputActionReference _activateActionReference;
        /// <summary>
        /// The action to perform to grab a hovered object
        /// </summary>
        [SerializeField] InputActionReference _grabActionReference;
        /// <summary>
        /// The action to perform to move to a certain point while hovering the floor
        /// </summary>
        [SerializeField] InputActionReference _moveActionReference;
        /// <summary>
        /// The action to perform to change the distance of a grabbed object
        /// </summary>
        [SerializeField] InputActionReference _moveGrabbedActionReference;
        /// <summary>
        /// The currently selected game object
        /// </summary>
        private Interactable _selected;
        /// <summary>
        /// The currently grabbed game object
        /// </summary>
        private Interactable _grabbed;
        /// <summary>
        /// bool disabling interaction, mainly when hovering UI
        /// </summary>
        private bool _interactionDisabled;
        /// <summary>
        /// The position of the end of the ray
        /// </summary>
        private Vector3 _rayPosition;
        public Vector3 RayPosition
        {
            get => _rayPosition;
        }
        /// <summary>
        /// The normal of the surface at the end of the ray
        /// </summary>
        private Vector3 _rayNormal;
        /// <summary>
        /// The other ray, used to interact with UI
        /// </summary>
        [SerializeField] XRRayInteractor _uiRay;

        bool _movementInitiated = false;

        // Start is called before the first frame update
        void Start()
        {
            //linking actions to functions
            _activateActionReference.action.performed += Activate;
            _activateActionReference.action.canceled += CancelActivation;

            _grabActionReference.action.performed += Grab;
            _grabActionReference.action.canceled += Release;

            _moveGrabbedActionReference.action.performed += MoveGrabbed;

            _moveActionReference.action.performed += StartMoving;
            _moveActionReference.action.canceled += Move;

            //line renderer set up
            _line = GetComponent<LineRenderer>();
            _line.SetPosition(0, transform.position);
            _line.SetPosition(0, transform.position + transform.forward * maxDistance);
            _line.startWidth = 0.05f;
            _line.endWidth = 0.05f;
        }

        // Update is called once per frame
        void Update()
        {
            _interactionDisabled = false;

            //verification that we are not currently hovering a UI
            if (_uiRay != null)
            {
                _ = _uiRay.TryGetHitInfo(out _, out _, out _, out _interactionDisabled);
            }
            if (!_interactionDisabled)
            {
                //updating position of the begining of the line
                _line.SetPosition(0, transform.position);
                //raycast to see if there's anything to interact with
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
                {
                    _rayPosition = hit.point;
                    _rayNormal = hit.normal;
                    _line.SetPosition(1, hit.point);

                    //if we're grabbing something, it stays selected
                    if (!_grabbed)
                    {

                        //can only interact with interactables
                        GameObject hitObject = hit.collider.gameObject;
                        Interactable interactable = hitObject.GetComponent<Interactable>();

                        while ((interactable == null || !interactable.isActiveAndEnabled) && hitObject.transform.parent != null)
                        {
                            hitObject = hitObject.transform.parent.gameObject;
                            interactable = hitObject.GetComponent<Interactable>();
                        }
                        if (interactable && interactable.isActiveAndEnabled)
                        {
                            if (_selected != interactable)
                            {
                                if (_selected)
                                {
                                    _selected.Unselect();
                                }
                                _selected = interactable;
                                _selected.Select();
                            }

                        }
                        else if (_selected)
                        {
                            _selected.Unselect();
                            _selected = null;
                        }
                    }
                }
                //position end of line at max interaction distance
                else
                {
                    _line.SetPosition(1, transform.position + transform.forward * maxDistance);
                    _rayPosition = _line.GetPosition(1);
                    _rayNormal = Vector3.zero;
                    if (_selected && !_grabbed)
                    {
                        _selected.Unselect();
                        _selected = null;
                    }
                }
            }
            //if hovering UI, hide interaction ray
            else
            {
                _line.SetPosition(0, transform.position);
                _line.SetPosition(1, transform.position);
                _rayPosition = _line.GetPosition(1);
                _rayNormal = Vector3.zero;
            }
        }

        /// <summary>
        /// Function called to interact with an object
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void Activate(InputAction.CallbackContext obj)
        {
            if (!_interactionDisabled && _selected)
            {
                _selected.Activate(_rayPosition, _rayNormal);
            }
        }

        /// <summary>
        /// Function called to stop interacting with an object
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void CancelActivation(InputAction.CallbackContext obj)
        {
            if (!_interactionDisabled && _selected)
            {
                _selected.CancelActivation();
            }
        }

        /// <summary>
        /// Function called to grab an object
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void Grab(InputAction.CallbackContext obj)
        {
            if (!_interactionDisabled && _selected)
            {
                if (_selected.Grabbable)
                {
                    _selected.Grab(transform, _rayPosition);
                    _grabbed = _selected;
                }
            }
        }

        /// <summary>
        /// Function called to stop grabbing an object
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void Release(InputAction.CallbackContext obj)
        {
            if (!_interactionDisabled && _grabbed)
            {
                _grabbed.Release();
                _grabbed = null;
            }
        }

        /// <summary>
        /// Function called to move a grabbed object closer or further from the controller
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void MoveGrabbed(InputAction.CallbackContext obj)
        {
            if (!_interactionDisabled && _grabbed)
            {
                if (obj.ReadValue<Vector2>().y > 0 || Vector3.Dot(_grabbed.transform.position - transform.position, transform.forward) > 0)
                {
                    _grabbed.transform.Translate(transform.forward * obj.ReadValue<Vector2>().y * 0.1f, Space.World);
                }
            }
        }

        /// <summary>
        /// Function called to teleport to somewhere else on the floor
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void StartMoving(InputAction.CallbackContext obj)
        {
            if (!_interactionDisabled && _selected is FloorInteraction)
            {
                _movementInitiated = true;
                GetComponent<LineRenderer>().startColor = Color.yellow;
                GetComponent<LineRenderer>().endColor = Color.yellow;
            }
        }

        /// <summary>
        /// Function called to teleport to somewhere else on the floor
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        private void Move(InputAction.CallbackContext obj)
        {
            if (_movementInitiated && !_interactionDisabled && _selected is FloorInteraction)
            {
                GetComponent<LineRenderer>().startColor = Color.white;
                GetComponent<LineRenderer>().endColor = Color.white;
                transform.root.position = _rayPosition;
            }
            _movementInitiated = false;
        }

        private void OnDisable()
        {
            if (_line)
            {
                _line.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_line)
            {
                _line.enabled = true;
            }
        }
    }
}