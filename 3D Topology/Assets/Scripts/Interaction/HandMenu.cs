using Networking;
using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interaction
{
    public class HandMenu : MonoBehaviour
    {
        [SerializeField] GameObject _menu;
        [SerializeField] InputActionReference _menuActionReference;
        [SerializeField] Recorder _recorder;
        [SerializeField] TMPro.TextMeshProUGUI _micButtonText;
        [SerializeField] Button _buttonPrefab;
        [SerializeField] XRRig _rig;
        [SerializeField] VerticalLayoutGroup _homeGroup;
        [SerializeField] VerticalLayoutGroup _teleportationGroup;
        [SerializeField] VerticalLayoutGroup _teleportationButtonGroup;
        [SerializeField] VerticalLayoutGroup _povCameraGroup;
        [SerializeField] VerticalLayoutGroup _povCameraButtonGroup;
        [SerializeField] VerticalLayoutGroup _soundGroup;
        [SerializeField] ColorScheme _colorScheme;
        [SerializeField] AudioMixer _audioMixer;
        List<Button> _playerButtons = new List<Button>();
        List<Button> _povCameraButtons = new List<Button>();

        // Start is called before the first frame update
        void Start()
        {
            _menuActionReference.action.performed += ShowOrHideMenu;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ShowOrHideMenu(InputAction.CallbackContext obj)
        {
            _menu.SetActive(!_menu.activeInHierarchy);
        }

        public void CutOrActivateMic()
        {
            _recorder.TransmitEnabled = !_recorder.TransmitEnabled;
            if (_recorder.TransmitEnabled)
            {
                _micButtonText.text = "Cut mic";
            }
            else
            {
                _micButtonText.text = "Activate mic";
            }
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void GeneratePlayerButtons()
        {
            int i = 0;
            foreach(PlayerIdentifier player in FindObjectsOfType<PlayerIdentifier>())
            {
                if (!player.View.IsMine)
                {
                    Color color = ColorScheme.Instance.GetPlayerColor(player.View.Owner.ActorNumber);
                    Button button = Instantiate(_buttonPrefab, _teleportationButtonGroup.transform);
                    button.GetComponent<Image>().color = color;
                    _playerButtons.Add(button);
                    button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = player.View.Owner.NickName;
                    button.onClick.AddListener(()=> TeleportToPlayer(player));
                    i += 1;
                }
            }
        }

        private void GeneratePOVCameraButtons()
        {
            int i = 0;
            foreach (POVCamera camera in FindObjectsOfType<POVCamera>())
            {
                Color color = ColorScheme.Instance.GetPlayerColor(camera.View.Owner.ActorNumber);
                Button button = Instantiate(_buttonPrefab, _povCameraButtonGroup.transform);
                Debug.Log(color);
                button.GetComponent<Image>().color = color;
                _povCameraButtons.Add(button);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = camera.Name;
                button.onClick.AddListener(() => camera.Activate(Vector3.zero, Vector3.zero));
                i += 1;
            }
        }

        public void TeleportToPlayer(PlayerIdentifier player)
        {
            _rig.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            _rig.transform.rotation = Quaternion.Euler(new Vector3(0, player.transform.rotation.y, 0));
        }

        public void GoToPOVCameras()
        {
            _homeGroup.gameObject.SetActive(false);
            _povCameraGroup.gameObject.SetActive(true);
            GeneratePOVCameraButtons();
        }

        public void GoToTeleportation()
        {
            _homeGroup.gameObject.SetActive(false);
            _teleportationGroup.gameObject.SetActive(true);
            GeneratePlayerButtons();
        }

        public void GoToSound()
        {
            _homeGroup.gameObject.SetActive(false);
            _soundGroup.gameObject.SetActive(true);
        }

        public void ShowOrHideHistograms()
        {
            foreach(TerminalHostInteraction host in FindObjectsOfType<TerminalHostInteraction>())
            {
                host.ShowOrHideHistograms();
            }
        }

        public void QuitTeleportation()
        {
            foreach(Button button in _playerButtons)
            {
                Destroy(button.gameObject); 
            }
            _playerButtons = new List<Button>();
            _teleportationGroup.gameObject.SetActive(false);
            _homeGroup.gameObject.SetActive(true);
        }

        public void QuitPOVCameras()
        {
            foreach (Button button in _povCameraButtons)
            {
                Destroy(button.gameObject);
            }
            _povCameraButtons = new List<Button>();
            _povCameraGroup.gameObject.SetActive(false);
            _homeGroup.gameObject.SetActive(true);
        }

        public void QuitSound()
        {
            _soundGroup.gameObject.SetActive(false);
            _homeGroup.gameObject.SetActive(true);
        }

        public void TeleportToRennes()
        {
            FindObjectOfType<RennesBuilding>().TeleportToEntry();
        }

        public void ChangeVoiceVolume(float sliderValue)
        {
            _audioMixer.SetFloat("VoicesVolume", Mathf.Log10(sliderValue) * 20);
        }

        public void ChangeEnvironmentVolume(float sliderValue)
        {
            _audioMixer.SetFloat("EnvironmentVolume", Mathf.Log10(sliderValue) * 20);
        }
    }
}