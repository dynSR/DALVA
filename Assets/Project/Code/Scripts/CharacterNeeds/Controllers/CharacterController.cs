using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviourPun
{
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float motionSmoothTime = .1f;
    [SerializeField] private float rotateVelocity = .1f;
    [SerializeField] private Animator characterAnimator;

    #region Refs
    protected InteractionSystem CharacterInteractions => GetComponent<InteractionSystem>();
    protected CharacterStat CharacterStats => GetComponent<CharacterStat>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    #endregion

    public float RotationSpeed { get => rotationSpeed; }
    public float MotionSmoothTime { get => motionSmoothTime; }
    public float RotateVelocity { get => rotateVelocity; }

    public Animator CharacterAnimator { get => characterAnimator; }

    protected virtual void Update() => HandleMotionAnimation(Agent, CharacterAnimator, "MoveSpeed", MotionSmoothTime);

    #region Character Destination and motion handling, including rotation
    public void SetAgentDestination(Vector3 pos, NavMeshAgent agent)
    {
        agent.SetDestination(pos);
    }

    public void HandleMotionAnimation(NavMeshAgent agent, Animator animator, string animationFloatName, float smoothTime)
    {
        if (!Agent.hasPath)
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