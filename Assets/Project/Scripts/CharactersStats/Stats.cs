using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Warrior, Marksman, Mage, Healer }
enum DefaultAttackType { Melee, Ranged }
public class Stats : MonoBehaviour, IDamageable, IKillable
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

    [Header("UI PARAMETERS")]
    [SerializeField] private GameObject damagePopUp;
    [SerializeField] private GameObject deathHUD;

    private Vector3 InFrontOfCharacter => transform.position + new Vector3(0, 0, -0.25f);

    //Health
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    public float HealthRegeneration { get => healthRegeneration; set => healthRegeneration = value; }

    //Cooldown
    public float MaxCooldownReduction { get => maxCooldownReduction; set => maxCooldownReduction = value; }
    public float CurrentCooldownReduction { get => currentCooldownReduction; set => currentCooldownReduction = Mathf.Clamp(value, 0, MaxCooldownReduction); }
    public bool IsCooldownReductionCapped => CurrentCooldownReduction == MaxCooldownReduction;

    //Armor
    public float CurrentArmor { get; set; }
    public float CurrentMagicResistance { get; set; }

    //Attack
    public float CurrentAttackDamage { get; set; }
    public float CurrentMagicDamage { get; set; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float CurrentAttackSpeed { get; set; }
    public float CurrentCriticalStrikeChance { get; set; }
    
    //Death
    public bool IsDead => CurrentHealth <= 0;
    public float TimeToRespawn { get => timeToRespawn; set => timeToRespawn = value; }
    
    //Abilities
    public List<Ability> CharacterAbilities { get => characterAbilities; }
    

    protected virtual void Awake()
    {
        GetAllCharacterAbilities();
    }

    protected virtual void Start()
    {
        //deathHUD.SetActive(false);

        //CARACTERISTICS

        //- Health
        SetHealthAtStartOfTheGameIsEqualTo(MaxHealth);
        //- Defense
        SetArmorAtStartOfTheGameIsEqualTo(baseArmor);
        SetMagicResistanceAtStartOfTheGameIsEqualTo(baseMagicResistance);
        //- Cooldown
        SetCooldownReductionAtStartOfTheGameIsEqualTo(baseCooldownReduction);
        //- Attack
        SetAttackDamageAtStartOfTheGameIsEqualTo(baseAttackDamage);
        SetMagicDamageAtStartOfTheGameIsEqualTo(baseMagicDamage);
        SetAttackSpeedAtStartOfTheGameIsEqualTo(baseAttackSpeed);
        SetCriticalStrikeChanceAtStartOfTheGameIsEqualTo(baseCriticalStrikeChance);
    }

    protected virtual void Update()
    {
        if (IsDead)
            OnDeath();
    }

    #region Settings at start of the game
    private void SetHealthAtStartOfTheGameIsEqualTo(float characterMaxHealth)
    {
        CurrentHealth = characterMaxHealth;
    }

    private void SetArmorAtStartOfTheGameIsEqualTo(float armorValueAtStart)
    {
        CurrentArmor = armorValueAtStart;
    }

    private void SetMagicResistanceAtStartOfTheGameIsEqualTo(float magicResistanceValueAtStart)
    {
        CurrentMagicResistance = magicResistanceValueAtStart;
    }

    private void SetCooldownReductionAtStartOfTheGameIsEqualTo(float cooldownValueAtStart)
    {
        CurrentCooldownReduction = cooldownValueAtStart;
    }
    private void SetAttackDamageAtStartOfTheGameIsEqualTo(float attackDamageAtStart)
    {
        CurrentCooldownReduction = attackDamageAtStart;
    }

    private void SetMagicDamageAtStartOfTheGameIsEqualTo(float magicDamageAtStart)
    {
        CurrentCooldownReduction = magicDamageAtStart;
    }

    private void SetCriticalStrikeChanceAtStartOfTheGameIsEqualTo(float criticalStrikeChanceValueAtStart)
    {
        CurrentCooldownReduction = criticalStrikeChanceValueAtStart;
    }

    private void SetAttackSpeedAtStartOfTheGameIsEqualTo(float attackSpeedValueAtStart)
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

            DamagePopUp.Create(InFrontOfCharacter, damagePopUp, physicalDamageTaken, DamageType.Physical);
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
                DamagePopUp.Create(InFrontOfCharacter, damagePopUp, magicDamageTaken, DamageType.Magic);
        }

        CurrentHealth -= (int)physicalDamageTaken + (int)magicDamageTaken;

        Debug.Log("Health = " + CurrentHealth + " physical damage = " + (int)physicalDamageTaken + " magic damage = " + (int)magicDamageTaken);
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float damageTaken, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);

        DamagePopUp.Create(InFrontOfCharacter, damagePopUp, damageTaken, damageType);
    }

    #region Death and respawn
    public virtual void OnDeath()
    {
        //Bloquer les inputs pour les compétences

        //Afficher le HUD de mort pendant le temps de la mort
        //deathHUD.SetActive(true);

        //Start une coroutine de respawn -> après un temps t le personnage réapparaît à son point de spawn
        StartCoroutine(Respawn(TimeToRespawn));
    }

    protected IEnumerator Respawn(float timeBeforeRespawn)
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        //Respawn
        //Set Position At Spawn Location
        CurrentHealth = MaxHealth;
        //deathHUD.SetActive(false);
        Debug.Log("is Dead " + IsDead);
    }
    #endregion
}
