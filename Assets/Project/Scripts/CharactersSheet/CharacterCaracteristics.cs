using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Warrior, Marksman, Mage, Healer }
public class CharacterCaracteristics : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float healthRegeneration;
    [SerializeField] private float manaRegeneration;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float MaxMana { get => maxMana; set => maxMana = value; }
    public float HealthRegeneration { get => healthRegeneration; set => healthRegeneration = value; }
    public float ManaRegeneration { get => manaRegeneration; set => manaRegeneration = value; }
}
