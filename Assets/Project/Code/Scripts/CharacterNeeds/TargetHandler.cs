using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetHandler : MonoBehaviour
{
    [Header("TARGETS INFORMATIONS")]
    [SerializeField] private Transform target; //Est en publique pour debug
    [SerializeField] private Transform knownTarget;

    [Header("INTERACTIONS PARAMETERS")]
    [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private float rotationSpeed = 0.075f;

    public Transform Target { get => target; set => target = value; }
    public Transform KnownTarget { get => knownTarget; set => knownTarget = value; }

    public float distance;

    #region References
    protected CharacterStats CharacterStats => GetComponent<CharacterStats>();
    protected CharacterController CharacterController => GetComponent<CharacterController>();
    protected Animator CharacterAnimator => GetComponent<CharacterController>().CharacterAnimator;
    #endregion

    protected virtual void Update()
    {
        if (CharacterStats.IsDead)
        {
            Target = null;
            return;
        }

        SetTargetOnMouseClick();

        MoveTowardsAnExistingTarget(interactionRange);
    }

    public abstract void Interact();
    public abstract void ResetInteractionState();


    #region Set player's target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.RightClickIsPressed())
        {
            Target = null;

            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<EntityDetection>() != null && hit.collider.GetComponent<EntityDetection>().enabled)
                {
                    Target = hit.collider.transform;
                    ResetInteractionState();

                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
                        interactionRange = CharacterStats.UsedCharacter.BaseAttackRange;
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester)
                        interactionRange = 1.75f;
                }
                // Ground hit
                else
                {
                    CharacterController.Agent.isStopped = false;
                    CharacterController.Agent.stoppingDistance = 0.2f;
                    ResetInteractionState();
                }
            }
        }
    }
    #endregion

    #region Moving to a target
    void MoveTowardsAnExistingTarget(float minDistance)
    {
        if (Target != null)
        {
            CharacterController.HandleCharacterRotation(transform, Target.position, CharacterController.RotateVelocity, rotationSpeed);

            distance = Vector3.Distance(transform.position, Target.position);
            CharacterController.Agent.stoppingDistance = minDistance;

            if (distance > minDistance)
            {
                Debug.Log("Far from target");
                CharacterController.Agent.isStopped = false;
                CharacterController.Agent.SetDestination(Target.position);
            }
            else if (distance <= minDistance)
            {
                Debug.Log("Close enough to target");
                CharacterController.Agent.isStopped = true;
                Interact();
            }
        }
    }
    #endregion
}