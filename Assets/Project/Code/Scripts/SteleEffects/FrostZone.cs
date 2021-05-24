using UnityEngine;

public class FrostZone : StatusEffectZoneCore
{
    [Range(0f, 0.75f)]
    [SerializeField] private float movementSpeedReduction = 0f;
    [Range(0f, 0.75f)]
    [SerializeField] private float attackSpeedReduction = 0f;

    public float MovementSpeedReduction { get => movementSpeedReduction; set => movementSpeedReduction = value; }
    public float AttackSpeedReduction { get => attackSpeedReduction; set => attackSpeedReduction = value; }

    protected override void ApplyAffect(EntityStats target)
    {
        target.GetStat(StatType.MovementSpeed).AddModifier(new StatModifier(-MovementSpeedReduction, StatType.MovementSpeed, StatModType.PercentAdd, this));
        target.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(-AttackSpeedReduction, StatType.AttackSpeed, StatModType.PercentAdd, this));

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
