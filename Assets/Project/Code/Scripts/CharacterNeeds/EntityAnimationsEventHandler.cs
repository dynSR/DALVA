using UnityEngine;

public class EntityAnimationsEventHandler : MonoBehaviour
{
    public Animator MyAnimator => GetComponent<Animator>();
    private InteractionSystem Interactions => GetComponentInParent<InteractionSystem>();
    private EntityStats Stats => GetComponentInParent<EntityStats>();
    private VisibilityState VisibilityState => GetComponentInParent<VisibilityState>();

    private void Awake()
    {
        MyAnimator.runtimeAnimatorController = Stats.UsedEntity.AnimatorController;
    }

    public void RangedAttack_AnimationEvent()
    {
        Interactions.RangedAttack();
    }

    public void MeleeAttack_AnimationEvent()
    {
        Interactions.MeleeAttack();
    }

    public void ResetAttackState_AnimationEvent()
    {
        Interactions.CanPerformAttack = true;
        Interactions.HasPerformedAttack = false;
    }

    public void SetEntityToInvisible_AnimationEvent()
    {
        VisibilityState.SetToInvisible();
    }
}