using UnityEngine;

public class HasteEffectLogic : StatusEffectLogic
{
    [SerializeField] private float multiplicator;
    [SerializeField] private float valueToAdd;
    [Tooltip("If false, the value will be multiplicated by the value multiplicator.")]
    [SerializeField] private bool addsToStat;

    protected override void ApplyStatusEffectOnTarget(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            if (GetTargetStatusEffectHandler(target).IsEffectAlreadyApplied(this)) return;

            if (!addsToStat)
                GetTargetCharacterStats(target).CurrentMovementSpeed *= multiplicator;
            if (addsToStat)
                GetTargetCharacterStats(target).CurrentMovementSpeed += valueToAdd;

            GetTargetStatusEffectHandler(target).AddNewEffect(this);
        }
    }

    public override void RemoveEffect()
    {
        if (GetTargetStatusEffectHandler(Target) != null)
            GetTargetCharacterStats(Target).CurrentMovementSpeed = GetTargetCharacterStats(Target).UsedCharacter.BaseMovementSpeed 
                + GetTargetCharacterStats(Target).AdditiveMovementSpeed;
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