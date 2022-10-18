using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Interaction
{
    /// <summary>
    /// Class used to display or hide the map
    /// </summary>
    public class MapDisplayer : MonoBehaviour
    {
        /// <summary>
        /// The map to display
        /// </summary>
        public GameObject _map;
        /// <summary>
        /// The position at which the map should appear
        /// </summary>
        [SerializeField] Transform _mapTransform;
        List<MeshRenderer> _toReennable;
        public Transform MapTransform
        {
            get => _mapTransform;
        }
        /// <summary>
        /// The action to perform to display the map
        /// </summary>
        public InputActionReference showMapActionReference;

        // Start is called before the first frame update
        void Start()
        {
            //linking actions to displaying or hiding the map
            showMapActionReference.action.performed += ShowMap;
            showMapActionReference.action.canceled += HideMap;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Displays the map
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        public void ShowMap(InputAction.CallbackContext obj)
        {
            _toReennable = new List<MeshRenderer>();
            foreach (MeshRenderer mesh in transform.parent.GetComponentsInChildren<MeshRenderer>())
            {
                if (mesh.enabled)
                {
                    _toReennable.Add(mesh);
                    mesh.enabled = false;
                }
            }
            _map.SetActive(true);
            _map.transform.SetParent(transform);
            _map.transform.localPosition = Vector3.zero;
            _map.transform.localRotation = Quaternion.identity;
            GetComponent<InteractionRay>().enabled = false;
        }

        /// <summary>
        /// Hides the map
        /// </summary>
        /// <param name="obj">Callback context of the action</param>
        public void HideMap(InputAction.CallbackContext obj)
        {
            foreach (MeshRenderer mesh in _toReennable)
            {
                mesh.enabled = true;
            }
            _map.SetActive(false);
            GetComponent<InteractionRay>().enabled = true;
        }
    }
}