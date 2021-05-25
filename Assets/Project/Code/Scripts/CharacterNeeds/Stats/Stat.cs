using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    //CF Règles équipements > Légende caractéristiques
    Health, HealthRegeneration, Shield,
    MovementSpeed, PhysicalResistances, MagicalResistances,
    AttackSpeed, AttackRange, PhysicalPower, MagicalPower, BonusPhysicalPower, BonusMagicalPower,
    AdaptativePenetration, PhysicalPenetration, MagicalPenetration,
    PhysicalLifesteal, MagicalLifesteal,
    CriticalStrikeChance,
    Cooldown_Reduction, HealAndShieldEffectiveness, HarmfulEffectReduction,
    RessourcesGiven,
    IncreasedDamageTaken, DamageReduction,
    EquipementCostReduction, BonusDamageAgaintBuildings
}

[Serializable]
public class Stat
{
    [Header("INFORMATIONS")]
    [SerializeField] private string name;
    [SerializeField] private StatType statType;
    [SerializeField] private Sprite icon = null;

    [Header("VALUES")]
    [SerializeField] private float baseValue = 0f;
    [SerializeField] private float capValue = 0f;
    public float Value; /*{ get; set; }*/ //debug
    public float MaxValue; /*{ get; set; }*/ //debug

    #region Public variables
    public List<StatModifier> statModifiers;

    public string Name { get => name; set => name = value; }
    public StatType StatType { get => statType; set => statType = value; }
    public float BaseValue { get => baseValue; set => baseValue = value; }
    public float CapValue { get => capValue; }
    public Sprite Icon { get => icon; set => icon = value; }
    #endregion

    public void AddModifier(StatModifier mod)
    {
        statModifiers.Add(mod);
        Value = CalculateValue();
    }

    public void RemoveModifier(StatModifier mod)
    {
        statModifiers.Remove(mod);

        Value = CalculateValue();
    }

    public void RemoveAllModifiersFromSource(object source)
    {
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                statModifiers.RemoveAt(i);
            }

            Value = CalculateValue();
            MaxValue = Value;
        }
    }

    public float CalculateValue()
    {
        float finalValue = BaseValue;

        if (statModifiers.Count > 0)
        {
            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    finalValue *= 1 + mod.Value;
                }

                MaxValue = finalValue;
            }
        }

        if (CapValue > 0 && finalValue >= CapValue)
            finalValue = CapValue;

        //if (finalValue >= MaxValue)
        //    finalValue = MaxValue;

        //Debug.Log(Value);

        return finalValue;
    }

    public void InitValue()
    {
        Value = BaseValue;
        MaxValue = Value;
    }
}