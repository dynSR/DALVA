using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessZone : StatusEffectZoneCore
{
    [Range(0f, 10f)]
    [SerializeField] private float physicalResistancesReduction = 0f;
    [Range(0f, 10f)]
    [SerializeField] private float magicalResistancesReduction = 0f;

    public float PhysicalResistancesReduction { get => physicalResistancesReduction; set => physicalResistancesReduction = value; }
    public float MagicalResistancesReduction { get => magicalResistancesReduction; set => magicalResistancesReduction = value; }

    protected override void ApplyAffect(EntityStats target)
    {
        target.GetStat(StatType.PhysicalResistances).AddModifier(new StatModifier(-PhysicalResistancesReduction, StatType.PhysicalResistances, StatModType.PercentAdd, this));
        target.GetStat(StatType.MagicalResistances).AddModifier(new StatModifier(-MagicalResistancesReduction, StatType.MagicalResistances, StatModType.PercentAdd, this));
    }

    protected override void RemoveEffect(EntityStats target)
    {
        target.GetStat(StatType.PhysicalResistances).RemoveAllModifiersFromSource(this);
        target.GetStat(StatType.MagicalResistances).RemoveAllModifiersFromSource(this);
    }
}