using UnityEngine;

public enum StatModType
{
    Flat = 100,
    PercentAdd = 200
}

[System.Serializable]
public class StatModifier
{
    public float Value;
    public StatType StatType;
    public StatModType Type;
    public readonly int Order;
    public object Source;

    public StatModifier(float value, StatType statType, StatModType type, int order, object source)
    {
        Value = value;
        StatType = statType;
        Type = type;
        Order = order;
        Source = source;
    }

    //Contrustor that requires only to reference a value and a mod type, order is handled by enum int value
    public StatModifier(float value, StatType stat, StatModType type) : this(value, stat, type, (int)type, null) { }
    public StatModifier(float value, StatType stat, StatModType type, int order) : this(value, stat, type, order, null) { }
    public StatModifier(float value, StatType stat, StatModType statType, object source) : this (value, stat, statType, (int)statType, source) { }
}