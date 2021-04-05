using UnityEngine;

public class EntityAnimationsEventHandler : MonoBehaviour
{
    [Header("ABILITIES")]
    [SerializeField] private AbilityLogic abilityA;
    [SerializeField] private AbilityLogic abilityZ;
    [SerializeField] private AbilityLogic abilityE;
    [SerializeField] private AbilityLogic abilityR;

    public Animator MyAnimator => GetComponent<Animator>();
    private InteractionSystem Interactions => GetComponentInParent<InteractionSystem>();
    private EntityStats Stats => GetComponentInParent<EntityStats>();
    private CharacterController Controller => GetComponentInParent<CharacterController>();

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

    public void SpawnThirdAbilityEffect()
    {
        abilityE.ApplyAbilityAtLocation(abilityE.CastLocation, abilityE.Ability.AbilityEffectObject);
    }
    public void ResetThirdAbility()
    {
        abilityE.ResetAbilityAnimation("UsesThirdAbility");
    }

    public void SetCastingState_AnimationEvent()
    {
        Controller.IsCasting = true;
    }

    public void ResetCastingState_AnimationEvent()
    {
        Controller.IsCasting = false;
    }

    public void SetMoveState_AnimationEvent()
    {
        Controller.CanMove = false;
    }

    public void ResetMoveState_AnimationEvent()
    {
        Controller.CanMove = true;
    }
}