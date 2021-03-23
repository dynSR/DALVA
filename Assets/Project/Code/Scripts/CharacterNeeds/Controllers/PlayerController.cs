using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class PlayerController : CharacterController, IPunObservable
{
    [Header("MOVEMENTS PARAMETERS")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private Camera characterCamera;

    [Header("MOVEMENTS FEEDBACK PARAMETERS")]
    [SerializeField] private GameObject movementFeedback;

    [SerializeField] private bool isPlayerInHisBase = true; // put in private after

    #region Refs
    public Camera CharacterCamera { get { return characterCamera; } private set { characterCamera = value; } }
    public LineRenderer MyLineRenderer => GetComponentInChildren<LineRenderer>();
    #endregion

    public bool IsPlayerInHisBase { get => isPlayerInHisBase; set => isPlayerInHisBase = value; }
    public bool IsCursorHoveringUIElement => EventSystem.current.IsPointerOverGameObject();

    //Network refs
    [HideInInspector]
    [SerializeField] private Vector3 networkPosition = Vector3.zero;
    [HideInInspector]
    [SerializeField] private Quaternion networkRotation = Quaternion.identity;
    [HideInInspector]
    [SerializeField] private float networkSpeed = 0f;
    private double lastNetworkUpdate = 0f;


    protected override void Update()
    {
        if (Stats.IsDead) return;

        if (GameObject.Find("NetworkManager") == null || photonView.IsMine)
        {
            if (UtilityClass.RightClickIsHeld() && !IsCursorHoveringUIElement)
            {
                SetNavMeshDestination(UtilityClass.RayFromMainCameraToMousePosition());

                DebugPathing(MyLineRenderer);
            }
        }
        else
        {
            UpdateNetworkPosition();
        }
    }

    #region Handle Cursor Movement 
    public void SetNavMeshDestination(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, walkableLayer))
        {
            if (UtilityClass.RightClickIsPressed())
            {
                CreateMovementFeedback(movementFeedback, raycastHit.point);
            }

            SetAgentDestination(Agent, raycastHit.point);
            HandleCharacterRotation(transform, raycastHit.point, RotateVelocity, RotationSpeed);
        }
    }
    #endregion

    #region Debug
    public void DebugPathing(LineRenderer line)
    {
        if (Agent.hasPath || Interactions.Target != null)
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(gameObject.GetComponent<NavMeshAgent>().velocity.magnitude);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkSpeed = (float)stream.ReceiveNext();

            lastNetworkUpdate = info.SentServerTime;
        }
    }

    public void InstantiateCharacterCameraAtStartOfTheGame()
    {
        GameObject cameraInstance = Instantiate(CharacterCamera.gameObject, CharacterCamera.transform.position, CharacterCamera.transform.rotation) as GameObject;

        cameraInstance.GetComponent<CameraController>().TargetToFollow = this.transform;

        CharacterCamera = cameraInstance.GetComponent<Camera>();
    }

    private void UpdateNetworkPosition()
    {
        float pingInSeconds = PhotonNetwork.GetPing() * 0.001f;
        float timeSinceUpdate = (float)(PhotonNetwork.Time - lastNetworkUpdate);
        float totalTimePassed = pingInSeconds + timeSinceUpdate;

        Vector3 exterpolatedTargetPosition = networkPosition + transform.forward * networkSpeed * totalTimePassed;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, exterpolatedTargetPosition, networkSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, exterpolatedTargetPosition) > 1f) newPosition = exterpolatedTargetPosition;

        transform.position = newPosition;
    }
    #endregion
}