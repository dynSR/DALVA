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
    None }

[CreateAssetMenu(fileName = "Character_", menuName = "ScriptableObjects/Characters", order = 2)]
public class BaseCharacter : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;
    public CharacterClass CharacterClass { get => characterClass; }

    [SerializeField] private CombatType combatType;
    [SerializeField] private Stat[] characterStats;

    [Header("RESSOURCES")]
    [SerializeField] private float baseRessourcesGiven = 50f;
    public float BaseRessourcesGiven { get => baseRessourcesGiven; }

    [Header("ANIMATION")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
    public Stat[] CharacterStats { get => characterStats; }

    private void OnValidate()
    {
        for (int i = 0; i < CharacterStats.Length; i++)
        {
            if (CharacterStats.Length == System.Enum.GetValues(typeof(StatType)).Length)
            {
                CharacterStats[i]._StatType = (StatType)System.Enum.GetValues(typeof(StatType)).GetValue(i);
            }

            CharacterStats[i].Name = CharacterStats[i]._StatType.ToString();
        }
    }
}