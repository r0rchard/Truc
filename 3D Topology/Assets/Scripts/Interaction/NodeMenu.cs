using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    public class NodeMenu : MonoBehaviour
    {
        [SerializeField] TerminalHostInteraction _hostInteraction;
        [SerializeField] TMPro.TextMeshProUGUI _description;
        [SerializeField] CanvasGroup _homeGroup;
        [SerializeField] CanvasGroup _descriptionGroup;
        [SerializeField] CanvasGroup _alertGroup;
        [SerializeField] TMPro.TextMeshProUGUI _pipeButtonText;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_description.isActiveAndEnabled)
            {
                _description.text = _hostInteraction.Description;
            }
        }

        private void OnEnable()
        {
            if (_hostInteraction.AlertRaised)
            {
                _alertGroup.gameObject.SetActive(true);
                _homeGroup.gameObject.SetActive(false);
                _descriptionGroup.gameObject.SetActive(false);
            }
            else
            {
                _alertGroup.gameObject.SetActive(false);
                _homeGroup.gameObject.SetActive(true);
                _descriptionGroup.gameObject.SetActive(false);
            }
        }

        public void ShowInfos()
        {
            _homeGroup.gameObject.SetActive(false);
            _alertGroup.gameObject.SetActive(false);
            _description.text = _hostInteraction.Description;
            _descriptionGroup.gameObject.SetActive(true);
        }

        public void ReturnHome()
        {
            _descriptionGroup.gameObject.SetActive(false);
            _alertGroup.gameObject.SetActive(false);
            _homeGroup.gameObject.SetActive(true);
        }

        public void ChangePipeVisibility()
        {
            if (_hostInteraction.ChangePipeVisibility())
            {
                _pipeButtonText.text = "Hide connecting pipe";
            }
            else
            {
                _pipeButtonText.text = "Show connecting pipe";
            }
        }

        public void TeleportToDemoRoom()
        {
            DemoRoom demoRoom = FindObjectOfType<DemoRoom>();
            XRRig rig = FindObjectOfType<XRRig>();
            demoRoom.Door.TeleportPoint = rig.transform.position;
            rig.transform.position = demoRoom.SpawnPoint.position;
            rig.transform.forward = demoRoom.transform.forward;
        }
    }
}