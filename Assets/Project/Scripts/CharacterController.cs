using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LineRenderer))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    private NavMeshAgent navMeshAgent;
    Ray ray;
    RaycastHit cursorRaycastHit;
    Vector3 navMeshDestination;
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private GameObject movementFeedback;

    private LineRenderer lineRenderer;

    private Animator animator;

    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = MovementSpeed;

        animator = transform.GetChild(0).GetComponent<Animator>();

        lineRenderer = GetComponent<LineRenderer>();
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Process Moving character");
            HandleRayCast();
            Move(navMeshDestination);
        }

        HandleAnimation("isMoving", !PathComplete());

        if (navMeshAgent.hasPath)
        {
            HandleRotation();

            lineRenderer.positionCount = navMeshAgent.path.corners.Length;
            lineRenderer.SetPositions(navMeshAgent.path.corners);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void HandleRayCast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out cursorRaycastHit, 100f, walkableLayer))
        {
            navMeshDestination = cursorRaycastHit.point;
            MovementFeedbackInstantiation(movementFeedback, navMeshDestination);
        }
    }

    void HandleRotation()
    {
        //transform.LookAt(navMeshAgent.nextPosition);
    }

    void Move(Vector3 destinationToMove)
    {
        navMeshAgent.destination = destinationToMove;
    }

    protected bool PathComplete()
    {
        if (Vector3.Distance(navMeshAgent.destination, navMeshAgent.transform.position) <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                return true;
        }

        return false;
    }

    void MovementFeedbackInstantiation(GameObject _movementFeedback, Vector3 pos)
    {
        if (_movementFeedback != null)
            Instantiate(_movementFeedback, pos, Quaternion.identity);
    }

    void HandleAnimation(string boolName, bool value)
    {
        animator.SetBool(boolName, value);
    }

}
