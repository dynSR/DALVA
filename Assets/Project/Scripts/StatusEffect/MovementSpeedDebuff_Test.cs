using UnityEngine;

public class MovementSpeedDebuff_Test : StatusEffect
{
    protected override void ApplyStatusEffectOnTarget(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            if (GetTargetStatusEffectHandler(target).IsTheDurationOfStatusEffectOver(this)) return;

            base.CheckForExistingStatusEffect(GetTargetStatusEffectHandler(target));

            GetTargetCharacterController(target).CurrentMoveSpeed /= 2;

            GetTargetStatusEffectHandler(target).ApplyNewStatusEffect(this);

            GetTargetStatusEffectHandler(target).GetComponentInChildren<Animator>().SetTrigger("IsSlowed");
        }
    }

    public override void RemoveStatusEffect()
    {
        if (GetTargetStatusEffectHandler(Target) != null)
        {
            if (!ValueAffectedResetsBeforeApplyingStatuEffect) return;

            if (ValueAffectedResetsBeforeApplyingStatuEffect)
            {
                GetTargetCharacterController(Target).CurrentMoveSpeed = GetTargetCharacterController(Target).InitialMoveSpeed;
                GetTargetStatusEffectHandler(Target).GetComponentInChildren<Animator>().SetTrigger("IsSlowed");
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}