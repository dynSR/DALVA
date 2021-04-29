using UnityEngine;

public class EntityAnimationsEventHandler : MonoBehaviour
{
    #region References
    public Animator MyAnimator => GetComponent<Animator>();
    private InteractionSystem Interactions => GetComponentInParent<InteractionSystem>();
    private EntityStats Stats => GetComponentInParent<EntityStats>();
    private CharacterController Controller => GetComponentInParent<CharacterController>();
    #endregion

    bool attackAnimationHasBeenChosen = false;

    private void Awake() => MyAnimator.runtimeAnimatorController = Stats.BaseUsedEntity.AnimatorController;

    #region Attack animation
    public void RangedAttack_AnimationEvent()
    {
        Interactions.RangedAttack();
        //PickAnAttackAnimation();
    }

    public void MeleeAttack_AnimationEvent()
    {
        Interactions.MeleeAttack();
        //PickAnAttackAnimation();
    }

    public void ResetAttackState_AnimationEvent()
    {
        Interactions.CanPerformAttack = true;
        Interactions.HasPerformedAttack = false;
        //attackAnimationHasBeenChosen = false;
    }

    //private void PickAnAttackAnimation()
    //{
    //    if (UtilityClass.ContainsParam(MyAnimator, "AttackAnimationValue"))
    //    {
    //        if (!attackAnimationHasBeenChosen)
    //        {
    //            if (MyAnimator.GetInteger("AttackAnimationValue") == 0 || MyAnimator.GetInteger("AttackAnimationValue") == 2)
    //            {
    //                MyAnimator.SetInteger("AttackAnimationValue", 1);
    //                attackAnimationHasBeenChosen = true;
    //            }
    //            else if (MyAnimator.GetInteger("AttackAnimationValue") == 1)
    //            {
    //                MyAnimator.SetInteger("AttackAnimationValue", 2);
    //                attackAnimationHasBeenChosen = true;
    //            }
    //        }
    //    }
    //}
    #endregion

    #region Abilities
    public void LockCharacterInPlaceBeforeCasting(int boolValue)
    {
        if (boolValue == 1)
            Controller.Agent.ResetPath();
    }

    public void SpawnAbilityEffect(int abilityNumber)
    {
        StartCoroutine(Stats.EntityAbilities[abilityNumber].ApplyAbilityEffectAtLocation(
            Stats.EntityAbilities[abilityNumber].CastLocation, 
            Stats.EntityAbilities[abilityNumber].Ability.AbilityEffectObject, 
            Stats.EntityAbilities[abilityNumber].Ability.DelayBeforeApplyingDamageOrEffect));
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
        if(!Controller.IsRooted)
            Controller.CanMove = false;
    }

    public void ResetMoveState_AnimationEvent()
    {
        if (!Controller.IsRooted)
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