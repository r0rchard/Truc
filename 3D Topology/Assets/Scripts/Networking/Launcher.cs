using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using Parsing;
using VRKeys;
using Interaction;

namespace Networking
{
    /// <summary>
    /// The class that handles the connection to the server
    /// </summary>
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields
        [SerializeField] Keyboard _keyboard;
        [SerializeField] InteractionRay _leftRay;
        [SerializeField] InteractionRay _rightRay;
        [SerializeField] GameObject _leftMallet;
        [SerializeField] GameObject _rightMallet;
        [SerializeField] DemoRoom _demoRoom;
        public bool OnlineMode;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";
        string _username;


        #endregion

        #region MonoBehaviourPunCallbacks Callbacks


        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinRandomRoom();
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions());
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            PhotonNetwork.LocalPlayer.NickName = _username;
            if (!PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WaitForPipelineManager());
            }
            else
            {
                JSONReader.Instance.GenerateEnvironment();
            }
            _demoRoom.gameObject.SetActive(true);
        }


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            //initialising Cyber Range interface in main thread to get Application.dataPath
            CyberRangeInterface instance = CyberRangeInterface.Instance;
            instance.OnlineMode = OnlineMode;
            ChangeRigInteractors();
            Connect();
        }


        #endregion


        #region Public Methods

        public void ValidateUserName(string username)
        {
            if (username != "")
            {
                _username = username;
                ChangeRigInteractors();
                Connect();
                _keyboard.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }


        #endregion

        private void ChangeRigInteractors()
        {
            _leftMallet.SetActive(false);
            _rightMallet.SetActive(false);
            _leftRay.gameObject.SetActive(true);
            _rightRay.gameObject.SetActive(true);
        }

        IEnumerator WaitForPipelineManager()
        {
            while(PipelineManager.Instance == null)
            {
                yield return null;
            }
            JSONReader.Instance.GenerateEnvironment();
        }
    }
}