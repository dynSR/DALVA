using UnityEngine;

public class WeaknessZone : StatusEffectZoneCore
{
    [Range(0f, 10f)]
    [SerializeField] private float physicalResistancesReduction = 0f;
    [Range(0f, 10f)]
    [SerializeField] private float magicalResistancesReduction = 0f;
    [SerializeField] private float increasedDamageTakenValue = 0.15f;

    public float PhysicalResistancesReduction { get => physicalResistancesReduction; set => physicalResistancesReduction = value; }
    public float MagicalResistancesReduction { get => magicalResistancesReduction; set => magicalResistancesReduction = value; }

    public bool CanApplyIncreasedDamageTaken = false;

    protected override void ApplyAffect(EntityStats target)
    {
        target.Controller.ActivatePoisonVFX();
        target.GetStat(StatType.PhysicalResistances).AddModifier(new StatModifier(-PhysicalResistancesReduction, StatType.PhysicalResistances, StatModType.PercentAdd, this));
        target.GetStat(StatType.MagicalResistances).AddModifier(new StatModifier(-MagicalResistancesReduction, StatType.MagicalResistances, StatModType.PercentAdd, this));

        if (CanApplyIncreasedDamageTaken)
        {
            target.GetStat(StatType.IncreasedDamageTaken).AddModifier(new StatModifier(increasedDamageTakenValue, StatType.IncreasedDamageTaken, StatModType.Flat, this));
        }
    }

    protected override void RemoveEffect(EntityStats target)
    {
        target.Controller.DeactivatePoisonVFX();
        target.GetStat(StatType.PhysicalResistances).RemoveAllModifiersFromSource(this);
        target.GetStat(StatType.MagicalResistances).RemoveAllModifiersFromSource(this);

        if (CanApplyIncreasedDamageTaken)
        {
            target.GetStat(StatType.IncreasedDamageTaken).RemoveAllModifiersFromSource(this);
        }
    }
}