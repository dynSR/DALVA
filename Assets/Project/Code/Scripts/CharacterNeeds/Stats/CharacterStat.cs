using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour, IDamageable, IKillable
{
    public delegate void StatValueChangedHandler(float newValue, float maxValue);
    public static event StatValueChangedHandler OnHealthValueChanged;

    #region Refs
    private CharacterController Controller => GetComponent<CharacterController>();
    private InteractionSystem Interactions => GetComponent<InteractionSystem>();
    #endregion

    [Header("CHARACTER INFORMATIONS")]
    [SerializeField] private BaseCharacter usedCharacter;
    [SerializeField] private List<AbilityLogic> characterAbilities;
    public BaseCharacter UsedCharacter { get => usedCharacter; }
    public List<AbilityLogic> CharacterAbilities { get => characterAbilities; }

    [Header("STATS")]
    [SerializeField] private int amountOfRessourcesGiventOnDeath = 0;
    
    public List<Stat> CharacterStats;
    int CurrentRessourcesGivenOnDeath { get => amountOfRessourcesGiventOnDeath; /*set;*/ } // set commenté, à décommenter et à utiliser si besoin de modifier la valeur

    [Header("DEATH PARAMETERS")]
    public Transform sourceOfDamage;
    [SerializeField] private float timeToRespawn;
    public float TimeToRespawn { get => timeToRespawn; private set => timeToRespawn = value; }

    public bool IsDead => GetStat(StatType.Health).Value <= 0f;
    private bool CanTakeDamage => !IsDead;
    private bool isDeathEventHandled = false;

    [Header("UI PARAMETERS")]
    [SerializeField] private GameObject damagePopUp;
    [SerializeField] private GameObject deathHUD;

    private Vector3 InFrontOfCharacter => transform.position + new Vector3(0, 0, -0.25f);

    protected virtual void Awake()
    {
        GetAllCharacterAbilities();
        InitStats();
    }

    protected virtual void Start()
    {
        if (deathHUD != null)
            deathHUD.SetActive(false);
    }

    protected virtual void Update() { OnDeath(); if (Input.GetKeyDown(KeyCode.T)) TakeDamage(transform, 0, 0, 50, 0, 0, 175, 0, 0); }

    #region Settings at start of the game
    private void GetAllCharacterAbilities()
    {
        foreach (AbilityLogic abilityFound in GetComponents<AbilityLogic>())
        {
            CharacterAbilities.Add(abilityFound);
        }
    }
    #endregion

    #region Take Damage Section
    public virtual void TakeDamage(
        Transform sourceOfDamage,
        float targetPhysicalResistances,
        float targetMagicalResistances,
        float characterPhysicalPower,
        float characterMagicalPower,
        float characterCriticalStrikeChance,
        float characterCriticalStrikeMultiplier,
        float characterPhysicalPenetration,
        float characterMagicalPenetration)
    {
        if (CanTakeDamage)
        {
            if (characterPhysicalPower > 0)
            {
                bool isAttackCritical = false;
                float randomValue = Random.Range(0, 100);

                if (characterCriticalStrikeChance > 0 
                    && randomValue <= characterCriticalStrikeChance)
                {
                    isAttackCritical = true;
                    characterPhysicalPower *= characterCriticalStrikeMultiplier / 100;
                }

                if (targetPhysicalResistances > 0)
                {
                    if (characterPhysicalPenetration > 0)
                        characterPhysicalPower *= 100 / (100 + (targetPhysicalResistances - (targetPhysicalResistances * (characterPhysicalPenetration / 100))));
                    else
                        characterPhysicalPower *= 100 / (100 + targetPhysicalResistances);
                }
                else if (targetPhysicalResistances <= 0)
                {
                    characterPhysicalPower *= 2 - 100 / (100 - targetPhysicalResistances);
                }

                characterPhysicalPower *= 100 / (100 + (/*( */targetPhysicalResistances /* - armorFlatReduction )*/ * (characterPhysicalPenetration / 100)));

                if (isAttackCritical)
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, characterPhysicalPower, DamageType.Critical);
                else if (characterPhysicalPower > 0)
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, characterPhysicalPower, DamageType.Physical);
            }

            if (characterMagicalPower > 0)
            {
                if (characterMagicalPenetration > 0)
                    characterMagicalPower *= 100 / (100 + (targetMagicalResistances- (targetMagicalResistances * (characterMagicalPenetration / 100))));
                else
                    characterMagicalPower *= 100 / (100 + targetMagicalResistances);

                Debug.Log("Magic Resistance is over 0 / " + " Magic Damage " + (int)characterMagicalPower);

                if (characterPhysicalPower > 0)
                    StartCoroutine(CreateDamagePopUpWithDelay(0.25f, characterMagicalPower, DamageType.Magic));
                else if (characterMagicalPower > 0)
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, characterMagicalPower, DamageType.Magic);
            }
            else if (targetMagicalResistances <= 0)
            {
                characterMagicalPower *= 2 - 100 / (100 - targetMagicalResistances);

                if (characterPhysicalPower > 0)
                    StartCoroutine(CreateDamagePopUpWithDelay(0.25f, characterMagicalPower, DamageType.Magic));
                else if (characterMagicalPower > 0)
                    DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, characterMagicalPower, DamageType.Magic);
            }

            this.sourceOfDamage = sourceOfDamage;
            GetStat(StatType.Health).Value -= ((int)characterPhysicalPower + (int)characterMagicalPower);

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());

            Debug.Log("Health = " + GetStat(StatType.Health).Value + " physical damage = " + (int)characterPhysicalPower + " magic damage = " + (int)characterMagicalPower);
        }
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float damageTaken, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);

        DamagePopupLogic.Create(InFrontOfCharacter, damagePopUp, damageTaken, damageType);
        //Debug.Log(gameObject.name + " Life is : " + CurrentHealth);
    }
    #endregion

    #region Death and respawn
    public virtual void OnDeath()
    {
        if (IsDead && !isDeathEventHandled)
        {
            isDeathEventHandled = true;
            Controller.Agent.ResetPath();
            StartCoroutine(ProcessDeathTimer(TimeToRespawn));
        }
    }

    void GiveRessourcesToAPlayerOnDeath()
    {
        if (sourceOfDamage != null
            && sourceOfDamage.GetComponent<CharacterRessources>() != null)
        {
            sourceOfDamage.GetComponent<CharacterRessources>().AddRessources((int)CurrentRessourcesGivenOnDeath);
            //Debug.Log("Ressources have been given to a player, the last stored source of damage");
        }
    }

    private void Die()
    {
        //Afficher le HUD de mort pendant le temps de la mort
        if (deathHUD != null)
            deathHUD.SetActive(true);

        if (Interactions != null)
            Interactions.CanPerformAttack = false;

        //isoler pour un appel
        Controller.CharacterAnimator.SetBool("IsDead", true);

        Debug.Log("TOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM");

        GiveRessourcesToAPlayerOnDeath();
    }

    private IEnumerator ProcessDeathTimer(float delay)
    {
        Die();

        yield return new WaitForSeconds(delay);

        Respawn();

        yield return new WaitForSeconds(0.25f);

        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
    }

    private void Respawn()
    {
        Debug.Log("Respawn");
        GetStat(StatType.Health).Value = GetStat(StatType.Health).CalculateValue();

        Controller.CharacterAnimator.SetBool("IsDead", false);

        if (Interactions != null)
            Interactions.CanPerformAttack = true;

        sourceOfDamage = null;

        //Désafficher le HUD de mort après la mort
        if (deathHUD != null)
            deathHUD.SetActive(false);

        //Set Position At Spawn Location
        //transform.position = spawnLocation;
        //Spawn Respawn VFX

        Debug.Log("is Dead " + IsDead);

        isDeathEventHandled = false;

        
    }
    #endregion

    #region Handle Stats
    void InitStats()
    {
        for (int i = 0; i < CharacterStats.Count; i++)
        {
            CharacterStats[i].InitValue();
        }

        Controller.SetNavMeshAgentSpeed(Controller.Agent, GetStat(StatType.Movement_Speed).Value);

        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
    }

    public Stat GetStat(StatType statType)
    {
        Stat stat = null;

        for (int i = CharacterStats.Count - 1; i >= 0; i--)
        {
            if (CharacterStats[i]._StatType == statType)
            {
                stat = CharacterStats[i];
            }
        }

        return stat;
    }
    #endregion

    #region Editor Purpose
    public void RefreshCharacterStats()
    {
        if (usedCharacter != null)
        {
            CharacterStats.Clear();

            for (int i = 0; i < usedCharacter.CharacterStats.Count; i++)
            {
                Stat stat = new Stat();

                CharacterStats.Add(stat);
                CharacterStats[i].Name = usedCharacter.CharacterStats[i].Name;
                CharacterStats[i]._StatType = usedCharacter.CharacterStats[i]._StatType;

                if (usedCharacter.CharacterStats[i].BaseValue > 0)
                {
                    CharacterStats[i].BaseValue = usedCharacter.CharacterStats[i].BaseValue;
                }
                else
                {
                    CharacterStats[i].BaseValue = 0;
                }
            }
        }
    }
    #endregion
}