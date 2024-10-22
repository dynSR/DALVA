﻿using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using DarkTonic.MasterAudio;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterController : MonoBehaviourPun
{
    public delegate void StunStateHandler();
    public event StunStateHandler OnTargetStunned;

    [Header("CONTROLLER ATTRIBUTES VALUE")]
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float motionSmoothTime = .1f;
    [SerializeField] private float rotateVelocity = .1f;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private bool canMove = true;
    private bool isCasting = false;
    [SerializeField] private bool isStunned = false;
    [SerializeField] private bool isRooted = false;

    [Header("VFX")]
    [SerializeField] private GameObject stunVFX;
    [SerializeField] private GameObject rootedVFX;
    [SerializeField] private GameObject[] slowedVFX;
    [SerializeField] private GameObject burningVFX;
    [SerializeField] private GameObject poisonnedVFX;
    public GameObject StunVFX { get => stunVFX; }
    public GameObject RootedVFX { get => rootedVFX; }

    #region Refs
    protected InteractionSystem Interactions => GetComponent<InteractionSystem>();
    public EntityStats Stats => GetComponent<EntityStats>();
    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    #endregion

    public float RotationSpeed { get => rotationSpeed; }
    public float MotionSmoothTime { get => motionSmoothTime; }
    public float RotateVelocity { get => rotateVelocity; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsCasting { get => isCasting; set => isCasting = value; }
    public bool IsStunned { get => isStunned; set => isStunned = value; }
    public bool IsRooted { get => isRooted; set => isRooted = value; }

    public Animator CharacterAnimator { get => characterAnimator; }

    protected virtual void Update()
    {
        HandleMotionAnimation(Agent, CharacterAnimator, "MoveSpeed", MotionSmoothTime);
    }

    #region Character Destination and motion handling, including rotation
    public void SetNavMeshAgentSpeed(NavMeshAgent agent, float value)
    {
        agent.speed = value;
    }

    public void SetAgentDestination(NavMeshAgent agent, Vector3 pos)
    {
        if (!CanMove || IsStunned || IsRooted) return;

        agent.destination = pos;
        //agent.SetDestination(pos);
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

    public void HandleCharacterRotation(Transform transform)
    {
        if (IsCasting || IsStunned) return;

        if (Agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(Agent.velocity.normalized);
        }
    }

    public void HandleCharacterRotationBeforeCasting(Transform transform, Vector3 target, float rotateVelocity, float rotationSpeed)
    {
        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            rotationSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    public void StunTarget()
    {
        OnTargetStunned?.Invoke();

        if (Agent.enabled)
            Agent.ResetPath();

        Interactions.ResetInteractionState();
        Interactions.CanPerformAttack = false;

        IsStunned = true;

        StunVFX.SetActive(true);

        Interactions.IsAttacking = false;
    }
    public void UnStunTarget()
    {
        IsStunned = false;
        StunVFX.SetActive(false);
    }

    public void RootTarget()
    {
        Agent.ResetPath();
        IsRooted = true;
        RootedVFX.SetActive(true);
    }

    public void UnRootTarget()
    {
        IsRooted = false;
        RootedVFX.SetActive(false);
    }

    #region VFX Toggle
    public void ActivateSlowVFX()
    {
        foreach (var item in slowedVFX)
        {
            if(!item.activeInHierarchy)
                item.SetActive(true);
        }
    }

    public void DeactivateSlowVFX()
    {
        foreach (var item in slowedVFX)
        {
            if (item.activeInHierarchy)
                item.SetActive(false);
        }
    }

    public void ActivateBurnVFX()
    {
        if (!burningVFX.activeInHierarchy)
            burningVFX.SetActive(true);
    }

    public void DeactivateBurnVFX()
    {
        if (burningVFX.activeInHierarchy)
            burningVFX.SetActive(false);
    }

    public void ActivatePoisonVFX()
    {
        if (!poisonnedVFX.activeInHierarchy)
            poisonnedVFX.SetActive(true);
    }

    public void DeactivatePoisonVFX()
    {
        if (poisonnedVFX.activeInHierarchy)
            poisonnedVFX.SetActive(false);
    }
    #endregion
    #endregion
}