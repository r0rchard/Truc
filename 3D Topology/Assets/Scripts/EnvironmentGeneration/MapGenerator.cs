using DataTransmission;
using Interaction;
using Parsing.CyberRangeSerialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace EnvironmentGeneration
{
    /// <summary>
    /// A class used to generate a map of buttons for teleporation
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {

        //singleton
        public static MapGenerator Instance;
        /// <summary>
        /// The rig to follow
        /// </summary>
        [SerializeField] XRRig _rig;
        /// <summary>
        /// Visual indicator to know where the user is on the map
        /// </summary>
        [SerializeField] YouAreHereIndicator _indicator;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);

            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Creates a miniature representation of an environment that can be used to teleport around
        /// </summary>
        /// <param name="environment">The environment to make a map for</param>
        public void SetUpMiniMap(Environment environment)
        {
            Environment miniEnv = Instantiate(environment);
            Destroy(miniEnv.GetComponent<Photon.Pun.PhotonView>());
            Instantiate(_indicator, miniEnv.transform).Rig = _rig;

            miniEnv.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

            foreach (Interactable interactable in miniEnv.GetComponentsInChildren<Interactable>())
            {
                Destroy(interactable);
            }

            Pipeline[] mainPipelines = environment.GetComponentsInChildren<Pipeline>();
            Pipeline[] miniPipelines = miniEnv.GetComponentsInChildren<Pipeline>();

            for (int i = 0; i < mainPipelines.Length; i++)
            {
                mainPipelines[i].Linked = miniPipelines[i];
            }

            for (int i = 0; i < environment.hosts.Count; i++)
            {
                miniEnv.hosts[i].Linked = environment.hosts[i];
            }

            for (int i = 0; i < environment.networks.Count; i++)
            {
                miniEnv.networks[i].Linked = environment.networks[i];
            }

            for (int i = 0; i < miniEnv.networks.Count; i++)
            {
                MapInteractable interactable = miniEnv.networks[i].gameObject.AddComponent<MapInteractable>();
                interactable.TeleportSpot = environment.networks[i].transform;
            }

            for (int i = 0; i < miniEnv.hosts.Count; i++)
            {
                if (environment.hosts[i].Connections.Count > 1)
                {
                    MapInteractable interactable = miniEnv.hosts[i].gameObject.AddComponent<MapInteractable>();
                    interactable.TeleportSpot = environment.hosts[i].transform;
                }
            }

            foreach (var mapDisplayer in FindObjectsOfType<MapDisplayer>())
            {
                mapDisplayer._map = miniEnv.gameObject;
            }

            StartCoroutine(LetMapInitialize(miniEnv));
        }

        IEnumerator LetMapInitialize(Environment miniEnv)
        {
            yield return null;
            miniEnv.gameObject.SetActive(false);
        }
    }
}