using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Warrior, Marksman, Mage, Healer }
enum DefaultAttackType { Melee, Ranged }
public class Stats : MonoBehaviour, IDamageable, IKillable
{
    [Header("CHARACTER NAME & CLASSE")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;

    [Header("CHARACTER ABVILITIES AND DEFAULT ATTACK TYPE")]
    [SerializeField] private List<Ability> characterAbilities;
    [SerializeField] private DefaultAttackType defaultAttackType;

    [Header("HEALTH PARAMETERS")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegeneration;
    private float currentHealth;

    [Header("COOLDOWN PARAMETERS")]
    [SerializeField] private float baseCooldownReduction;
    [SerializeField] private float maxCooldownReduction;
    private float currentCooldownReduction;

    [Header("DEFENSE PARAMETERS")]
    [SerializeField] private float baseArmor;
    [SerializeField] private float baseMagicResistance;

    [Header("DAMAGE PARAMETERS")]
    [SerializeField] private float baseAttackDamage;
    private float currentAttackDamage;
    [SerializeField] private float baseMagicDamage;

    [Header("ATTACK RANGE PARAMETERS")]
    [SerializeField] private float attackRange;

    [Header("ATTACK SPEED PARAMETERS")]
    [SerializeField] private float baseAttackSpeed;
    private float currentAttackSpeed;
    private float additiveAttackSpeed;

    [Header("CRITICAL STRIKES PARAMETERS")]
    [SerializeField] private float baseCriticalStrikeChance;
    private float currentCriticalStrikeChance;
    [SerializeField] private float baseCriticalStrikeDamageMultiplier;

    [Header("DEATH PARAMETERS")]
    [SerializeField] private float timeToRespawn;
    private CombatBehaviour CombatBehaviour => GetComponent<CombatBehaviour>();

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

    //Damages
    public float CurrentAttackDamage { get; set; }
    public float CurrentMagicDamage { get; set; }

    //AttackRange
    public float AttackRange { get => attackRange; set => attackRange = value; }

    //Attack Speed
    public float AdditiveAttackSpeed { get => additiveAttackSpeed; set => additiveAttackSpeed = value / 100; }
    public float CurrentAttackSpeed { get => currentAttackSpeed; set => currentAttackSpeed = baseAttackSpeed * (1 + AdditiveAttackSpeed); }

    //Critical Strikes
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
    public float CurrentCriticalStrikeMultiplier { get; set; }

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

        //- Damages
        SetAttackDamageAtStartOfTheGameIsEqualTo(baseAttackDamage);
        SetMagicDamageAtStartOfTheGameIsEqualTo(baseMagicDamage);

        //- Attack Speed
        SetAttackSpeedAtStartOfTheGameIsEqualTo(baseAttackSpeed);

        //- Critical Strike Chance
        SetCriticalStrikeChanceAtStartOfTheGameIsEqualTo(baseCriticalStrikeChance);
        SetCriticalStrikeMultiplierAtStartOfTheGameIsEqualTo(baseCriticalStrikeDamageMultiplier);
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
        CurrentAttackDamage = attackDamageAtStart;
    }

    private void SetMagicDamageAtStartOfTheGameIsEqualTo(float magicDamageAtStart)
    {
        CurrentMagicDamage = magicDamageAtStart;
    }

    private void SetCriticalStrikeChanceAtStartOfTheGameIsEqualTo(float criticalStrikeChanceValueAtStart)
    {
        CurrentCriticalStrikeChance = criticalStrikeChanceValueAtStart;
    }

    private void SetCriticalStrikeMultiplierAtStartOfTheGameIsEqualTo(float criticalStrikeMultiplierValueAtStart)
    {
        CurrentCriticalStrikeMultiplier = criticalStrikeMultiplierValueAtStart;
    }

    private void SetAttackSpeedAtStartOfTheGameIsEqualTo(float attackSpeedValueAtStart)
    {
        CurrentAttackSpeed = attackSpeedValueAtStart;
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
    public virtual void TakeDamage(float attackDamageTaken, float magicDamageTaken, float characterCriticalStrikeChance, float characterCriticalStrikeMultiplier)
    {
        if (attackDamageTaken > 0)
        {
            float randomValue = Random.Range(0, 100);
            Debug.Log("Random Value to determined critical strike or not : " + randomValue);

            if (randomValue <= characterCriticalStrikeChance)
            {
                attackDamageTaken *= characterCriticalStrikeMultiplier / 100;
                Debug.Log("Critical Strike");
            }

            if (CurrentArmor <= 0)
            {
                attackDamageTaken *= 2 - 100 / (100 - CurrentArmor);
                Debug.Log("Armor is equal or inferior to 0 / " + " Physical Damage " + (int)attackDamageTaken);
            }
            else
            {
                attackDamageTaken *= 100 / (100 + CurrentArmor);
                Debug.Log("Armor is over 0 / " + " Physical Damage " + (int)attackDamageTaken);
            }

            DamagePopUp.Create(InFrontOfCharacter, damagePopUp, attackDamageTaken, DamageType.Physical);
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
        //deathHUD.SetActive(true);

        if (CombatBehaviour != null) CombatBehaviour.CanPerformAttack = false;
        
        yield return new WaitForSeconds(timeBeforeRespawn);

        //Set Position At Spawn Location

        CurrentHealth = MaxHealth;

        //Désafficher le HUD de mort pendant le temps de la mort
        //deathHUD.SetActive(false);

        if (CombatBehaviour != null) CombatBehaviour.CanPerformAttack = true;

        Debug.Log("is Dead " + IsDead);
    }
    #endregion
}
