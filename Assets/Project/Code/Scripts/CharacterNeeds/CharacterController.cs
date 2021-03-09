using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviourPun
{
    [Header("MOVEMENTS PARAMETERS")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Camera characterCamera;
    [SerializeField] private float rotateSpeedMovement = 0.1f;
    [SerializeField] private bool isPlayerInHisBase = true;
    private readonly float motionSmoothTime = .1f;

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
    private InteractionsSystem CharacterInteractions => GetComponent<InteractionsSystem>();
    private CharacterStats CharacterStats => GetComponent<CharacterStats>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    public float InitialMoveSpeed { get; private set; }

    //ajouter additive speed

    public float CurrentMoveSpeed { get => Agent.speed ; set => Agent.speed = value; }
    public float MotionSmoothTime { get => motionSmoothTime; }
    public float RotateVelocity { get; set; }
    public bool IsPlayerInHisBase { get => isPlayerInHisBase; set => isPlayerInHisBase = value; }

    private bool PlayerIsConsultingHisShopAtBase => IsPlayerInHisBase && GetComponentInChildren<PlayerHUDManager>().IsShopWindowOpen;
    public bool CursorIsHoveringMiniMap => EventSystem.current.IsPointerOverGameObject();

    protected virtual void Awake()
    {
        InitialMoveSpeed = Agent.speed;
        CurrentMoveSpeed = InitialMoveSpeed;
    }

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && !photonView.IsMine && PhotonNetwork.IsConnected || CharacterStats.IsDead) return;

        if (UtilityClass.RightClickIsHeld())
        {
            if (CursorIsHoveringMiniMap) return;

            CharacterInteractions.ResetInteractionState();

            SetNavMeshDestination(UtilityClass.RayFromMainCameraToMousePosition());
        }

        UtilityClass.HandleMotionAnimation(Agent, CharacterAnimator, "MoveSpeed", MotionSmoothTime);
        DebugPathing(MyLineRenderer);
    }

    #region Handle Movement 
    public void SetNavMeshDestination(Ray ray)
    {
        if (PlayerIsConsultingHisShopAtBase) return;

        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, walkableLayer))
        {
            if (UtilityClass.RightClickIsPressed())
            {
                Debug.Log("Object touched by the character controller raycast " + raycastHit.transform.gameObject.name);

                CreateMovementFeedback(movementFeedback, raycastHit.point);
            }

            UtilityClass.SetAgentDestination(raycastHit.point, Agent);
            UtilityClass.HandleCharacterRotation(transform, raycastHit.point, RotateVelocity, rotateSpeedMovement);
        }
    }

    public void DebugPathing(LineRenderer line)
    {
        if (Agent.hasPath && CharacterInteractions.Target != null)
        {
            line.positionCount = Agent.path.corners.Length;
            line.SetPositions(Agent.path.corners);
            line.enabled = true;
        }
        else
            line.enabled = false;
    }

    private void CreateMovementFeedback(GameObject _movementFeedback, Vector3 pos)
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