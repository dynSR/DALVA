using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Warrior, Marksman, Mage, Healer }
enum DefaultAttackType { Melee, Distance }
public class Character : MonoBehaviour, IDamageable, IKillable
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private List<Ability> characterAbilities;
    [SerializeField] private DefaultAttackType defaultAttackType;

    [Header("HEALTH PARAMETERS")]
    private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegeneration;

    [Header("COOLDOWN PARAMETERS")]
    private float currentCooldownReduction;
    [SerializeField] private float baseCooldownReduction;
    [SerializeField] private float maxCooldownReduction;

    [Header("DEFENSE PARAMETERS")]
    [SerializeField] private float baseArmor;
    [SerializeField] private float baseMagicResistance;

    [Header("ATTACK PARAMETERS")]
    [SerializeField] private float baseAttackDamage;
    [SerializeField] private float baseMagicDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float baseAttackSpeed;
    [SerializeField] private float baseCriticalStrikeChance;

    [Header("DEATH PARAMETERS")]
    [SerializeField] private float timeToRespawn;

    [Header("DAMAGE POPUP PARAMETERS")]
    [SerializeField] private GameObject damagePopUp;
    private Vector3 inFrontOfCharacter => transform.position + new Vector3(-0.5f, 0, 0.45f);

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    public float HealthRegeneration { get => healthRegeneration; set => healthRegeneration = value; }

    public float MaxCooldownReduction { get => maxCooldownReduction; set => maxCooldownReduction = value; }
    public float CurrentCooldownReduction { get => currentCooldownReduction; set => currentCooldownReduction = Mathf.Clamp(value, 0, MaxCooldownReduction); }

    public float CurrentArmor { get; set; }
    public float CurrentMagicResistance { get; set; }

    public float CurrentAttackDamage { get; set; }
    public float CurrentMagicDamage { get; set; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float CurrentAttackSpeed { get; set; }
    public float CurrentCriticalStrikeChance { get; set; }
    
    public bool IsDead => CurrentHealth <= 0;
    public bool IsCooldownReductionCapped => CurrentCooldownReduction == MaxCooldownReduction;

    public List<Ability> CharacterAbilities { get => characterAbilities; }
    public float TimeToRespawn { get => timeToRespawn; set => timeToRespawn = value; }
    public Transform Target { get; set; }

    protected virtual void Awake()
    {
        GetAllCharacterAbilities();
    }

    protected virtual void Start()
    {
        //Health
        SetHealthAtStartOfTheGame(MaxHealth);
        //Defense
        SetArmorAtStartOfTheGame(baseArmor);
        SetMagicResistanceAtStartOfTheGame(baseMagicResistance);
        //Misc
        SetCooldownReductionAtStartOfTheGame(baseCooldownReduction);
        //Attack
        SetAttackDamageAtStartOfTheGame(baseAttackDamage);
        SetMagicDamageAtStartOfTheGame(baseMagicDamage);
        SetAttackSpeedAtStartOfTheGame(baseAttackSpeed);
        SetCriticalStrikeChanceAtStartOfTheGame(baseCriticalStrikeChance);
    }

    #region Settings at start of the game
    private void SetHealthAtStartOfTheGame(float characterMaxHealth)
    {
        CurrentHealth = characterMaxHealth;
    }

    private void SetArmorAtStartOfTheGame(float armorValueAtStart)
    {
        CurrentArmor = armorValueAtStart;
    }

    private void SetMagicResistanceAtStartOfTheGame(float magicResistanceValueAtStart)
    {
        CurrentMagicResistance = magicResistanceValueAtStart;
    }

    private void SetCooldownReductionAtStartOfTheGame(float cooldownValueAtStart)
    {
        CurrentCooldownReduction = cooldownValueAtStart;
    }
    private void SetAttackDamageAtStartOfTheGame(float attackDamageAtStart)
    {
        CurrentCooldownReduction = attackDamageAtStart;
    }

    private void SetMagicDamageAtStartOfTheGame(float magicDamageAtStart)
    {
        CurrentCooldownReduction = magicDamageAtStart;
    }

    private void SetCriticalStrikeChanceAtStartOfTheGame(float criticalStrikeChanceValueAtStart)
    {
        CurrentCooldownReduction = criticalStrikeChanceValueAtStart;
    }

    private void SetAttackSpeedAtStartOfTheGame(float attackSpeedValueAtStart)
    {
        CurrentCooldownReduction = attackSpeedValueAtStart;
    }

    private void GetAllCharacterAbilities()
    {
        foreach (Ability abilityFound in GetComponents<Ability>())
        {
            CharacterAbilities.Add(abilityFound);
        }
    }
    #endregion

    public virtual void TakeDamage(float physicalDamageTaken, float magicDamageTaken)
    {
        if (physicalDamageTaken > 0)
        {
            if (CurrentArmor <= 0)
            {
                physicalDamageTaken *= 2 - 100 / (100 - CurrentArmor);
                Debug.Log("Armor is equal or inferior to 0 / " + " Physical Damage " + (int)physicalDamageTaken);
            }
            else
            {
                physicalDamageTaken *= 100 / (100 + CurrentArmor);
                Debug.Log("Armor is over 0 / " + " Physical Damage " + (int)physicalDamageTaken);
            }

            DamagePopUp.Create(inFrontOfCharacter, damagePopUp, physicalDamageTaken, DamageType.Physical);
        }

        if (magicDamageTaken > 0)
        {
            if (CurrentMagicResistance <= 0)
            {
                magicDamageTaken *= 2 - 100 / (100 - CurrentMagicResistance);
                Debug.Log("Magic Resistance is equal or inferior to 0 / " + " Magic Damage " + (int)magicDamageTaken);

            }
            else
            {
                magicDamageTaken *= 100 / (100 + CurrentMagicResistance);
                Debug.Log("Magic Resistance is over 0 / " + " Magic Damage " + (int)magicDamageTaken);
            }

            if (physicalDamageTaken > 0) 
                StartCoroutine(CreateDamagePopUpWithDelay(0.15f, magicDamageTaken, DamageType.Magic));
            else
                DamagePopUp.Create(inFrontOfCharacter, damagePopUp, magicDamageTaken, DamageType.Magic);
        }

        CurrentHealth -= (int)physicalDamageTaken + (int)magicDamageTaken;

        Debug.Log("Health = " + CurrentHealth + " physical damage = " + (int)physicalDamageTaken + " magic damage = " + (int)magicDamageTaken);
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float damageTaken, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);

        DamagePopUp.Create(inFrontOfCharacter, damagePopUp, damageTaken, damageType);
    }

    #region Death and respawn
    public virtual void OnDeath()
    {
        //Bloquer les inputs pour les compétences

        //Afficher le HUD de mort pendant le temps de la mort

        //Start une coroutine de respawn -> après un temps t le personnage réapparaît à son point de spawn
        //StartCoroutine(Respawn(TimeToRespawn));
    }

    protected IEnumerator Respawn(float timeBeforeRespawn)
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        //Respawn
    }
    #endregion
}
