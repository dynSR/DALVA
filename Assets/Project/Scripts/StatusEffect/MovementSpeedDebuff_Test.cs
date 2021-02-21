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
        }
    }

    public override void RemoveStatusEffect()
    {
        if (GetTargetStatusEffectHandler(Target) != null)
        {
            if (!ValueAffectedResetsBeforeApplyingStatuEffect) return;

            if (ValueAffectedResetsBeforeApplyingStatuEffect)
                GetTargetCharacterController(Target).CurrentMoveSpeed = GetTargetCharacterController(Target).InitialMoveSpeed;
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