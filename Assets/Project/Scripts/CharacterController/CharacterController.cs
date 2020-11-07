using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
//Enemies + players inherits from
public class CharacterController : MonoBehaviourPun
{
    [Header("CHARACTER RENDERER")]
    [SerializeField] private GameObject rendererGameObject;

    //Raycast used for movements
    Ray ray;
    RaycastHit cursorRaycastHit;
    
    [Header("MOVEMENTS PARAMETERS")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Camera characterCamera;

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
    public GameObject PathLandmark { get => pathLandmark; }
    public GameObject RendererGameObject { get => rendererGameObject; }
    public float InitialSpeed { get; private set; }
    public float CurrentSpeed { get => NavMeshAgent.speed; set => NavMeshAgent.speed = value; }

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    protected virtual void Awake()
    {
        //Reseau
        if (GameObject.Find("GameNetworkManager") != null && photonView != null && photonView.IsMine)
        {
            CharacterController.LocalPlayerInstance = this.gameObject;
            InstantiateCharacterCameraAtStartOfTheGame();
        }
        ////Local
        else if(GameObject.Find("GameNetworkManager") == null)
            InstantiateCharacterCameraAtStartOfTheGame();
    }

    protected virtual void Start()
    {
        InitialSpeed = NavMeshAgent.speed;
        CurrentSpeed = InitialSpeed;
    }

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Process Moving character");
            SetNavMeshDestinationWithRayCast();
        }

        MoveWithMouseClick();
        DebugPathing(LineRenderer);
    }

    #region Handle Movement 
    private void SetNavMeshDestinationWithRayCast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out cursorRaycastHit, 100f, walkableLayer))
        {
            MovementFeedbackInstantiation(movementFeedback, cursorRaycastHit.point);
            NavMeshAgent.SetDestination(cursorRaycastHit.point);
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

    public void MoveAgentToSpecificDestination(Transform destination)
    {
        if (NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
        {
            NavMeshAgent.destination = destination.position;
            HandleAnimation("isMoving", true);
        }
        else
        {
            NavMeshAgent.Move(Vector3.zero);
            HandleAnimation("isMoving", false);
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

    private void HandleAnimation(string boolName, bool value)
    {
        Animator.SetBool(boolName, value);
    }

    private void InstantiateCharacterCameraAtStartOfTheGame()
    {
        GameObject cameraInstance = Instantiate(characterCamera.gameObject, characterCamera.transform.position, characterCamera.transform.rotation) as GameObject;
        cameraInstance.GetComponent<CameraController>().TargetToFollow = this.transform;
    }
}