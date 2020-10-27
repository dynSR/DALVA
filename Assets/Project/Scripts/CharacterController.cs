using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LineRenderer))]
//Enemies + players inherits from
public class CharacterController : MonoBehaviour
{
    [Header("MOVEMENT PARAMETERS")]
    [Range(0,1)]
    [SerializeField] private float movementSpeed = 5f;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;

    Ray ray;
    RaycastHit cursorRaycastHit;

    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private GameObject movementFeedback;
    private LineRenderer lineRenderer;
    private Vector3 moveVelocity = Vector3.zero;

    [SerializeField] private Transform projectileEmiterLocation;

    private Animator animator;

    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public Transform ProjectileEmiterLocation { get => projectileEmiterLocation; }

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //navMeshAgent.speed = MovementSpeed;

        animator = transform.GetChild(0).GetComponent<Animator>();

        lineRenderer = GetComponent<LineRenderer>();
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Process Moving character");
            SetNavMeshDestinationWithRayCast();
        }

        Move();
        DebugPathing(lineRenderer);
    }

    //protected virtual void FixedUpdate()
    //{
    //    Move();
    //}

    #region Handle Movement 
    void SetNavMeshDestinationWithRayCast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out cursorRaycastHit, 100f, walkableLayer))
        {
            MovementFeedbackInstantiation(movementFeedback, cursorRaycastHit.point);
            navMeshAgent.SetDestination(cursorRaycastHit.point);
        }
    }

    void Move()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            //NavMeshAgent
            navMeshAgent.Move(navMeshAgent.desiredVelocity * MovementSpeed * Time.deltaTime);

            //Rigidbody
            //moveVelocity = navMeshAgent.pathEndPosition * MovementSpeed;
            //rb.MovePosition(moveVelocity * Time.fixedDeltaTime);


            HandleAnimation("isMoving", true);
        }
        else
        {
            navMeshAgent.Move(Vector3.zero);
            HandleAnimation("isMoving", false);
        }
    }

    void DebugPathing(LineRenderer line)
    {
        if (navMeshAgent.hasPath)
        {
            line.positionCount = navMeshAgent.path.corners.Length;
            line.SetPositions(navMeshAgent.path.corners);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }

    void MovementFeedbackInstantiation(GameObject _movementFeedback, Vector3 pos)
    {
        if (_movementFeedback != null)
            Instantiate(_movementFeedback, pos, Quaternion.identity);
    }
    #endregion

    void HandleAnimation(string boolName, bool value)
    {
        animator.SetBool(boolName, value);
    }
}