using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedBuff_Test : StatusEffect
{
    protected override void ApplyStatusEffectOnTarget(Transform target)
    {
        if (GetTargetStatusEffectHandler(target).IsDurationOfStatusEffectAlreadyApplied(this)) return;

        base.CheckForExistingStatusEffect(GetTargetStatusEffectHandler(target));

        GetTargetCharacterController(target).CurrentSpeed *= 2;

        GetTargetStatusEffectHandler(target).ApplyNewStatusEffectDuration(this);
    }

    public override void RemoveStatusEffect()
    {
        if (!DoStatusEffectResetTheValueAffectedToInitialValueBeforeApplying) return;

        if (DoStatusEffectResetTheValueAffectedToInitialValueBeforeApplying)
            GetTargetCharacterController(Target).CurrentSpeed = GetTargetCharacterController(Target).InitialSpeed;
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
