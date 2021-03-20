using UnityEngine;

public class SlowEffectLogic : StatusEffectLogic
{
    [SerializeField] private float divisor;
    [SerializeField] private float valueToTakeOff;
    [Tooltip("If false, the value will be divied by the value divisor.")]
    [SerializeField] private bool takesOffToStat;

    protected override void ApplyStatusEffectOnTarget(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            if (GetTargetStatusEffectHandler(target).IsEffectAlreadyApplied(this)) return;

            if (!takesOffToStat)
                GetTargetCharacterStats(Target).GetStat(StatType.Movement_Speed).Value /= divisor;
            if (takesOffToStat)
                GetTargetCharacterStats(Target).GetStat(StatType.Movement_Speed).Value -= valueToTakeOff;

            GetTargetStatusEffectHandler(target).AddNewEffect(this);
        }
    }

    public override void RemoveEffect()
    {
        if (GetTargetStatusEffectHandler(Target) != null)
        {
            GetTargetCharacterStats(Target).GetStat(StatType.Movement_Speed).Value = GetTargetCharacterStats(Target).GetStat(StatType.Movement_Speed).CalculateValue();
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