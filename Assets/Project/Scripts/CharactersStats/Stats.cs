using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Warrior, Ranger, Mage }
public class Stats : MonoBehaviour, IDamageable, IKillable
{
    [Header("CHARACTER NAME & CLASSE")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;

    [Header("CHARACTER ABILITIES AND DEFAULT ATTACK TYPE")]
    [SerializeField] private List<Ability> characterAbilities;

    #region Defensive & Utilitary Stats
    [Header("HEALTH")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealthRegeneration;
    [SerializeField] private float currentHealth; //Set to private after Debug
    #region Health Public Variables
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    public float CurrentHealthRegeneration { get => currentHealthRegeneration; set => currentHealthRegeneration = value; }
    #endregion

    [Header("COOLDOWN")]
    [SerializeField] private float baseCooldownReduction;
    [SerializeField] private float maxCooldownReduction;
    [SerializeField] private float currentCooldownReduction; //Set to private after Debug
    #region Cooldown Public Variables
    public float MaxCooldownReduction { get => maxCooldownReduction; set => maxCooldownReduction = value; }
    public float CurrentCooldownReduction
    {
        get => currentCooldownReduction;
        set
        {
            if (currentCooldownReduction > MaxCooldownReduction)
            {
                currentCooldownReduction = MaxCooldownReduction;
            }
            else
            {
                currentCooldownReduction = Mathf.Clamp(value, 0, MaxCooldownReduction);
            }
        }
    }
    public bool IsCooldownReductionCapped => CurrentCooldownReduction == MaxCooldownReduction;
    #endregion

    [Header("DEFENSE")]
    [SerializeField] private float baseArmor;
    [SerializeField] private float baseMagicResistance;
    #region Resistances Public Variables
    public float CurrentArmor { get; set; }
    public float CurrentMagicResistance { get; set; }
    #endregion
    #endregion

    #region Offensive Stats
    [Header("BASE DAMAGE")]
    [SerializeField] private float baseAttackDamage;
    [SerializeField] private float baseMagicDamage;
    [SerializeField] private float currentAttackDamage; //Set to private after Debug
    [SerializeField] private float currentMagicDamage; //Set to private after Debug
    #region Damages Public Variables
    public float CurrentAttackDamage { get => currentAttackDamage; set => currentAttackDamage = value; }
    public float CurrentMagicDamage { get; set; }
    #endregion

    [Header("ATTACK RANGE")]
    [SerializeField] private float attackRange;
    #region AttackRange Public Variables
    public float AttackRange { get => attackRange; set => attackRange = value; }
    #endregion

    [Header("ATTACK SPEED")]
    [SerializeField] private float baseAttackSpeed;
    [SerializeField] private float currentAttackSpeed; //Set to private after Debug
    [SerializeField] private float maxAttackSpeed;
    [SerializeField] private float additiveAttackSpeed;//Set to private after Debug
    #region Attack Speed Public Variables
    public float AdditiveAttackSpeed { get => additiveAttackSpeed; set => additiveAttackSpeed = value; }
    public float CurrentAttackSpeed
    {
        get => currentAttackSpeed;
        set
        {
            if (currentAttackSpeed >= maxAttackSpeed)
            {
                currentAttackSpeed = maxAttackSpeed;
            }
            else
            {
                currentAttackSpeed = value * (1 + AdditiveAttackSpeed / 100);
            }
        }
    }
    #endregion

    [Header("CRITICAL STRIKES CHANCE")]
    [SerializeField] private float baseCriticalStrikeChance;
    [SerializeField] private float currentCriticalStrikeChance;//Set to private after Debug
    #region Critical Strikes Public Variables
    public float CurrentCriticalStrikeChance
    {
        get => currentCriticalStrikeChance;
        set
        {
            if (currentCriticalStrikeChance > 100)
            {
                currentCriticalStrikeChance = 100;
            }
            else
            {
                currentCriticalStrikeChance = value;
            }
        }
    }
    public float CurrentCriticalStrikeMultiplier
    {
        get
        {
            return currentCriticalStrikeDamageMultiplier;
        }
        set
        {
            if (currentCriticalStrikeDamageMultiplier < baseCriticalStrikeDamageMultiplier)
            {
                currentCriticalStrikeDamageMultiplier = baseCriticalStrikeDamageMultiplier;
            }
            else if (currentCriticalStrikeDamageMultiplier > maxCriticalStrikeDamageMultiplier)
            {
                currentCriticalStrikeDamageMultiplier = maxCriticalStrikeDamageMultiplier;
            }
            else
            {
                currentCriticalStrikeDamageMultiplier = Mathf.Clamp(value, 175, maxCriticalStrikeDamageMultiplier);
            }
        }
    }
    #endregion

    [Header("CRITICAL STRIKES DAMAGE MULTIPLIER")]
    [SerializeField] private float baseCriticalStrikeDamageMultiplier = 175f;
    [SerializeField] private float maxCriticalStrikeDamageMultiplier = 300f;
    [SerializeField] private float currentCriticalStrikeDamageMultiplier; //Set to private after Debug

    [Header("DEFENSES PENETRATION")]
    [SerializeField] private float currentArmorPenetration;
    [SerializeField] private float currentMagicResistancePenetration;
    #region Defenses Penetration Public Variables
    public float CurrentArmorPenetration { get => currentArmorPenetration; set => currentArmorPenetration = value; }
    public float CurrentMagicResistancePenetration { get => currentMagicResistancePenetration; set => currentMagicResistancePenetration = value; }
    #endregion
    #endregion

    [Header("DEATH PARAMETERS")]
    [SerializeField] private float timeToRespawn;
    private CombatBehaviour CombatBehaviour => GetComponent<CombatBehaviour>();

    [Header("UI PARAMETERS")]
    [SerializeField] private GameObject damagePopUp;
    [SerializeField] private GameObject deathHUD;

    private Vector3 InFrontOfCharacter => transform.position + new Vector3(0, 0, -0.25f);

    //Death
    public bool IsDead => CurrentHealth <= 0;
    public float TimeToRespawn { get => timeToRespawn; set => timeToRespawn = value; }
    
    //Abilities
    public List<Ability> CharacterAbilities { get => characterAbilities; }
    

    protected virtual void Awake()
    {
        GetAllCharacterAbilities();

        #region Set Base Character Stats at Start of the game
        //CARACTERISTICS
        //- Health
        SetHealthAtStart(MaxHealth);

        //- Defense
        SetArmorAtStart(baseArmor);
        SetMagicResistanceAtStart(baseMagicResistance);

        //- Cooldown
        SetCooldownReductionAtStart(baseCooldownReduction);

        //- Damages
        SetAttackDamageAtStart(baseAttackDamage);
        SetMagicDamageAtStart(baseMagicDamage);

        //- Attack Speed
        SetAttackSpeedAtStart(baseAttackSpeed);

        //- Critical Strike Chance
        SetCriticalStrikeChanceAtStart(baseCriticalStrikeChance);
        SetCriticalStrikeMultiplierAtStart(baseCriticalStrikeDamageMultiplier);

        //- Defenses Penetration
        SetArmorPenetrationAtStart(0);
        SetMagicResistancePenetrationAtStart(0);
        #endregion
    }

    protected virtual void Start()
    {
        if (deathHUD != null)
            deathHUD.SetActive(false);
    }

    protected virtual void Update()
    {
        if (IsDead)
            OnDeath();
    }

    #region Settings at start of the game
    private void SetHealthAtStart(float characterMaxHealth)
    {
        CurrentHealth = characterMaxHealth;
    }

    private void SetArmorAtStart(float armorValueAtStart)
    {
        CurrentArmor = armorValueAtStart;
    }

    private void SetMagicResistanceAtStart(float magicResistanceValueAtStart)
    {
        CurrentMagicResistance = magicResistanceValueAtStart;
    }

    private void SetCooldownReductionAtStart(float cooldownValueAtStart)
    {
        CurrentCooldownReduction = cooldownValueAtStart;
    }
    private void SetAttackDamageAtStart(float attackDamageAtStart)
    {
        CurrentAttackDamage = attackDamageAtStart;
    }

    private void SetMagicDamageAtStart(float magicDamageAtStart)
    {
        CurrentMagicDamage = magicDamageAtStart;
    }

    private void SetCriticalStrikeChanceAtStart(float criticalStrikeChanceValueAtStart)
    {
        CurrentCriticalStrikeChance = criticalStrikeChanceValueAtStart;
    }

    private void SetCriticalStrikeMultiplierAtStart(float criticalStrikeMultiplierValueAtStart)
    {
        CurrentCriticalStrikeMultiplier = criticalStrikeMultiplierValueAtStart;
    }

    private void SetAttackSpeedAtStart(float attackSpeedValueAtStart)
    {
        CurrentAttackSpeed = attackSpeedValueAtStart;
    }

    private void SetArmorPenetrationAtStart(float armorPenetrationValueAtStart)
    {
        CurrentArmorPenetration = armorPenetrationValueAtStart;
    }

    private void SetMagicResistancePenetrationAtStart(float MagicResistancePenetrationValueAtStart)
    {
        CurrentMagicResistancePenetration = MagicResistancePenetrationValueAtStart;
    }

    private void GetAllCharacterAbilities()
    {
        foreach (Ability abilityFound in GetComponents<Ability>())
        {
            CharacterAbilities.Add(abilityFound);
        }
    }
    #endregion

    #region Take Damage Section
    public virtual void TakeDamage(float attackDamageTaken, float magicDamageTaken, float criticalStrikeChance, float criticalStrikeMultiplier, float armorPenetration, float magicResistancePenetration)
    {
        if (attackDamageTaken > 0)
        {
            float randomValue = Random.Range(0, 100);

            if (randomValue <= criticalStrikeChance)
            {
                attackDamageTaken *= criticalStrikeMultiplier / 100;
            }

            if (CurrentArmor > 0)
            {
                if (armorPenetration > 0)
                    attackDamageTaken *= 100 / (100 + (CurrentArmor * (armorPenetration / 100)));
                else
                    attackDamageTaken *= 100 / (100 + CurrentArmor);
            }
            else if (CurrentArmor <= 0)
            {
                attackDamageTaken *= 2 - 100 / (100 - CurrentArmor);
            }
            
            //attackDamageTaken *= 100 / (100 + (/*( */CurrentArmor /* - armorFlatReduction )*/ * (armorPenetration / 100)));

            DamagePopUp.Create(InFrontOfCharacter, damagePopUp, attackDamageTaken, DamageType.Physical);
        }

        if (magicDamageTaken > 0)
        {
            if (magicResistancePenetration > 0)
                magicDamageTaken *= 100 / (100 + (CurrentMagicResistance * (magicResistancePenetration / 100)));
            else
                magicDamageTaken *= 100 / (100 + CurrentMagicResistance);

            Debug.Log("Magic Resistance is over 0 / " + " Magic Damage " + (int)magicDamageTaken);

            if (attackDamageTaken > 0) 
                StartCoroutine(CreateDamagePopUpWithDelay(0.15f, magicDamageTaken, DamageType.Magic));
            else
                DamagePopUp.Create(InFrontOfCharacter, damagePopUp, magicDamageTaken, DamageType.Magic);
        }
        else if (magicDamageTaken <= 0)
        {
            magicDamageTaken *= 2 - 100 / (100 - CurrentMagicResistance);

            if (attackDamageTaken > 0)
                StartCoroutine(CreateDamagePopUpWithDelay(0.15f, magicDamageTaken, DamageType.Magic));
            else
                DamagePopUp.Create(InFrontOfCharacter, damagePopUp, magicDamageTaken, DamageType.Magic);
        }

        CurrentHealth -= (int)attackDamageTaken + (int)magicDamageTaken;

        Debug.Log("Health = " + CurrentHealth + " physical damage = " + (int)attackDamageTaken + " magic damage = " + (int)magicDamageTaken);
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float damageTaken, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);

        DamagePopUp.Create(InFrontOfCharacter, damagePopUp, damageTaken, damageType);
        Debug.Log(gameObject.name + " Life is : " + CurrentHealth);
    }
    #endregion

    #region Death and respawn
    public virtual void OnDeath()
    {
        StartCoroutine(Respawn(TimeToRespawn));
    }

    protected IEnumerator Respawn(float timeBeforeRespawn)
    {
        //Afficher le HUD de mort pendant le temps de la mort
        if (deathHUD != null)
            deathHUD.SetActive(true);

        if (CombatBehaviour != null) CombatBehaviour.CanPerformAttack = false;
        
        yield return new WaitForSeconds(timeBeforeRespawn);

        //Set Position At Spawn Location

        CurrentHealth = MaxHealth;

        //Désafficher le HUD de mort pendant le temps de la mort
        if (deathHUD != null)
            deathHUD.SetActive(false);

        if (CombatBehaviour != null) CombatBehaviour.CanPerformAttack = true;

        Debug.Log("is Dead " + IsDead);
    }
    #endregion
}
