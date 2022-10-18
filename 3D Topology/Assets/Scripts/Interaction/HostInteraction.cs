using EnvironmentGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Interaction
{
    /// <summary>
    /// An interactable host
    /// </summary>
    public class HostInteraction : Interactable
    {
        /// <summary>
        /// The text used to display informations about the host
        /// </summary>
        [SerializeField] private TMPro.TextMeshPro _textMeshPro;
        /// <summary>
        /// The sprite renderer that displays the icon of the host
        /// </summary>
        [SerializeField] private SpriteRenderer _spriteRenderer;
        /// <summary>
        /// Whether the text should follow the camera or not
        /// </summary>
        [SerializeField] bool _rotateText = true;
        HostNode _host;
        public bool AlertRaised
        {
            get => _host.AlertRaised;
        }

        public string Description
        {
            get => _textMeshPro.text;
            set => _textMeshPro.text = value;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _textMeshPro.enabled = false;
            _host = GetComponent<HostNode>();
        }

        // Update is called once per frame
        void Update()
        {
            //make the text face the camera
            if (_rotateText && _textMeshPro.enabled)
            {
                _textMeshPro.transform.LookAt(Camera.main.transform);
                _textMeshPro.transform.Rotate(0, 180, 0, Space.Self);
            }
        }

        public override void Activate(Vector3 activationLocation, Vector3 normal)
        {
            base.Activate(activationLocation, normal);
            _textMeshPro.enabled = true;
        }

        public override void CancelActivation()
        {
            base.CancelActivation();
            _textMeshPro.enabled = false;
        }

        /// <summary>
        /// Set the icon displayed by the host
        /// </summary>
        /// <param name="sprite">The icon to display</param>
        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}