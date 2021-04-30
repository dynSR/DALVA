using UnityEngine;

public class FrostZone : StatusEffectZoneCore
{
    [Range(0f, 0.75f)]
    [SerializeField] private float movementSpeedReduction = 0f;
    [Range(0f, 0.75f)]
    [SerializeField] private float attackSpeedReduction = 0f;

    protected override void ApplyAffect(EntityStats target)
    {
        target.GetStat(StatType.MovementSpeed).AddModifier(new StatModifier(-movementSpeedReduction, StatType.MovementSpeed, StatModType.PercentAdd, this));
        target.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(-attackSpeedReduction, StatType.AttackSpeed, StatModType.PercentAdd, this));

        GetTargetController(target).Agent.speed = target.GetStat(StatType.MovementSpeed).Value;
    }

    protected override void RemoveEffect(EntityStats target)
    {
        target.GetStat(StatType.MovementSpeed).RemoveAllModifiersFromSource(this);
        target.GetStat(StatType.AttackSpeed).RemoveAllModifiersFromSource(this);

        GetTargetController(target).Agent.speed = target.GetStat(StatType.MovementSpeed).Value;
    }

    CharacterController GetTargetController(EntityStats target)
    {
        return target.GetComponent<CharacterController>();
    }
}
