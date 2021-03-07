using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Archer, Berzerk, Coloss, DaggerMaster, Mage, Priest, None }

[CreateAssetMenu(fileName = "Character_", menuName = "ScriptableObjects/Characters", order = 2)]
public class Character : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;
    public CharacterClass CharacterClass { get => characterClass; }

    [SerializeField] private CombatType combatType;

    #region Stats
    [Header("HEALTH")]
    [SerializeField] private float baseMaxHealth;
    [SerializeField] private float baseHealthRegeneration;
    public float BaseMaxHealth { get => baseMaxHealth; }
    public float BaseHealthRegeneration { get => baseHealthRegeneration; }

    [Header("COOLDOWN")]
    [SerializeField] private float baseCooldownReduction;
    [SerializeField] private float maxCooldownReduction;
    public float BaseCooldownReduction { get => baseCooldownReduction; }
    public float MaxCooldownReduction { get => maxCooldownReduction; }
    
    [Header("DEFENSES")]
    [SerializeField] private float baseArmor;
    [SerializeField] private float baseMagicResistance;
    public float BaseArmor { get => baseArmor; }
    public float BaseMagicResistance { get => baseMagicResistance; }

    [Header("RANGE")]
    [SerializeField] private float baseAttackRange;
    public float BaseAttackRange { get => baseAttackRange; }

    [Header("ATTACK SPEED")]
    [SerializeField] private float baseAttackSpeed = 0.625f;
    [SerializeField] private float maxAttackSpeed = 3f;
    public float BaseAttackSpeed { get => baseAttackSpeed; }
    public float MaxAttackSpeed { get => maxAttackSpeed; }

    [Header("DAMAGE")]
    [SerializeField] private float baseAttackDamage;
    [SerializeField] private float baseMagicDamage;
    public float BaseAttackDamage { get => baseAttackDamage; }
    public float BaseMagicDamage { get => baseMagicDamage; }

    [Header("CRITICAL STRIKES CHANCE")]
    [SerializeField] private float baseCriticalStrikeChance;
    public float BaseCriticalStrikeChance { get => baseCriticalStrikeChance; }

    [Header("BASE CRITICAL STRIKES DAMAGE MULTIPLIER")]
    [SerializeField] private float baseCriticalStrikeMultiplier = 175f;
    [SerializeField] private float maxCriticalStrikeMultiplier = 300f;
    public float BaseCriticalStrikeMultiplier { get => baseCriticalStrikeMultiplier; }
    public float MaxCriticalStrikeMultiplier { get => maxCriticalStrikeMultiplier; }
    #endregion

    [Header("ANIMATION")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
}