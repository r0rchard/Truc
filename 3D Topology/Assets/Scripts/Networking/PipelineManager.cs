using DataTransmission;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// The class that handles changes with pipelines to send them to every client
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PipelineManager : MonoBehaviour, IPunInstantiateMagicCallback
    {
        //singleton
        public static PipelineManager Instance;

        /// <summary>
        /// The managed pipelines that can be updated
        /// </summary>
        private Dictionary<string, Pipeline> _pipelines = new Dictionary<string, Pipeline>();

        /// <summary>
        /// The view to synchronise this manager with others on the network
        /// </summary>
        private PhotonView _view;


        // Start is called before the first frame update
        void Start()
        {
            _view = GetComponent<PhotonView>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //singleton
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);

            Instance = this;
        }


        /// <summary>
        /// Allows the manager to manage a new pipepline
        /// </summary>
        /// <param name="pipeline">The new pipeline to manage</param>
        public void AddPipeline(Pipeline pipeline, string identifier)
        {
            _pipelines.Add(identifier, pipeline);
        }

        /// <summary>
        /// Synchronises the pipeline of a player to this instance
        /// </summary>
        /// <param name="player">The player whose pipeline will be synchronised</param>
        /// <param name="identifier">The identifier of the pipeline to synchronise</param>
        [PunRPC]
        public void SyncPipe(Player player, string identifier)
        {
            _view.RPC("UpdateSpeed", player, identifier, _pipelines[identifier].TransferSpeed);
            _view.RPC("UpdateSize", player, identifier, _pipelines[identifier].TransferSize);
            if (_pipelines[identifier].ColorOverriden)
            {
                _view.RPC("UpdateColor", player, identifier, new Vector3(_pipelines[identifier].Color.r, _pipelines[identifier].Color.g, _pipelines[identifier].Color.b));
            }
            if (_pipelines[identifier].HeightOverriden)
            {
                _view.RPC("UpdateHostHeight", player, identifier, _pipelines[identifier].HostHeight);
                _view.RPC("UpdateNetworkHeight", player, identifier, _pipelines[identifier].NetworkHeight);
            }
        }

        /// <summary>
        /// Allows to synchronise a player with the master
        /// </summary>
        /// <param name="identifier">The identifier of the pipeline to synchronise</param>
        public void SyncPipeToMaster(string identifier)
        {
            _view.RPC("SyncPipe", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, identifier);
        }

        /// <summary>
        /// Synnchronises all players with this instance
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to synchronise</param>
        public void OverridePipeForEveryone(string identifier)
        {
            _view.RPC("UpdateSpeed", RpcTarget.Others, identifier, _pipelines[identifier].TransferSpeed);
            _view.RPC("UpdateSize", RpcTarget.Others, identifier, _pipelines[identifier].TransferSize);
        }

        /// <summary>
        /// Override the color of a pipeline for everyone
        /// </summary>
        /// <param name="color">The new color of the pipeline</param>
        /// <param name="identifier">The identifier of the pipe to synchronise</param>
        public void OverrideColor(string identifier, Color color)
        {
            _view.RPC("UpdateColor", RpcTarget.All, identifier, new Vector3(color.r, color.g, color.b));
        }

        /// <summary>
        /// Resets the color of a pipeline for everyone
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to synchronise</param>
        public void NetworkResetColor(string identifier)
        {
            _view.RPC("ResetColor", RpcTarget.All, identifier);
        }

        /// <summary>
        /// Updates the transfer speed of a pipeline
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to update</param>
        /// <param name="newSpeed">The new transfer speed of the pipeline</param>
        [PunRPC]
        public void UpdateSpeed(string identifier, float newSpeed)
        {
            _pipelines[identifier].TransferSpeed = newSpeed;
        }

        /// <summary>
        /// Updates the transfer size of a pipeline
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to update</param>
        /// <param name="newSize">The new transfer size of the pipeline</param>
        [PunRPC]
        public void UpdateSize(string identifier, float newSize)
        {
            _pipelines[identifier].TransferSize = newSize;
        }

        /// <summary>
        /// Updates the color of a pipeline
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to update</param>
        /// <param name="color">The new transfer color of the pipeline</param>
        [PunRPC]
        void UpdateColor(string identifier, Vector3 color)
        {
            _pipelines[identifier].OverrideColor(new Color(color.x, color.y, color.z));
        }


        /// <summary>
        /// Resets the color of a pipeline
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to reset the color of</param>
        [PunRPC]
        void ResetColor(string identifier)
        {
            _pipelines[identifier].ResetColor();
        }

        [PunRPC]
        void UpdateHostHeight(string identifier, float newHeight)
        {
            _pipelines[identifier].HostHeight = newHeight;
            _pipelines[identifier].ModifyPrefabMeshes();
        }

        [PunRPC]
        void UpdateNetworkHeight(string identifier, float newHeight)
        {
            _pipelines[identifier].NetworkHeight = newHeight;
            _pipelines[identifier].ModifyPrefabMeshes();
        }

        public void NetworkUpdateHostHeight(string identifier, float newHeight)
        {
            _view.RPC("UpdateHostHeight", RpcTarget.All, identifier, newHeight);
        }

        public void NetworkUpdateNetworkHeight(string identifier, float newHeight)
        {
            _view.RPC("UpdateNetworkHeight", RpcTarget.All, identifier, newHeight);
        }

        /// <summary>
        /// Resets the height of a pipeline
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to reset the color of</param>
        [PunRPC]
        void ResetHeight(string identifier)
        {
            _pipelines[identifier].ResetHeight();
        }

        /// <summary>
        /// Resets the height of a pipeline for everyone
        /// </summary>
        /// <param name="identifier">The identifier of the pipe to synchronise</param>
        public void NetworkResetHeight(string identifier)
        {
            _view.RPC("ResetHeight", RpcTarget.All, identifier);
        }

        [PunRPC]
        void RemovePipeline(string identifier)
        {
            Destroy(_pipelines[identifier].gameObject);
            _pipelines.Remove(identifier);
        }

        public void NetworkRemovePipeline(string identifier)
        {
            _view.RPC("RemovePipeline", RpcTarget.All, identifier);
        }
    }
}