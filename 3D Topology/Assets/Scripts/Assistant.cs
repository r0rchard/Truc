using EnvironmentGeneration;
using Interaction;
using Photon.Pun;
using Photon.Realtime;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class Assistant : MonoBehaviour, IInRoomCallbacks
{
    NavMeshAgent _agent;
    public NavMeshAgent Agent
    {
        get => _agent;
        set => _agent = value;
    }

    Animator _animator;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    Vector3 _previousPosition;
    AudioSource _audioSource;

    AbstractNode _currentRoom;
    public AbstractNode CurrentRoom
    {
        get => _currentRoom;
        set => _currentRoom = value;
    }

    Environment _environment;
    public Environment Environment
    {
        get => _environment;
        set => _environment = value;
    }

    bool _roomChanged = false;
    public bool RoomChanged
    {
        get => _roomChanged;
        set => _roomChanged = value;
    }

    bool _isGuiding = false;
    public bool IsGuiding
    {
        get => _isGuiding;
    }

    [SerializeField] AudioClip _userConnection;
    [SerializeField] AudioClip _userDisconnection;
    [SerializeField] AudioClip _cameraAdded;
    [SerializeField] AudioClip _alert;
    [SerializeField] AudioClip _followMe;
    [SerializeField] LightBulb _bulb;

    [SerializeField] Spline _guidingPath;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _previousPosition = transform.position;
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldDeltaPosition = transform.position - _previousPosition;
        _previousPosition = transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.05f && _agent.remainingDistance > _agent.radius;
        // Update animation parameters
        _animator.SetBool("move", shouldMove);
        _animator.SetFloat("velx", velocity.x);
        _animator.SetFloat("vely", velocity.y);
    }

    private void OnAnimatorMove()
    {
        if (_agent != null)
        {
            transform.position = _agent.nextPosition;
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // We reset the path if we've reached the destination
            if (!_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _agent.ResetPath();
                }
            }

            //apply root motion for nicer idle animation
            if (!_agent.hasPath)
            {
                _animator.ApplyBuiltinRootMotion();
            }
        }
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Notify(Color.green, _userConnection);
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Notify(new Color(255, 127, 0), _userDisconnection);
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }

    public void PlayClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void OnCameraAdded()
    {
        Notify(new Color(255, 127, 0), _cameraAdded);
    }

    void Notify(Color color, AudioClip clip)
    {
        _bulb.Color = color;
        _bulb.gameObject.SetActive(true);
        _animator.SetTrigger("notification");
        PlayClip(clip);
    }

    public IEnumerator AlertNotification()
    {
        Notify(Color.red, _alert);
        yield return new WaitForSeconds(3);
    }

    public void StartGuidingTo(AbstractNode node)
    {
        StartCoroutine(GuideTo(node));
        StartCoroutine(FollowMeVoice());
    }

    IEnumerator GuideTo(AbstractNode node)
    {
        _roomChanged = false;
        _isGuiding = true;
        List<AbstractNode> pathToFollow = _environment.AStarSearch(_currentRoom, node);
        while (pathToFollow.Count > 0)
        {
            if (_currentRoom == pathToFollow[0])
            {
                pathToFollow.RemoveAt(0);
                if (pathToFollow.Count > 0)
                {
                    Vector3 destination = Vector3.zero;
                    Vector3 vibrationTarget = Vector3.zero;
                    Vector3 otherSide = Vector3.zero;
                    if(_currentRoom is NetworkNode network)
                    {
                        if(network.FrontDoor.FireWallExit.FireWall == pathToFollow[0])
                        {
                            destination = (network.FrontDoor.AssistantSpawn.position + network.FrontDoor.UserSpawn.position) / 2f;
                            vibrationTarget = network.FrontDoor.transform.position;
                            otherSide = network.FrontDoor.OtherSide.position;
                        }
                        else
                        {
                            destination = (network.BackDoor.AssistantSpawn.position + network.BackDoor.UserSpawn.position) / 2f;
                            vibrationTarget = network.BackDoor.transform.position;
                            otherSide = network.BackDoor.OtherSide.position;
                        }
                    }
                    else
                    {
                        FireWall fireWall = _currentRoom as FireWall;
                        foreach(FireWallExit exit in fireWall.Exits)
                        {
                            if(exit.Network == pathToFollow[0])
                            {
                                destination = (exit.AssistantSpawn.position + exit.UserSpawn.position) / 2f;
                                vibrationTarget = exit.transform.position;
                                otherSide = exit.OtherSide.position;
                                break;
                            }
                        }
                    }
                    _agent.SetDestination(destination);
                    VibrationManager.Instance.VibrateTowardsDirection(vibrationTarget);
                    while (_agent.pathPending)
                    {
                        yield return null;
                    }
                    Spline guidingPath = Instantiate(_guidingPath);
                    while(guidingPath.nodes.Count > 2)
                    {
                        guidingPath.RemoveNode(guidingPath.nodes[0]);
                    }
                    for (int i = 0; i < _agent.path.corners.Length; i++)
                    {
                        if (i != _agent.path.corners.Length - 1) 
                        {
                            guidingPath.AddNode(new SplineNode(_agent.path.corners[i], _agent.path.corners[i + 1]));
                        }
                        else
                        {
                            guidingPath.AddNode(new SplineNode(_agent.path.corners[i], 2 * _agent.path.corners[i] - _agent.path.corners[i - 1]));
                        }

                        if(i == 0 || i == 1)
                        {
                            guidingPath.RemoveNode(guidingPath.nodes[0]);
                        }
                    }
                    yield return new WaitUntil(() => _agent.remainingDistance > _agent.radius || _roomChanged);
                    yield return new WaitUntil(() => (transform.position - Camera.main.transform.position).magnitude < 2 || _roomChanged);
                    if (!_roomChanged)
                    {
                        yield return new WaitForSeconds(3);
                        _agent.SetDestination(otherSide);
                        VibrationManager.Instance.StopVibrating();
                        VibrationManager.Instance.VibrateTowardsDirection(otherSide);
                        yield return new WaitUntil(() => _roomChanged);
                    }
                    Destroy(guidingPath.gameObject);
                    VibrationManager.Instance.StopVibrating();
                    _roomChanged = false;
                }
            }
            else
            {
                pathToFollow = _environment.AStarSearch(_currentRoom, node);
            }
        }
        _isGuiding = false;
    }

    IEnumerator FollowMeVoice()
    {
        while (!_isGuiding)
        {
            yield return null;
        }
        while (_isGuiding)
        {
            _audioSource.clip = _followMe;
            _audioSource.Play();
            yield return new WaitForSeconds(10);
        }
    }
}
