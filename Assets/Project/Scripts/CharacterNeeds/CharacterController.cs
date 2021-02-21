using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviourPun
{
    Ray RayFromCameraToMousePosition => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    [Header("MOVEMENTS PARAMETERS")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private float rotateSpeedMovement = 0.1f;
    [SerializeField] private bool isPlayerInHisBase = true;

    private float motionSmoothTime = .1f;

    [Header("MOVEMENTS FEEDBACK PARAMETERS")]
    [SerializeField] private GameObject movementFeedback;

    public LineRenderer MyLineRenderer => GetComponentInChildren<LineRenderer>();
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

    public Camera CharacterCamera { get { return characterCamera; } private set { characterCamera = value; } }
    private CharacterStats CharacterStats => GetComponent<CharacterStats>();
    private CharacterCombatBehaviour CharacterCombatBehaviour => GetComponent<CharacterCombatBehaviour>();
    private PlayerRessourcesHandler PlayerRessources => GetComponent<PlayerRessourcesHandler>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    public float InitialMoveSpeed { get; private set; }
    public float CurrentMoveSpeed { get => Agent.speed ; set => Agent.speed = value; }
    public float RotateVelocity { get; set; }
    public bool IsPlayerInHisBase { get => isPlayerInHisBase; set => isPlayerInHisBase = value; }

    private bool PlayerIsConsultingHisShopAtBase => IsPlayerInHisBase && PlayerRessources.PlayerShopWindow.IsShopWindowOpen;
    public bool CursorIsHoveringMiniMap => EventSystem.current.IsPointerOverGameObject();


    protected virtual void Awake()
    {
        InitialMoveSpeed = Agent.speed;
        CurrentMoveSpeed = InitialMoveSpeed;
    }

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && !photonView.IsMine && PhotonNetwork.IsConnected) return;

        if (CharacterStats.IsDead) return;

        if (UtilityClass.RightClickIsHeld())
        {
            if (CursorIsHoveringMiniMap) return;

            SetNavMeshDestinationWithRayCast(RayFromCameraToMousePosition);
        }

        HandleMotionAnimation();
        DebugPathing(MyLineRenderer);
    }

    #region Handle Movement 
    public void SetNavMeshDestinationWithRayCast(Ray ray)
    {
        if (PlayerIsConsultingHisShopAtBase) return;

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, walkableLayer))
        {
            if (UtilityClass.RightClickIsPressed())
            {
                Debug.Log("Object touched by the character controller raycast " + raycastHit.transform.gameObject.name);

                MovementFeedbackInstantiation(movementFeedback, raycastHit.point);
            }

            SetNavMeshAgentDestination(raycastHit.point);
            HandleCharacterRotation(transform, raycastHit.point, RotateVelocity, rotateSpeedMovement);
        }
    }

    private void SetNavMeshAgentDestination(Vector3 pos)
    {
        Agent.SetDestination(pos);
    }

    public void HandleMotionAnimation()
    {
        float moveSpeed = Agent.velocity.magnitude / Agent.speed;
        CharacterAnimator.SetFloat("MoveSpeed", moveSpeed, motionSmoothTime, Time.deltaTime);
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

    public void DebugPathing(LineRenderer line)
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

    #region Network Needs
    public void InstantiateCharacterCameraAtStartOfTheGame()
    {
        GameObject cameraInstance = Instantiate(CharacterCamera.gameObject, CharacterCamera.transform.position, CharacterCamera.transform.rotation) as GameObject;
        cameraInstance.GetComponent<CameraController>().TargetToFollow = this.transform;

        CharacterCamera = cameraInstance.GetComponent<Camera>();
    }
    #endregion
}