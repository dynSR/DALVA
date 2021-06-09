using System.Collections.Generic;
using UnityEngine;

public enum EntityType 
{ 
    Warrior,
    Sorcerer,
    Dummy,
    Minion,
    ForestMonster,
    Boss,
    Stele, SteleEffect,
    None
}

public enum Species
{
    None,
    Bear, 
    Boar,
    Cougar,
    Deer,
    Moose,
    Rabbit,
    Raccoon,
    Wolf,
}

[CreateAssetMenu(fileName = "Entity_", menuName = "ScriptableObjects/Entities", order = 2)]
public class BaseEntity : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string entityName = "[Type in]";
    [SerializeField] private EntityType entityType = EntityType.None;
    [SerializeField] private Species entitySpecies = Species.None;
    public EntityType EntityType { get => entityType; set => entityType = value; }
    public Species EntitySpecies { get => entitySpecies; set => entitySpecies = value; }

    [SerializeField] private CombatType combatType = CombatType.None;
    [SerializeField] private List<Stat> entityStats;

    [Header("ANIMATION")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
    public List<Stat> EntityStats { get => entityStats; private set => entityStats = value; }

    private void OnValidate()
    {
        for (int i = 0; i < EntityStats.Count; i++)
        {
            if (EntityStats.Count == System.Enum.GetValues(typeof(StatType)).Length)
                EntityStats[i].StatType = (StatType)System.Enum.GetValues(typeof(StatType)).GetValue(i);

            EntityStats[i].Name = EntityStats[i].StatType.ToString() + " - " + EntityStats[i].BaseValue.ToString();
        }
    }
}