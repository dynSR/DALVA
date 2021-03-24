using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviourPun, IPunObservable
{
    [Header("CONTROLLER ATTRIBUTES VALUE")]
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float motionSmoothTime = .1f;
    [SerializeField] private float rotateVelocity = .1f;
    [SerializeField] private Animator characterAnimator;
    private bool canMove = true;
    private bool isCasting = false;

    //Network
    [HideInInspector]
    [SerializeField] private Vector3 networkPosition = Vector3.zero;
    [HideInInspector]
    [SerializeField] private Quaternion networkRotation = Quaternion.identity;
    [HideInInspector]
    [SerializeField] private float networkSpeed = 0f;
    private double lastNetworkUpdate = 0f;

    #region Refs
    protected InteractionSystem Interactions => GetComponent<InteractionSystem>();
    protected CharacterStat Stats => GetComponent<CharacterStat>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    #endregion

    public float RotationSpeed { get => rotationSpeed; }
    public float MotionSmoothTime { get => motionSmoothTime; }
    public float RotateVelocity { get => rotateVelocity; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsCasting { get => isCasting; set => isCasting = value; }

    public Animator CharacterAnimator { get => characterAnimator; }

    protected virtual void Update()
    {
        HandleMotionAnimation(Agent, CharacterAnimator, "MoveSpeed", MotionSmoothTime);

        //Check if it's NOT a player
        if(GetComponent<PlayerController>() == null)
        {
            
        }
    }

    #region Character Destination and motion handling, including rotation
    public void SetNavMeshAgentSpeed(NavMeshAgent agent, float value)
    {
        agent.speed = value;
    }

    public void SetAgentDestination(NavMeshAgent agent, Vector3 pos)
    {
        if (!CanMove) return;
            
        agent.SetDestination(pos);
    }

    public void HandleMotionAnimation(NavMeshAgent agent, Animator animator, string animationFloatName, float smoothTime)
    {
        if (!agent.hasPath)
        {
            animator.SetFloat(animationFloatName, 0, smoothTime, Time.deltaTime);
            return;
        }

        float moveSpeed = agent.velocity.sqrMagnitude / agent.speed;
        animator.SetFloat(animationFloatName, moveSpeed, smoothTime, Time.deltaTime);
    }

    public void HandleCharacterRotation(Transform transform, Vector3 target, float rotateVelocity, float rotationSpeed)
    {
        if (IsCasting) return;

        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            rotationSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
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

    public void UpdateNetworkPosition()
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