﻿using DarkTonic.MasterAudio;
using UnityEngine;

public class EntityAnimationsEventHandler : MonoBehaviour
{
    #region References
    public Animator MyAnimator => GetComponent<Animator>();
    private InteractionSystem Interactions => GetComponentInParent<InteractionSystem>();
    private EntityStats Stats => GetComponentInParent<EntityStats>();
    private CharacterController Controller => GetComponentInParent<CharacterController>();
    private EntityDetection EDetection => GetComponentInParent<EntityDetection>();

    private EntityStats queuedTargetStats;
    #endregion

    [Header("VFX ANIMATION")]
    public GameObject meleeAttackEffect;

    [Header("SFX ANIMATION")]
    [SoundGroup][SerializeField] private string meleeAttackSound;

    private void Awake()
    {
        if (Stats != null)
            MyAnimator.runtimeAnimatorController = Stats.BaseUsedEntity.AnimatorController;
    }

    public void SetQueuedTargetAsTarget()
    {
        if (Interactions.QueuedTarget != null)
        {
            queuedTargetStats = Interactions.QueuedTarget.GetComponent<EntityStats>();

            if (queuedTargetStats != null && !queuedTargetStats.IsDead)
            {
                Interactions.IsAttacking = false;
                Interactions.Target = Interactions.QueuedTarget;
                Interactions.QueuedTarget = null;

                if (MyAnimator.GetLayerWeight(1) == 0)
                {
                    MyAnimator.SetLayerWeight(1, 1);
                }
            }
        }
    }

    #region Attack animation
    public void RangedAttack_AnimationEvent()
    {
        Interactions.RangedAttack();
    }

    public void MeleeAttack_AnimationEvent()
    {
        Interactions.MeleeAttack();

        if (meleeAttackEffect != null)
            Instantiate(meleeAttackEffect, Interactions.Target.position, meleeAttackEffect.transform.rotation);
    }

    public void ResetAttackState_AnimationEvent()
    {
        Interactions.CanPerformAttack = true;
        Interactions.HasPerformedAttack = false;
    }

    public void PlayMeleeAttackSound()
    {
        switch (Stats.BaseUsedEntity.EntitySpecies)
        {
            case Species.None:
                UtilityClass.PlaySoundGroupImmediatly(meleeAttackSound, transform);
                break;
            case Species.Bear:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Bear_Attack", transform);
                break;
            case Species.Boar:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Boar_Attack", transform);
                break;
            case Species.Cougar:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Cougar_Attack", transform);
                break;
            case Species.Deer:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Deer_Attack", transform);
                break;
            case Species.Moose:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Moose_Attack", transform);
                break;
            /*case Species.Rabbit:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Rabbit_Attack", transform);
                break;*/
            case Species.Raccoon:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Raccoon_Attack", transform);
                break;
            case Species.Tiger:
                UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Tiger_Attack", transform);
                break;
        }
    }
    #endregion

    #region Abilities
    public void LockCharacterInPlaceBeforeCasting(int boolValue)
    {
        if (boolValue == 1 && Controller.Agent.enabled)
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

        SetQueuedTargetAsTarget();
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
        if(EDetection.TypeOfEntity != TypeOfEntity.Monster)
         Destroy(transform.root.gameObject);
    }
    #endregion
}