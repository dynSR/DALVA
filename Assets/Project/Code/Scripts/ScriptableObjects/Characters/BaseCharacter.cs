using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass 
{ 
    Archer, 
    Berzerk, 
    Coloss, 
    DaggerMaster, 
    Mage, 
    Priest,
    Dummy,
    MeleeSbire,
    DistanceSbire,
    SpecialSbire,
    ForestMonster,
    Boss,
    Nexus,
    None 
}

[CreateAssetMenu(fileName = "Character_", menuName = "ScriptableObjects/Characters", order = 2)]
public class BaseCharacter : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string characterName = "[Type in]";
    [SerializeField] private CharacterClass characterClass = CharacterClass.None;
    public CharacterClass CharacterClass { get => characterClass; }

    [SerializeField] private CombatType combatType = CombatType.None;
    [SerializeField] private List<Stat> characterStats;

    [Header("ANIMATION")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
    public List<Stat> CharacterStats { get => characterStats; private set => characterStats = value; }

    private void OnValidate()
    {
        for (int i = 0; i < CharacterStats.Count; i++)
        {
            CharacterStats[i].StatType = (StatType)System.Enum.GetValues(typeof(StatType)).GetValue(i);
            CharacterStats[i].Name = CharacterStats[i].StatType.ToString();
        }
    }
}