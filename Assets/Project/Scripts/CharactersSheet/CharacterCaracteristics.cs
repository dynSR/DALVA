using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterClass { Warrior, Marksman, Mage, Healer }
public class CharacterCaracteristics : MonoBehaviour
{

    [Header("CORE PARAMETERS")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private List<Ability> characterAbilities;

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float healthRegeneration;
    [SerializeField] private float manaRegeneration;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float MaxMana { get => maxMana; set => maxMana = value; }
    public float HealthRegeneration { get => healthRegeneration; set => healthRegeneration = value; }
    public float ManaRegeneration { get => manaRegeneration; set => manaRegeneration = value; }
    public List<Ability> CharacterAbilities { get => characterAbilities; }

    public virtual void GetAllCharacterAbilities()
    {
        foreach (Ability abilityFound in GetComponents<Ability>())
        {
            CharacterAbilities.Add(abilityFound);
        }
    }

    public virtual void Awake()
    {
        GetAllCharacterAbilities();
    }
}
