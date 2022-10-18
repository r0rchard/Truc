using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Interaction
{
    /// <summary>
    /// Abstract class for basic interaction
    /// </summary>
    [RequireComponent(typeof(Outline))]
    public class Interactable : MonoBehaviour
    {
        /// <summary>
        /// The outline to display when the object is hovered
        /// </summary>
        private Outline _outline;
        /// <summary>
        /// Color of the outline when the object is hovered
        /// </summary>
        [SerializeField] Color _hoverColor = Color.yellow;
        /// <summary>
        /// bool to activate or deactivate interaction with the object
        /// </summary>
        protected bool _canBeSelected = true;
        public bool Selectable
        {
            get => _canBeSelected;
            set => _canBeSelected = value;
        }

        /// <summary>
        /// bool to prevent an object from being grabbed
        /// </summary>
        [SerializeField] protected bool _canBeGrabbed = false;
        public bool Grabbable
        {
            get => _canBeGrabbed;
            set => _canBeGrabbed = value;
        }
        protected bool _isGrabbed;
        protected Transform _originalParent;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _outline = GetComponent<Outline>();
            _outline.OutlineWidth = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// The function to execute when interacting with the object
        /// </summary>
        public virtual void Activate(Vector3 activationLocation, Vector3 normal) { }

        public virtual void CancelActivation() { }

        /// <summary>
        /// The function to execute when the object is hovered
        /// </summary>
        public virtual void Select()
        {
            if (_canBeSelected)
            {
                _outline.OutlineColor = _hoverColor;
                _outline.OutlineWidth = 2;
            }
        }

        /// <summary>
        /// The function to execute when the object stops being hovered
        /// </summary>
        public virtual void Unselect()
        {
            _outline.OutlineWidth = 0;
        }

        /// <summary>
        /// Function allowing to move the object
        /// </summary>
        /// <param name="newParent">The new parent transform of the object</param>
        public virtual void Grab(Transform newParent, Vector3 grabPoint)
        {
            if (_canBeGrabbed && !_isGrabbed)
            {
                _originalParent = transform.parent;
                transform.parent = newParent;
                _isGrabbed = true;
            }
        }

        /// <summary>
        /// Function called to stop moving the object
        /// </summary>
        public virtual void Release()
        {
            if (_canBeGrabbed)
            {
                transform.parent = _originalParent;
                _isGrabbed = false;
            }
        }
    }
}