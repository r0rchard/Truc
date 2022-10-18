using EnvironmentGeneration;
using HSVPicker;
using Networking;
using Photon.Pun;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DataTransmission
{

    #if UNITY_EDITOR
    [CustomEditor(typeof(Pipeline))]
    public class PipelineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Pipeline pipeline = (Pipeline)target;
            DrawDefaultInspector();
            if (Application.isPlaying)
            {
                pipeline.TransferSize = EditorGUILayout.FloatField("Transfer Size", pipeline.TransferSize);
                pipeline.TransferSpeed = EditorGUILayout.FloatField("Transfer Speed", pipeline.TransferSpeed);
                pipeline.NetworkHeight = EditorGUILayout.FloatField("Network Height", pipeline.NetworkHeight);
                pipeline.HostHeight = EditorGUILayout.FloatField("Host Height", pipeline.HostHeight);
            }
        }
    }
    #endif

    /// <summary>
    /// Class reprenseting the connection between a network and a host.
    /// Prefabs move along a cylinder to represent data transfer
    /// </summary>
    public class Pipeline : MonoBehaviour
    {

        /// <summary>
        /// 1 if the transfer is from host to network and -1 if it's going the other way
        /// </summary>
        [SerializeField, HideInInspector] int _transferDirection = 1;

        [SerializeField, HideInInspector] float _transferSize = 2;
        public float TransferSize
        {
            get => _transferSize;
            set
            {
                _transferSize = value;
                SetColor();
                if (Linked != null)
                {
                    Linked.TransferSize = value;
                }
            }
        }
        [SerializeField, HideInInspector] float _transferSpeed = 10f;
        public float TransferSpeed
        {
            get => _transferSpeed;
            set
            {
                _transferSpeed = value;
                SetColor();
                if (Linked != null)
                {
                    Linked.TransferSpeed = value;
                }
                foreach(MovingPrefab prefab in _prefabs)
                {
                    prefab.SetAudioLength(_adjustedDistanceBetweenPrefabs / (_transferSpeed / transform.parent.localScale.x));
                }
            }
        }

        [SerializeField, HideInInspector] NetworkNode _network;
        /// <summary>
        /// The network connected to this pipeline
        /// </summary>
        public NetworkNode Network
        {
            get => _network;
            set => _network = value;
        }

        [SerializeField, HideInInspector] HostNode _host;
        /// <summary>
        /// The host connected to this pipeline
        /// </summary>
        public HostNode Host
        {
            get => _host;
            set => _host = value;
        }

        [SerializeField, HideInInspector] string _identifier;
        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        /// <summary>
        /// The prefab of the objects that will move along the cylinder
        /// </summary>
        [SerializeField] MovingPrefab _movingPrefab;

        /// <summary>
        /// The ideal distance we'd like between the prefabs of the pipeline
        /// </summary>
        [SerializeField, HideInInspector] float _distanceBetweenPrefabs = 5;

        /// <summary>
        /// The distance between the prefabs once the actual length of the pipeline is taken into account
        /// </summary>
        float _adjustedDistanceBetweenPrefabs;

        [SerializeField, HideInInspector] float _width = 0.2f;
        /// <summary>
        /// The width of the pipeline, 0.2 by default
        /// </summary>
        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                if (Linked != null)
                {
                    Linked.Width = value;
                }
            }
        }

        [SerializeField, HideInInspector] float _hostHeight = 3.5f;
        /// <summary>
        /// The height at which the pipeline should connnect to the host
        /// </summary>
        public float HostHeight
        {
            get => _hostHeight;
            set
            {
                _hostHeight = value * transform.parent.localScale.x;

                //the direction along which the prefabs will move
                Vector3 direction = (_network.transform.position + _networkHeight * transform.parent.up) - (_host.transform.position + _hostHeight * transform.parent.up);
                _length = direction.magnitude;

                //the pipeline is positioned halfway between the host and the network
                transform.position = _host.transform.position + transform.parent.up * _hostHeight + direction / 2f;
                //the pipeline is alligned along the path
                transform.up = direction;

                //the cylinder is elongated to be the correct length
                _cylinder.transform.localScale = new Vector3(_width, _length / 2f / transform.lossyScale.x, _width);

                _hostCylinder.transform.localScale = new Vector3(_width, (_hostHeight - _originalHostHeight) / 2f / transform.parent.localScale.x, _width);
                _hostCylinder.transform.position = _host.transform.position + transform.parent.up * (_originalHostHeight + (_hostHeight - _originalHostHeight) / 2f);
                _hostCylinder.transform.up = transform.parent.up;

                _adjustedDistanceBetweenPrefabs = (_length + Mathf.Abs(_hostHeight - _originalHostHeight) + Mathf.Abs(_networkHeight - _originalNetworkHeight)) / (_prefabs.Count - 2);

                if (Linked != null)
                {
                    Linked.HostHeight = value;
                }
            }
        }
        [SerializeField, HideInInspector] float _networkHeight = 3.5f;
        /// <summary>
        /// The height at which the pipeline should connnect to the network
        /// </summary>
        public float NetworkHeight
        {
            get => _networkHeight;
            set
            {
                _networkHeight = value * transform.parent.localScale.x;

                //the direction along which the prefabs will move
                Vector3 direction = (_network.transform.position + _networkHeight * transform.parent.up) - (_host.transform.position + _hostHeight * transform.parent.up);
                _length = direction.magnitude;

                //the pipeline is positioned halfway between the host and the network
                transform.position = _host.transform.position + transform.parent.up * _hostHeight + direction / 2f;
                //the pipeline is alligned along the path
                transform.up = direction;

                //the cylinder is elongated to be the correct length
                _cylinder.transform.localScale = new Vector3(_width, _length / 2f / transform.lossyScale.x, _width);


                _networkCylinder.transform.localScale = new Vector3(_width, (_networkHeight - _originalNetworkHeight) / 2f / transform.parent.localScale.x, _width);
                _networkCylinder.transform.position = _network.transform.position + transform.parent.up * (_originalNetworkHeight + (_networkHeight - _originalNetworkHeight) / 2f);
                _networkCylinder.transform.up = transform.parent.up;

                _adjustedDistanceBetweenPrefabs = (_length + Mathf.Abs(_hostHeight - _originalHostHeight) + Mathf.Abs(_networkHeight - _originalNetworkHeight)) / (_prefabs.Count - 2);

                if (Linked != null)
                {
                    Linked.NetworkHeight = value;
                }
            }
        }

        float _originalHostHeight;
        float _originalNetworkHeight;
        bool _heightOverriden = false;
        public bool HeightOverriden
        {
            get => _heightOverriden;
        }

        /// <summary>
        /// The list containing the prefabs moving along the pipeline
        /// </summary>
        [SerializeField, HideInInspector] List<MovingPrefab> _prefabs = new List<MovingPrefab>();

        /// <summary>
        /// The main body of the pipeline
        /// </summary>
        [SerializeField] GameObject _cylinder;
        [SerializeField] GameObject _hostCylinder;
        [SerializeField] GameObject _networkCylinder;

        /// <summary>
        /// The color gradient used to color the pipeline and the prefabs
        /// </summary>
        [SerializeField] Gradient _gradient;

        /// <summary>
        /// The color picker associated with the pipeline, used to override its color
        /// </summary>
        [SerializeField] ColorPicker _picker;
        /// <summary>
        /// bool used to know if the user has chosen a color for the pipeline and thus if it shouldn't be changed by the system
        /// </summary>
        bool _colorOverriden = false;
        public bool ColorOverriden
        {
            get => _colorOverriden;
        }

        public Color Color
        {
            get => _cylinder.GetComponent<Renderer>().material.color;
        }

        /// <summary>
        /// The length between the host and the network
        /// </summary>
        float _length;

        Pipeline _linked;
        /// <summary>
        /// The minimap pipeline that should be updated with this pipeline
        /// </summary>
        public Pipeline Linked
        {
            get => _linked;
            set => _linked = value;
        }

        // Start is called before the first frame update
        void Start()
        {
            // we first need to adjust the pipeline according to the scale of its parent (used mainly for the minimap)
            _distanceBetweenPrefabs *= transform.parent.localScale.x;
            _hostHeight *= transform.parent.localScale.x;
            _networkHeight *= transform.parent.localScale.x; ;
            _transferSpeed *= transform.parent.localScale.x;
            _transferSize *= transform.parent.localScale.x;
            _originalHostHeight = _hostHeight;
            _originalNetworkHeight = _networkHeight;
            _heightOverriden = false;

            //the direction along which the prefabs will move
            Vector3 direction = (_network.transform.position + _networkHeight * transform.parent.up) - (_host.transform.position + _hostHeight * transform.parent.up);
            _length = direction.magnitude;

            //the pipeline is positioned halfway between the host and the network
            transform.position = _host.transform.position + transform.parent.up * _hostHeight + direction / 2f;
            //the pipeline is alligned along the path
            transform.up = direction;

            //the cylinder is elongated to be the correct length
            _cylinder.transform.localScale = new Vector3(_width, _length / 2f / transform.lossyScale.x, _width);
            _hostCylinder.transform.localScale = new Vector3(_width, 0, _width);
            _hostCylinder.transform.position = _host.transform.position + transform.parent.up * _hostHeight;
            _hostCylinder.transform.up = transform.parent.up;
            _networkCylinder.transform.localScale = new Vector3(_width, 0, _width);
            _networkCylinder.transform.position = _network.transform.position + transform.parent.up * _networkHeight;
            _networkCylinder.transform.up = transform.parent.up;

            //how manny prefabs are we going to need?
            int numberOfPrefabs = (int)(_length / _distanceBetweenPrefabs) + 1;

            //if the pipeline is very short, we still need at least one prefab
            _adjustedDistanceBetweenPrefabs = _length / (Mathf.Max(numberOfPrefabs - 1, 1));

            //create the prefabs and place them at the host (their positions will be adjusted in the update)
            for (int i = 0; i <= numberOfPrefabs; i++)
            {
                MovingPrefab prefab = Instantiate(_movingPrefab);
                prefab.transform.position = _host.transform.position + transform.parent.up * _hostHeight;
                prefab.transform.localScale = Vector3.one * _transferSize;
                prefab.transform.SetParent(transform);
                prefab.SetAudioLength(_adjustedDistanceBetweenPrefabs / (_transferSpeed / transform.parent.localScale.x));
                prefab.State = MovingPrefab.StateEnum.INFLATING;
                prefab.ShouldPlaySound = !(Linked == null);
                _prefabs.Add(prefab);
            }

            //color the cylinder and the prefab
            SetColor();

            //allows the user to change the pipeline color
            _picker.onValueChanged.AddListener(color =>
            {
                PipelineManager.Instance.OverrideColor(_identifier, color);
            });

            //if we are not the master, we need to synchronize with it
            if (!PhotonNetwork.IsMasterClient)
            {
                PipelineManager.Instance.SyncPipeToMaster(_identifier);
            }
        }

        // Update is called once per frame
        void Update()
        {
            //the first prefab serves as a reference to position the others
            MovingPrefab firstPrefab = _prefabs[0];

            //the start point of the prefab
            Vector3 startingNodePosition;
            Vector3 startingNodeJunction;
            Vector3 endingNodePosition;
            Vector3 endingNodeJunction;
            float originalStartHeight;
            float startHeight;
            float originalEndHeight;
            float endHeight;

            //how far is the first prefab along its path?
            float firstPrefabProgress = 0;

            if (_transferDirection == 1)
            {
                startingNodePosition = _host.transform.position + transform.parent.up * _originalHostHeight;
                startingNodeJunction = _host.transform.position + transform.parent.up * _hostHeight;
                endingNodePosition = _network.transform.position + transform.parent.up * _originalNetworkHeight;
                endingNodeJunction = _network.transform.position + transform.parent.up * _networkHeight;
                originalStartHeight = _originalHostHeight;
                startHeight = _hostHeight;
                originalEndHeight = _originalNetworkHeight;
                endHeight = _networkHeight;
            }
            else
            {
                startingNodePosition = _network.transform.position + transform.parent.up * _originalNetworkHeight;
                startingNodeJunction = _network.transform.position + transform.parent.up * _networkHeight;
                endingNodePosition = _host.transform.position + transform.parent.up * _originalHostHeight;
                endingNodeJunction = _host.transform.position + transform.parent.up * _hostHeight;
                originalStartHeight = _originalNetworkHeight;
                startHeight = _networkHeight;
                originalEndHeight = _originalHostHeight;
                endHeight = _hostHeight;
            }

            float distanceFromStartPosition = (firstPrefab.transform.position - startingNodePosition).magnitude;
            float distanceFromStartJunction = (firstPrefab.transform.position - startingNodeJunction).magnitude;
            float distanceFromEndPosition = (firstPrefab.transform.position - endingNodePosition).magnitude;
            float distanceFromEndJunction = (firstPrefab.transform.position - endingNodeJunction).magnitude;

            if(firstPrefab.State == MovingPrefab.StateEnum.INFLATING)
            {
                //we increase the size of the prefab little by little to make it look like it's being inflated
                firstPrefab.transform.localScale += Vector3.one * Time.deltaTime / _adjustedDistanceBetweenPrefabs * _transferSpeed * _transferSize / transform.parent.localScale.x;
                //check that we have reached the max scale
                if (firstPrefab.transform.localScale.x > _transferSize / transform.parent.localScale.x)
                {
                    firstPrefab.transform.localScale = Vector3.one * _transferSize / transform.parent.localScale.x;
                    firstPrefab.State = MovingPrefab.StateEnum.ASCENDING;
                }

                firstPrefabProgress = firstPrefab.transform.localScale.x / (_transferSize / transform.parent.localScale.x) * _adjustedDistanceBetweenPrefabs;
            }

            else if(firstPrefab.State == MovingPrefab.StateEnum.ASCENDING)
            {
                firstPrefab.transform.position += Mathf.Sign(startHeight - originalStartHeight) * transform.parent.up * Time.deltaTime * _transferSpeed;
                if(distanceFromStartPosition > Mathf.Abs(startHeight - originalStartHeight))
                {
                    firstPrefab.transform.position = startingNodeJunction;
                    firstPrefab.State = MovingPrefab.StateEnum.MOVING;
                }

                firstPrefabProgress = _adjustedDistanceBetweenPrefabs + (firstPrefab.transform.position - startingNodePosition).magnitude;
            }

            else if(firstPrefab.State == MovingPrefab.StateEnum.MOVING)
            {
                firstPrefab.transform.position += _transferDirection * transform.up * Time.deltaTime * _transferSpeed;
                if (distanceFromStartJunction > _length)
                {
                    firstPrefab.transform.position = endingNodeJunction;
                    firstPrefab.State = MovingPrefab.StateEnum.DESCENDING;
                }

                firstPrefabProgress = _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + (firstPrefab.transform.position - startingNodeJunction).magnitude;
            }

            else if(firstPrefab.State == MovingPrefab.StateEnum.DESCENDING)
            {
                firstPrefab.transform.position -= Mathf.Sign(endHeight - originalEndHeight) * transform.parent.up * Time.deltaTime * _transferSpeed;
                if(distanceFromEndJunction > Mathf.Abs(endHeight - originalEndHeight))
                {
                    firstPrefab.transform.position = endingNodePosition;
                    firstPrefab.State = MovingPrefab.StateEnum.DEFLATING;
                }

                firstPrefabProgress = _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + _length + (firstPrefab.transform.position - endingNodeJunction).magnitude;
            }

            else if(firstPrefab.State == MovingPrefab.StateEnum.DEFLATING)
            {
                firstPrefab.transform.localScale -= Vector3.one * Time.deltaTime / _adjustedDistanceBetweenPrefabs * _transferSpeed * _transferSize / transform.parent.localScale.x;
                if (firstPrefab.transform.localScale.x < 0)
                {
                    firstPrefab.transform.localScale = Vector3.zero;
                    firstPrefab.transform.position = startingNodePosition;
                    firstPrefab.State = MovingPrefab.StateEnum.INFLATING;
                }

                firstPrefabProgress = _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + _length + Mathf.Abs(endHeight - originalEndHeight) +
                    (1 - firstPrefab.transform.localScale.x / (_transferSize / transform.parent.localScale.x)) * _adjustedDistanceBetweenPrefabs;
            }

            //position the other prefabs based on the first one
            for (int i = 1; i < _prefabs.Count; i++)
            {
                MovingPrefab prefab = _prefabs[i];

                //reset its scale
                prefab.transform.localScale = Vector3.one * _transferSize / transform.parent.localScale.x;

                //first prefab serves as reference
                float progress = firstPrefabProgress + i * _adjustedDistanceBetweenPrefabs;

                //clamp the progress between 0 and its max value
                if (progress > _length + 2 * _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + Mathf.Abs(endHeight - originalEndHeight))
                {
                    progress -= _length + 2 * _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + Mathf.Abs(endHeight - originalEndHeight);
                }

                //the prefab needs to be at the start of the cylinder and not completely inflated
                if (progress < _adjustedDistanceBetweenPrefabs)
                {
                    prefab.State = MovingPrefab.StateEnum.INFLATING;
                    prefab.transform.position = startingNodePosition;
                    prefab.transform.localScale = Vector3.one * progress / _adjustedDistanceBetweenPrefabs * _transferSize / transform.parent.localScale.x;
                }

                //the prefab needs to be at the end of the cylinder and partly deflated
                else if (progress < _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight))
                {
                    progress -= _adjustedDistanceBetweenPrefabs;
                    prefab.State = MovingPrefab.StateEnum.ASCENDING;
                    prefab.transform.position = startingNodePosition + progress * Mathf.Sign(startHeight - originalStartHeight) * transform.parent.up;
                }

                else if(progress < _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + _length)
                {
                    progress -= _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight);
                    prefab.State = MovingPrefab.StateEnum.MOVING;
                    prefab.transform.position = startingNodeJunction + progress * transform.up * _transferDirection;
                }

                else if(progress < _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + _length + Mathf.Abs(endHeight - originalEndHeight))
                {
                    progress -= _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + _length;
                    prefab.State = MovingPrefab.StateEnum.DESCENDING;
                    prefab.transform.position = endingNodeJunction - Mathf.Sign(endHeight - originalEndHeight) * progress * transform.parent.up;
                }

                else
                {
                    progress -= _adjustedDistanceBetweenPrefabs + Mathf.Abs(startHeight - originalStartHeight) + _length + Mathf.Abs(endHeight - originalEndHeight);
                    prefab.State = MovingPrefab.StateEnum.DEFLATING;
                    prefab.transform.position = endingNodePosition;
                    prefab.transform.localScale = Vector3.one * (1 - progress / _adjustedDistanceBetweenPrefabs) * _transferSize / transform.parent.localScale.x;
                }
            }
        }

        /// <summary>
        /// If the pipeline color has not been overriden, sets its color depending on the product TransferSpeed*TransferSize
        /// </summary>
        void SetColor()
        {
            if (Application.isPlaying && !_colorOverriden)
            {
                Color newColor = _gradient.Evaluate(_transferSize * _transferSpeed / 20 / transform.parent.localScale.x / transform.parent.localScale.x);
                _cylinder.GetComponent<Renderer>().material.color = newColor;
                _hostCylinder.GetComponent<Renderer>().material.color = newColor;
                _networkCylinder.GetComponent<Renderer>().material.color = newColor;
                foreach (MovingPrefab prefab in _prefabs)
                {
                    prefab.GetComponent<Renderer>().material.color = newColor;
                }
                _picker.CurrentColor = newColor;
            }
        }

        /// <summary>
        /// Let's the user choose a cutomised color for the pipeline
        /// </summary>
        /// <param name="newColor">The new color of the pipeline</param>
        public void OverrideColor(Color newColor)
        {
            if(!(newColor == _cylinder.GetComponent<Renderer>().material.color))
            {
                if (_linked)
                {
                    _linked.OverrideColor(newColor);
                }
                _colorOverriden = true;
                _cylinder.GetComponent<Renderer>().material.color = newColor;
                _hostCylinder.GetComponent<Renderer>().material.color = newColor;
                _networkCylinder.GetComponent<Renderer>().material.color = newColor;
                foreach (MovingPrefab prefab in _prefabs)
                {
                    prefab.GetComponent<Renderer>().material.color = newColor;
                    prefab.ModifyMesh();
                }
            }
        }

        /// <summary>
        /// Resets the color for everyone on the network
        /// </summary>
        public void NetworkResetColor()
        {
            PipelineManager.Instance.NetworkResetColor(_identifier);
        }

        /// <summary>
        /// Resets the pipeline's color so it will update with the transfer size and the transfer speed
        /// </summary>
        public void ResetColor()
        {
            if (_linked)
            {
                _linked.ResetColor();
            }
            _colorOverriden = false;
            SetColor();
            if (!_heightOverriden)
            {
                foreach (MovingPrefab prefab in _prefabs)
                {
                    prefab.ReturnToBaseMesh();
                }
            }
        }

        /// <summary>
        /// Resets the height for everyone on the network
        /// </summary>
        public void NetworkResetHeight()
        {
            PipelineManager.Instance.NetworkResetColor(_identifier);
        }

        /// <summary>
        /// Resets the pipeline's height
        /// </summary>
        public void ResetHeight()
        {
            if (_linked)
            {
                _linked.ResetHeight();
            }
            _heightOverriden = false;
            HostHeight = _originalHostHeight;
            NetworkHeight = _originalNetworkHeight;
            if (!_colorOverriden)
            {
                foreach(MovingPrefab prefab in _prefabs)
                {
                    prefab.ReturnToBaseMesh();
                }
            }
        }

        public void ModifyPrefabMeshes()
        {
            _heightOverriden = true;
            foreach (MovingPrefab prefab in _prefabs)
            {
                prefab.ModifyMesh();
            }

            if (_linked)
            {
                _linked.ModifyPrefabMeshes();
            }
        }

        /// <summary>
        /// Makes the prefabs go in the opposite direction
        /// </summary>
        public void ChangeDirection()
        {
            _transferDirection *= -1;
            if (_linked)
            {
                _linked.ChangeDirection();
            }
        }

        void OnEnable()
        {
            if (_linked)
            {
                _linked.gameObject.SetActive(true);
                _linked.enabled = true;
            }
        }

        void OnDisable()
        {
            if (_linked)
            {
                _linked.gameObject.SetActive(false);
                _linked.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_linked)
            {
                Destroy(_linked.gameObject);
            }
            _host.Connections.Remove(_identifier);
            _network.Connected.Remove(_identifier);
        }
    }
}