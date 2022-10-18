using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parsing.CyberRangeSerialization;
using System.Net;
using System;
using EnvironmentGeneration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Parsing
{
    /// <summary>
    /// Class that reads a Cyber Range topology stored as a JSON text asset before invoking the environment creator
    /// </summary>
    public class JSONReader : MonoBehaviour
    {
        /// <summary>
        /// The JSON file to generate an environment from
        /// </summary>
        public TextAsset json;

        //singleton
        public static JSONReader Instance;

        HostsSpecsAsset[] _hosts;
        NetworksSpecsAsset[] _networks;

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

        public void GenerateEnvironment()
        {
            StartCoroutine(EnvironmentGenerationCoroutine());
        }

        IEnumerator EnvironmentGenerationCoroutine()
        {
            if (CyberRangeInterface.Instance.OnlineMode)
            {
                Thread hostThread = new Thread(delegate ()
                {
                    _hosts = CyberRangeInterface.Instance.GetAllHosts();
                });
                hostThread.Start();

                yield return new WaitUntil(() => !hostThread.IsAlive);

                Thread networkThread = new Thread(delegate ()
                {
                    _networks = CyberRangeInterface.Instance.GetAllNetworks();
                });
                networkThread.Start();

                yield return new WaitUntil(() => !networkThread.IsAlive);

                EnvironmentGenerator.Instance.GenerateEnvironment(_hosts, _networks);
            }
            else
            {
                TextAsset jsonFile = Resources.Load<TextAsset>("wz-14");
                WorkzoneAsset workzoneAsset = JsonUtility.FromJson<WorkzoneAsset>(jsonFile.text);
                _hosts = workzoneAsset.hosts;
                _networks = workzoneAsset.networks;
                GraphGenerator.Instance.GenerateEnvironment(_hosts, _networks);
            }
        }
    }
}