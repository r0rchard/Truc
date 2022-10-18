using Interaction;
using Parsing.CyberRangeSerialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace EnvironmentGeneration
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(HostNode), true)]
    public class HostNodeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            HostNode host = (HostNode)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Alert"))
            {
                AlertManager.Instance.NetworkRaiseAlert(host);
            }

            if (GUILayout.Button("Stop Alert"))
            {
                AlertManager.Instance.NetworkStopAlert(host);
            }
        }
    }
    #endif

    /// <summary>
    /// A generic class describing a host
    /// </summary>
    public class HostNode : AbstractNode
    {
        HostsSpecsAsset _hostAsset;
        public HostsSpecsAsset HostAsset
        {
            get => _hostAsset;
            set => _hostAsset = value;
        }

        string _identifier;
        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        List<IPAddress> _ipAdresses = new List<IPAddress>();
        public List<IPAddress> IpAdresses
        {
            get => _ipAdresses;
        }

        Dictionary<string, NetworkNode> _connections = new Dictionary<string, NetworkNode>();
        public Dictionary<string, NetworkNode> Connections
        {
            get => _connections;
        }

        HostInteraction _interaction;
        public HostInteraction Interaction
        {
            get => _interaction;
            set => _interaction = value;
        }

        private HostNode _linked;
        public HostNode Linked
        {
            get => _linked;
            set => _linked = value;
        }

        private bool _alertRaised = false;
        public bool AlertRaised
        {
            get => _alertRaised;
        }

        [SerializeField] InternalState _internalState;
        public InternalState InternalState
        {
            get => _internalState;
        }

        [SerializeField] UnityEngine.UI.Image _powerButtonImage;

        /// <summary>
        /// Adds a network to the list of connection of the host
        /// </summary>
        /// <param name="network">The network to add</param>
        public void AddConnection(NetworkNode network, string identifier)
        {
            _connections.Add(identifier, network);
        }

        public void RaiseAlert()
        {
            _alertRaised = true;
            StartCoroutine(Alert());
        }

        IEnumerator Alert()
        {
            if (_interaction is TerminalHostInteraction terminalHost)
            {
                terminalHost.ChangePipeVisibility();
            }
            Assistant assistant = FindObjectOfType<Assistant>();
            yield return assistant.AlertNotification();
            assistant.StartGuidingTo(Connections.Values.First());
            Renderer renderer = GetComponent<Renderer>();
            if (renderer)
            {
                Color originalColor = renderer.material.color;
                while (_alertRaised)
                {
                    renderer.material.color = Color.red;
                    yield return new WaitForSeconds(0.5f);
                    renderer.material.color = originalColor;
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        public void StopAlert()
        {
            _alertRaised = false;
        }

        public void ToggleHost()
        {
            StartCoroutine(Toggling());
        }

        IEnumerator Toggling()
        {
            if (CyberRangeInterface.Instance.OnlineMode)
            {
                bool isOn = false;
                Thread thread = new Thread(delegate ()
                {
                    isOn = CyberRangeInterface.Instance.ToggleHost(_identifier);
                });
                thread.Start();
                yield return new WaitUntil(() => !thread.IsAlive);
                if (isOn)
                {
                    _powerButtonImage.color = Color.green;
                }
                else
                {
                    _powerButtonImage.color = Color.red;
                }
            }
            else if(_powerButtonImage.color == Color.red)
            {
                _powerButtonImage.color = Color.green;
            }
            else
            {
                _powerButtonImage.color = Color.red;
            }
        }
    }
}