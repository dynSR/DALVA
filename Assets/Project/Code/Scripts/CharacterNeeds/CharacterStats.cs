using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CharacterStats : MonoBehaviour, IDamageable, IKillable
{
    [SerializeField] private Character usedCharacter;
    public Character UsedCharacter { get => usedCharacter; }

    [Header("CHARACTER ABILITIES AND DEFAULT ATTACK TYPE")]
    [SerializeField] private List<Ability> characterAbilities;

    #region Refs
    private CharacterController Controller => GetComponent<CharacterController>();
    private InteractionSystem Interactions => GetComponent<InteractionSystem>();
    private VisibilityState VisibilityState => GetComponent<VisibilityState>();
    #endregion

    #region Current Stats

    #region Health
    private float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Clamp(value, 0, UsedCharacter.BaseMaxHealth + AdditionalMaxHealth); }
    public float AdditionalMaxHealth { get; set; }
    public float CurrentHealthRegeneration { get; set; }
    #endregion

    #region Movement Speed
    public float CurrentMovementSpeed { get => Controller.Agent.speed; set => Controller.Agent.speed = value; }
    public float AdditiveMovementSpeed { get; set; }
    #endregion

    #region Cooldown
    private float currentCooldownReduction;
    public float CurrentCooldownReduction
    {
        get => currentCooldownReduction;
        set { currentCooldownReduction = Mathf.Clamp(value, 0, UsedCharacter.MaxCooldownReduction); }
    }
    public bool IsCooldownReductionCapped => CurrentCooldownReduction == UsedCharacter.MaxCooldownReduction;
    #endregion

    #region Defenses
    public float CurrentArmor { get; set; }
    public float CurrentMagicResistance { get; set; }
    #endregion

    #region Attack
    public float CurrentAttackRange { get; set; }
    public float CurrentAttackDamage { get; set; }
    public float CurrentMagicDamage { get; set; }
    #endregion

    #region AttackSpeed
    private float currentAttackSpeed;
    public float AdditiveAttackSpeed { get; set; }
    public float CurrentAttackSpeed
    {
        get => currentAttackSpeed;
        set
        {
            currentAttackSpeed = Mathf.Clamp(
                    value * (1 + AdditiveAttackSpeed / 100),
                    UsedCharacter.BaseAttackSpeed,
                    UsedCharacter.MaxAttackSpeed);
        }
    }
    #endregion

    #region Critics
    private float currentCriticalStrikeChance;
    public float CurrentCriticalStrikeChance
    {
        get => currentCriticalStrikeChance;
        set { currentCriticalStrikeChance = Mathf.Clamp(value, 0, 100); }
    }

    private float currentCriticalStrikeDamageMultiplier;
    public float CurrentCriticalStrikeMultiplier
    {
        get { return currentCriticalStrikeDamageMultiplier; }
        set { currentCriticalStrikeDamageMultiplier = Mathf.Clamp(value, UsedCharacter.BaseCriticalStrikeMultiplier, UsedCharacter.MaxCriticalStrikeMultiplier); }
    }
    #endregion

    #region Penetration
    public float CurrentArmorPenetration { get; set; }
    public float CurrentMagicResistancePenetration { get; set; }
    #endregion

    #endregion

    public float CurrentRessourcesGiven { get; set; }

    [Header("DEATH PARAMETERS")]
    public Transform sourceOfDamage;
    [SerializeField] private float timeToRespawn;
    public float TimeToRespawn { get => timeToRespawn; private set => timeToRespawn = value; }
    
    public bool IsDead => CurrentHealth <= 0f;
    private bool CanTakeDamage => !IsDead;
    private bool isDeathEventHandled = false;
    
    [Header("UI PARAMETERS")]
    [SerializeField] private GameObject damagePopUp;
    [SerializeField] private GameObject deathHUD;

    public List<Ability> CharacterAbilities { get => characterAbilities; }
    private Vector3 InFrontOfCharacter => transform.position + new Vector3(0, 0, -0.25f);

    protected virtual void Awake()
    {
        GetAllCharacterAbilities();

        if(UsedCharacter != null)
            InitStats();
    }

    protected virtual void Start()
    {
        if (deathHUD != null)
            deathHUD.SetActive(false);
    }

    protected virtual void Update()
    {
        if (IsDead && !isDeathEventHandled)
            OnDeath();

        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(transform, 50, 50, 25, 175, 0, 0);
        }
    }

    #region Settings at start of the game
    private void InitStats()
    {
        CurrentHealth = UsedCharacter.BaseMaxHealth;
        CurrentHealthRegeneration = UsedCharacter.BaseHealthRegeneration;

        CurrentMovementSpeed = UsedCharacter.BaseMovementSpeed;

        CurrentArmor = UsedCharacter.BaseArmor;
        CurrentMagicResistance = UsedCharacter.BaseMagicResistance;

        CurrentCooldownReduction = UsedCharacter.BaseCooldownReduction;

        CurrentAttackRange = UsedCharacter.BaseAttackRange;
        CurrentAttackDamage = UsedCharacter.BaseAttackDamage;
        CurrentMagicDamage = UsedCharacter.BaseMagicDamage;

        CurrentCriticalStrikeChance = UsedCharacter.BaseCriticalStrikeChance;
        CurrentCriticalStrikeMultiplier = UsedCharacter.BaseCriticalStrikeMultiplier;

        CurrentAttackSpeed = UsedCharacter.BaseAttackSpeed;
        AdditiveAttackSpeed = 0f;

        CurrentArmorPenetration = 0f;
        CurrentMagicResistancePenetration = 0f;

        CurrentRessourcesGiven = UsedCharacter.BaseRessourcesGiven;
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
    public virtual void TakeDamage(Transform sourceOfDamage, float attackDamageTaken, float magicDamageTaken, float criticalStrikeChance, float criticalStrikeMultiplier, float armorPenetration, float magicResistancePenetration)
    {
        if (CanTakeDamage)
        {
            if (attackDamageTaken > 0)
            {
                bool isAttackCritical = false;
                float randomValue = Random.Range(0, 100);

                if (randomValue <= criticalStrikeChance)
                {
                    isAttackCritical = true;
                    attackDamageTaken *= criticalStrikeMultiplier / 100;
                }

                if (CurrentArmor > 0)
                {
                    if (armorPenetration > 0)
                        attackDamageTaken *= 100 / (100 + (CurrentArmor - (CurrentArmor * (armorPenetration / 100))));
                    else
                        attackDamageTaken *= 100 / (100 + CurrentArmor);
                }
                else if (CurrentArmor <= 0)
                {
                    attackDamageTaken *= 2 - 100 / (100 - CurrentArmor);
                }

                //attackDamageTaken *= 100 / (100 + (/*( */CurrentArmor /* - armorFlatReduction )*/ * (armorPenetration / 100)));
                if (isAttackCritical)
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, attackDamageTaken, DamageType.Critical);
                else
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, attackDamageTaken, DamageType.Physical);
            }

            if (magicDamageTaken > 0)
            {
                if (magicResistancePenetration > 0)
                    magicDamageTaken *= 100 / (100 + (CurrentMagicResistance - (CurrentMagicResistance * (magicResistancePenetration / 100))));
                else
                    magicDamageTaken *= 100 / (100 + CurrentMagicResistance);

                Debug.Log("Magic Resistance is over 0 / " + " Magic Damage " + (int)magicDamageTaken);

                if (attackDamageTaken > 0)
                    StartCoroutine(CreateDamagePopUpWithDelay(0.25f, magicDamageTaken, DamageType.Magic));
                else
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, magicDamageTaken, DamageType.Magic);
            }
            else if (magicDamageTaken <= 0)
            {
                magicDamageTaken *= 2 - 100 / (100 - CurrentMagicResistance);

                if (attackDamageTaken > 0)
                    StartCoroutine(CreateDamagePopUpWithDelay(0.25f, magicDamageTaken, DamageType.Magic));
                else
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, magicDamageTaken, DamageType.Magic);
            }

            this.sourceOfDamage = sourceOfDamage;
            CurrentHealth -= (int)attackDamageTaken + (int)magicDamageTaken;

            Debug.Log("Health = " + CurrentHealth + " physical damage = " + (int)attackDamageTaken + " magic damage = " + (int)magicDamageTaken);
        }
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float damageTaken, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);

        DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, damageTaken, damageType);
        Debug.Log(gameObject.name + " Life is : " + CurrentHealth);
    }
    #endregion

    #region Death and respawn
    public virtual void OnDeath()
    {
        isDeathEventHandled = true;
        StartCoroutine(ProcessDeathTimer(TimeToRespawn));
    }

    void GiveRessourcesToAPlayerOnDeath()
    {
        if (sourceOfDamage != null 
            && sourceOfDamage.CompareTag("Player"))
        {
            sourceOfDamage.GetComponent<CharacterRessources>().AddRessources((int)CurrentRessourcesGiven);
            Debug.Log("Ressources have been given to a player, the last stored source of damage");
        }
    }

    private void Die()
    {
        //Afficher le HUD de mort pendant le temps de la mort
        if (deathHUD != null)
            deathHUD.SetActive(true);

        if (Interactions != null) 
            Interactions.CanPerformAttack = false;

        GiveRessourcesToAPlayerOnDeath();

        VisibilityState.SetToInvisible();

        //Désafficher le HUD de mort pendant le temps de la mort
        if (deathHUD != null)
            deathHUD.SetActive(false);
    }

    private IEnumerator ProcessDeathTimer(float delay)
    {
        Die();

        yield return new WaitForSeconds(delay);

        Respawn();
    }

    private void Respawn()
    {
        Debug.Log("Respawn");

        CurrentHealth = UsedCharacter.BaseMaxHealth + AdditionalMaxHealth;

        if (Interactions != null) 
            Interactions.CanPerformAttack = true;

        //Set Position At Spawn Location
        //transform.position = spawnLocation;

        VisibilityState.SetToVisible();

        Debug.Log("is Dead " + IsDead);
        isDeathEventHandled = false;
    }
    #endregion
}