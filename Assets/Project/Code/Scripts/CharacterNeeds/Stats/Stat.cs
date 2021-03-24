using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    //CF Règles équipements > Légende caractéristiques
    Health, Health_Regeneration,
    Movement_Speed, Physical_Resistances, Magical_Resistances,
    Attack_Speed, Attack_Range, Physical_Power, Magical_Power,
    Adaptative_Penetration, Physical_Penetration, Magical_Penetration,
    Physical_Lifesteal, Magical_Lifesteal,
    Critical_Strike_Chance,
    Cooldown_Reduction, Heal_Shield_Effectiveness, Harmful_Effect_Reduction
}

[Serializable]
public class Stat
{
    public string Name;
    public StatType _StatType;

    public float BaseValue;
    public float CapValue;
    public float Value { get; set; }
    

    public List<StatModifier> statModifiers;

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
            }
        }

        if (CapValue > 0 && finalValue >= CapValue)
            finalValue = CapValue;

        Debug.Log(Value);

        return finalValue;
    }

    public void InitValue()
    {
        Value = BaseValue;
    }
}