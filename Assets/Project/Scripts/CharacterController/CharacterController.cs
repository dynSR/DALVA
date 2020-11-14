using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
//Enemies + players inherits from
public class CharacterController : MonoBehaviourPun
{
    Ray RayFromCameraToMousePosition => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    [Header("MOVEMENTS PARAMETERS")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private float rotateSpeedMovement = 0.1f;
    private float motionSmoothTime = .1f;

    [Header("MOVEMENTS FEEDBACK PARAMETERS")]
    [SerializeField] private GameObject movementFeedback;
    [SerializeField] private GameObject pathLandmark;
    private LineRenderer LineRenderer => GetComponent<LineRenderer>();
    [HideInInspector] public Animator CharacterAnimator
    {
        get => transform.GetChild(0).GetComponent<Animator>();
        set
        {
            if (transform.GetChild(0).GetComponent<Animator>() == null)
            {
                transform.GetChild(0).gameObject.AddComponent<Animator>();
            }
        }
    }

    private Stats CharacterStats => GetComponent<Stats>();
    private CombatBehaviour CharacterCombatBehaviour => GetComponent<CombatBehaviour>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    public GameObject PathLandmark { get => pathLandmark; }
    public float InitialSpeed { get; private set; }
    public float CurrentSpeed { get => Agent.speed ; set => Agent.speed = value; }
    public float RotateVelocity { get; set; }

    protected virtual void Awake()
    {
        InitialSpeed = Agent.speed;
        CurrentSpeed = InitialSpeed;
    }

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && photonView.IsMine == false && PhotonNetwork.IsConnected == true){ return; }

        if (CharacterStats.IsDead) return;

        if (Input.GetMouseButton(1))
        {
            SetNavMeshDestinationWithRayCast();
        }

        HandleMotionAnimation();
        DebugPathing(LineRenderer);
    }

    #region Handle Movement 
    private void SetNavMeshDestinationWithRayCast()
    {
        if (Physics.Raycast(RayFromCameraToMousePosition, out RaycastHit hit, Mathf.Infinity, walkableLayer))
        {
            if (Input.GetMouseButtonDown(1))
            {
                MovementFeedbackInstantiation(movementFeedback, hit.point);
            }

            Agent.SetDestination(hit.point);
            HandleCharacterRotation(transform, hit.point, RotateVelocity, rotateSpeedMovement);
        }
    }

    private void HandleMotionAnimation()
    {
        float speed = Agent.velocity.magnitude / Agent.speed;
        CharacterAnimator.SetFloat("Speed", speed, motionSmoothTime, Time.deltaTime);
    }

    public void HandleCharacterRotation(Transform transform, Vector3 target, float rotateVelocity, float rotateSpeed)
    {
        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
            rotationToLookAt.eulerAngles.y, 
            ref rotateVelocity, 
            rotateSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    private void DebugPathing(LineRenderer line)
    {
        if (Agent.hasPath)
        {
            line.positionCount = Agent.path.corners.Length;
            line.SetPositions(Agent.path.corners);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }

    private void MovementFeedbackInstantiation(GameObject _movementFeedback, Vector3 pos)
    {
        if (_movementFeedback != null)
            Instantiate(_movementFeedback, pos, Quaternion.identity);
    }
    #endregion

    public void InstantiateCharacterCameraAtStartOfTheGame()
    {
        GameObject cameraInstance = Instantiate(characterCamera.gameObject, characterCamera.transform.position, characterCamera.transform.rotation) as GameObject;
        cameraInstance.GetComponent<CameraController>().TargetToFollow = this.transform;
    }
}