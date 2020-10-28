using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AbilityType { Buff, Heal, Debuff, Projectile, CrowdControl, Movement, Shield } //A étoffer si besoin !

[CreateAssetMenu(fileName = "Ability_", menuName = "ScriptableObjects/Abilities", order = 1)]
public class Ability : ScriptableObject
{
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private AbilityType abilityType;
    [SerializeField] private GameObject abilityPrefab;

    [SerializeField] private float abilityCooldown;
    [SerializeField] private float abilityDamage;
    [SerializeField] private float abilityRange;
    [SerializeField] private float abilityAreaOfEffect;


    public string AbilityDescription { get => abilityDescription; }
    public string AbilityName { get => abilityName; }
    public AbilityType AbilityType { get => abilityType; }
    public GameObject AbilityPrefab { get => abilityPrefab; }

    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public float AbilityDamage { get => abilityDamage; set => abilityDamage = value; }
    public float AbilityRange { get => abilityRange; }
    public float AbilityAreaOfEffect { get => abilityAreaOfEffect; }
}
