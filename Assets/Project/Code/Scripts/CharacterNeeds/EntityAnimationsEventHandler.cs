using UnityEngine;

public class EntityAnimationsEventHandler : MonoBehaviour
{
    #region References
    public Animator MyAnimator => GetComponent<Animator>();
    private InteractionSystem Interactions => GetComponentInParent<InteractionSystem>();
    private EntityStats Stats => GetComponentInParent<EntityStats>();
    private CharacterController Controller => GetComponentInParent<CharacterController>();
    #endregion

    private void Awake() => MyAnimator.runtimeAnimatorController = Stats.UsedEntity.AnimatorController;

    #region Attack animation
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
    #endregion

    #region Abilities
    public void LockCharacterInPlaceBeforeCasting(int boolValue)
    {
        if (boolValue == 1)
            Controller.Agent.ResetPath();
    }

    public void SpawnAbilityEffect(int abilityNumber)
    {
        Stats.EntityAbilities[abilityNumber].ApplyAbilityEffectAtLocation
            (Stats.EntityAbilities[abilityNumber].CastLocation, Stats.EntityAbilities[abilityNumber].Ability.AbilityEffectObject);
    }

    public void SetAbilityAnimationToFalse(string animationName)
    {
        MyAnimator.SetBool(animationName, false);
        MyAnimator.SetLayerWeight(2, 0);
    }
    #endregion

    #region Casting State
    public void SetCastingState_AnimationEvent()
    {
        Controller.IsCasting = true;
    }

    public void ResetCastingState_AnimationEvent()
    {
        Controller.IsCasting = false;
    }
    #endregion

    #region Move State
    public void SetMoveState_AnimationEvent()
    {
        Controller.CanMove = false;
    }

    public void ResetMoveState_AnimationEvent()
    {
        Controller.CanMove = true;
    }
    #endregion

    #region On death - collider disabled and destroy object
    public void DesactivateCollider()
    {
        transform.parent.GetComponent<Collider>().enabled = false;
    }

    public void DestroyGameObject()
    {
        Destroy(transform.root.gameObject);
    }
    #endregion
}