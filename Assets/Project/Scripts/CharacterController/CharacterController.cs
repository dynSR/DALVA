using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
//Enemies + players inherits from
public class CharacterController : MonoBehaviourPun
{
    [Header("CHARACTER RENDERER")]
    [SerializeField] private GameObject rendererGameObject;

    //Raycast used for movements
    Ray RayFromCameraToMousePosition => Camera.main.ScreenPointToRay(Input.mousePosition);
    
    [Header("MOVEMENTS PARAMETERS")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Camera characterCamera;
    private Vector3 destinationToGo = Vector3.zero;

    [Header("MOVEMENTS FEEDBACK PARAMETERS")]
    [SerializeField] private GameObject movementFeedback;
    [SerializeField] private GameObject pathLandmark;
    private LineRenderer LineRenderer => GetComponent<LineRenderer>();
    private Animator Animator
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

    public NavMeshAgent NavMeshAgent => GetComponent<NavMeshAgent>();
    private Character CharacterStats => GetComponent<Character>();
    public GameObject PathLandmark { get => pathLandmark; }
    public GameObject RendererGameObject { get => rendererGameObject; }
    public float InitialSpeed { get; private set; }
    public float CurrentSpeed { get => NavMeshAgent.speed; set => NavMeshAgent.speed = value; }

    protected virtual void Start()
    {
        InitialSpeed = NavMeshAgent.speed;
        CurrentSpeed = InitialSpeed;
    }

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && photonView.IsMine == false && PhotonNetwork.IsConnected == true){ return; }

        if (Input.GetMouseButtonDown(1))
        {
            SetNavMeshDestinationWithRayCast();
        }

        MoveAgentToAttackDestination(destinationToGo);
        MoveWithMouseClick();
        DebugPathing(LineRenderer);
    }

    #region Handle Movement 
    private void SetNavMeshDestinationWithRayCast()
    {
        if (Physics.Raycast(RayFromCameraToMousePosition, out RaycastHit hit, 100f, walkableLayer))
        {
            MovementFeedbackInstantiation(movementFeedback, hit.point);
            NavMeshAgent.SetDestination(hit.point);
        }
        else if (Physics.Raycast(RayFromCameraToMousePosition, out hit, 100f) && hit.transform.CompareTag("Enemy"))
        {
            destinationToGo = hit.transform.position;
            MoveAgentToAttackDestination(destinationToGo);
        }
    }

    private void MoveWithMouseClick()
    {
        if (NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
        {
            NavMeshAgent.Move(NavMeshAgent.desiredVelocity * 0.25f * Time.deltaTime);

            rendererGameObject.transform.LookAt(pathLandmark.transform);

            HandleAnimation("isMoving", true);
        }
        else
        {
            NavMeshAgent.Move(Vector3.zero);
            HandleAnimation("isMoving", false);
        }
    }

    public void MoveAgentToAttackDestination(Vector3 hitPoint)
    {
        float distance = Vector3.Distance(transform.position, hitPoint);

        if (distance > CharacterStats.AttackRange)
        {
            NavMeshAgent.Move(NavMeshAgent.desiredVelocity * 0.25f * Time.deltaTime);
            HandleAnimation("isMoving", true);
            //Debug.Log("Move To Specific Destination");
        }
        else if (distance <= CharacterStats.AttackRange)
        {
            NavMeshAgent.Move(Vector3.zero);
            TryToAttack();
            //HandleAnimation("isAttacking", false);
            //Debug.Log("Specific Destination Reached");
        }
    }

    private void DebugPathing(LineRenderer line)
    {
        if (NavMeshAgent.hasPath)
        {
            line.positionCount = NavMeshAgent.path.corners.Length;
            line.SetPositions(NavMeshAgent.path.corners);
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

    void TryToAttack()
    {
        Debug.Log("Can Perform Attack");
    }

    public void HandleAnimation(string boolName, bool value)
    {
        Animator.SetBool(boolName, value);
    }

    public void InstantiateCharacterCameraAtStartOfTheGame()
    {
        GameObject cameraInstance = Instantiate(characterCamera.gameObject, characterCamera.transform.position, characterCamera.transform.rotation) as GameObject;
        cameraInstance.GetComponent<CameraController>().TargetToFollow = this.transform;
    }
}