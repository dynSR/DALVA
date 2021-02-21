using UnityEngine;

public class MovementSpeedBuff_Test : StatusEffect
{
    protected override void ApplyStatusEffectOnTarget(Transform myTarget)
    {
        if (GetTargetStatusEffectHandler(myTarget) != null)
        {
            if (GetTargetStatusEffectHandler(myTarget).IsTheDurationOfStatusEffectOver(this)) return;

            base.CheckForExistingStatusEffect(GetTargetStatusEffectHandler(myTarget));

            GetTargetCharacterController(myTarget).CurrentMoveSpeed *= 2;

            GetTargetStatusEffectHandler(myTarget).ApplyNewStatusEffect(this);
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