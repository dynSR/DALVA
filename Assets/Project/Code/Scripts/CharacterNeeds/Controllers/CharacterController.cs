using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviourPun
{
    [Header("CONTROLLER ATTRIBUTES VALUE")]
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float motionSmoothTime = .1f;
    [SerializeField] private float rotateVelocity = .1f;
    [SerializeField] private Animator characterAnimator;
    private bool canMove = true;

    #region Refs
    protected InteractionSystem Interactions => GetComponent<InteractionSystem>();
    protected CharacterStat Stats => GetComponent<CharacterStat>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    #endregion

    public float RotationSpeed { get => rotationSpeed; }
    public float MotionSmoothTime { get => motionSmoothTime; }
    public float RotateVelocity { get => rotateVelocity; }
    public bool CanMove { get => canMove; set => canMove = value; }

    public Animator CharacterAnimator { get => characterAnimator; }

    protected virtual void Update() => HandleMotionAnimation(Agent, CharacterAnimator, "MoveSpeed", MotionSmoothTime);

    #region Character Destination and motion handling, including rotation
    public void SetNavMeshAgentSpeed(NavMeshAgent agent, float value)
    {
        agent.speed = value;
    }

    public void SetAgentDestination(NavMeshAgent agent, Vector3 pos)
    {
        if(CanMove)
            agent.SetDestination(pos);
    }

    public void HandleMotionAnimation(NavMeshAgent agent, Animator animator, string animationFloatName, float smoothTime)
    {
        if (!agent.hasPath)
        {
            animator.SetFloat(animationFloatName, 0, smoothTime, Time.deltaTime);
            return;
        }

        float moveSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(animationFloatName, moveSpeed, smoothTime, Time.deltaTime);
    }

    public void HandleCharacterRotation(Transform transform, Vector3 target, float rotateVelocity, float rotationSpeed)
    {
        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            rotationSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }
    #endregion
}