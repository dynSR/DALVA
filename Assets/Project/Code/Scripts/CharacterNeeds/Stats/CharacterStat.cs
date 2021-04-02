using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : MonoBehaviour, IDamageable, IKillable, ICurable, IRegenerable
{
    public delegate void StatValueChangedHandler(float newValue, float maxValue);
    public event StatValueChangedHandler OnHealthValueChanged;

    public delegate void DamageTakenHandler();
    public event DamageTakenHandler OnDamageTaken;

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
    public List<Stat> CharacterStats;

    [Header("LIFE PARAMETERS")]
    [SerializeField] private float timeToRespawn;
    [SerializeField] private GameObject healVFX;
    float regenerationAddedValue;
    public float TimeToRespawn { get => timeToRespawn; private set => timeToRespawn = value; }
    public Transform SourceOfDamage { get; set; }

    public bool IsDead => GetStat(StatType.Health).Value <= 0f;
    public bool CanTakeDamage { get; set; }
    private bool isDeathEventHandled = false;

    [Header("UI PARAMETERS")]
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject deathHUD;
    public GameObject Popup { get => popup; }

    [Header("SOUNDS")]
    [MasterCustomEvent] public string deathCustomEvent;
    EventSounds EventSounds => GetComponent<EventSounds>();

    public Vector3 InFrontOfCharacter => transform.position + new Vector3(0, Controller.Agent.height / 2, 0);

    private void OnEnable()
    {
        if(EventSounds != null)
            EventSounds.RegisterReceiver();
    }

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

    protected virtual void Update() { OnDeath(); if (Input.GetKeyDown(KeyCode.T)) TakeDamage(transform, 0, 0, 50, 0, 0, 175, 0, 0); if (Input.GetKeyDown(KeyCode.H)) Heal(transform, 50f); }

    #region Settings at start of the game
    private void GetAllCharacterAbilities()
    {
        CanTakeDamage = true;

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

                //characterPhysicalPower *= 100 / (100 + (/*( */targetPhysicalResistances /* - armorFlatReduction )*/ * (characterPhysicalPenetration / 100)));

                if (isAttackCritical)
                    global::Popup.Create(InFrontOfCharacter, Popup, characterPhysicalPower, StatType.PhysicalPower, Popup.GetComponent<Popup>().PhysicalDamageIcon, true);
                else if (characterPhysicalPower > 0)
                    global::Popup.Create(InFrontOfCharacter, Popup, characterPhysicalPower, StatType.PhysicalPower, Popup.GetComponent<Popup>().PhysicalDamageIcon);
            }

            if (characterMagicalPower > 0)
            {
                if (targetMagicalResistances > 0)
                {
                    if (characterMagicalPenetration > 0)
                        characterMagicalPower *= 100 / (100 + (targetMagicalResistances - (targetMagicalResistances * (characterMagicalPenetration / 100))));
                    else
                        characterMagicalPower *= 100 / (100 + targetMagicalResistances);
                }
                else if (targetMagicalResistances <= 0)
                {
                    characterMagicalPower *= 2 - 100 / (100 - targetMagicalResistances);
                }

                if (characterPhysicalPower > 0)
                    StartCoroutine(CreateDamagePopUpWithDelay(0.25f, characterMagicalPower, StatType.MagicalPower, Popup.GetComponent<Popup>().MagicalDamageIcon));
                else if (characterMagicalPower > 0 && characterMagicalPower > 0)
                    global::Popup.Create(InFrontOfCharacter, Popup, characterMagicalPower, StatType.MagicalPower, Popup.GetComponent<Popup>().MagicalDamageIcon);
            }
            
            this.SourceOfDamage = sourceOfDamage;
            GetStat(StatType.Health).Value -= ((int)characterPhysicalPower + (int)characterMagicalPower);

            OnDamageTaken?.Invoke();

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());

            //Debug.Log("Health = " + GetStat(StatType.Health).Value + " physical damage = " + (int)characterPhysicalPower + " magic damage = " + (int)characterMagicalPower);
        }
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float value, StatType statType, Sprite icon)
    {
        yield return new WaitForSeconds(delay);

        global::Popup.Create(InFrontOfCharacter, Popup, value, statType, icon);
        //Debug.Log(gameObject.name + " Life is : " + CurrentHealth);
    }
    #endregion

    #region Heal / Regeneration Section
    public void RegenerateHealth(Transform target, float regenerationThreshold)
    {
        CharacterStat targetStats = target.GetComponent<CharacterStat>();

        if (targetStats != null && targetStats.GetStat(StatType.Health).Value < targetStats.GetStat(StatType.Health).MaxValue)
        {
            if (targetStats.GetStat(StatType.Health).Value == targetStats.GetStat(StatType.Health).MaxValue) return;

            regenerationAddedValue += regenerationThreshold;

            targetStats.GetStat(StatType.Health).Value += regenerationThreshold;

            if (targetStats.GetStat(StatType.Health).Value + regenerationThreshold >= targetStats.GetStat(StatType.Health).MaxValue)
            {
                regenerationThreshold = targetStats.GetStat(StatType.Health).MaxValue - targetStats.GetStat(StatType.Health).Value;
                targetStats.GetStat(StatType.Health).Value += regenerationThreshold;
            }

            if (regenerationAddedValue > (regenerationThreshold * 2))
            {
                OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
                regenerationAddedValue = 0;
            }
        }
    }

    public void Heal(Transform target, float healAmount)
    {
        CharacterStat targetStats = target.GetComponent<CharacterStat>();

        if (targetStats != null)
        {
            if (targetStats.GetStat(StatType.Health).Value == targetStats.GetStat(StatType.Health).MaxValue) return;

            if (targetStats.GetStat(StatType.Health).Value + healAmount >= targetStats.GetStat(StatType.Health).MaxValue)
            {
                healAmount = targetStats.GetStat(StatType.Health).MaxValue - targetStats.GetStat(StatType.Health).Value;
                targetStats.GetStat(StatType.Health).Value += healAmount;
            }
            else
            {
                targetStats.GetStat(StatType.Health).Value += healAmount;
            }

            if(healVFX != null)
                healVFX.SetActive(true);

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
        }
    }
    #endregion

    #region Death and respawn
    public virtual void OnDeath()
    {
        if (IsDead && !isDeathEventHandled)
        {
            isDeathEventHandled = true;

            CanTakeDamage = false;

            Controller.CharacterAnimator.SetBool("Attack", false);
            Controller.CharacterAnimator.ResetTrigger("NoTarget");

            if (GetComponent<EntityDetection>() != null) 
            {
                GetComponent<EntityDetection>().enabled = false;

                if (GetComponent<EntityDetection>().Outline.enabled)
                    GetComponent<EntityDetection>().Outline.enabled = false;
            }

            if (GetComponent<CursorLogic>() != null) GetComponent<CursorLogic>().SetCursorToNormalAppearance();

            Controller.Agent.ResetPath();
            GetStat(StatType.Health).Value = 0f;

            StartCoroutine(ProcessDeathTimer(TimeToRespawn));

            MasterAudio.FireCustomEvent(deathCustomEvent, transform);
        }
    }

    void GiveRessourcesToAPlayerOnDeath(float valueToGive)
    {
        if (SourceOfDamage != null
            && SourceOfDamage.GetComponent<CharacterRessources>() != null)
        {
            SourceOfDamage.GetComponent<CharacterRessources>().AddRessources((int)valueToGive);
            StartCoroutine(CreateDamagePopUpWithDelay(1.15f, valueToGive, StatType.RessourcesGiven, GetStat(StatType.RessourcesGiven).Icon));
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

        Controller.CharacterAnimator.SetBool("IsDead", true);

        GiveRessourcesToAPlayerOnDeath(GetStat(StatType.RessourcesGiven).Value);
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

        CanTakeDamage = true;

        Controller.CharacterAnimator.SetBool("IsDead", false);

        if (Interactions != null)
            Interactions.CanPerformAttack = true;

        SourceOfDamage = null;

        //Désafficher le HUD de mort après la mort
        if (deathHUD != null)
            deathHUD.SetActive(false);

        if (GetComponent<EntityDetection>() != null)
            GetComponent<EntityDetection>().enabled = true;

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

        Controller.SetNavMeshAgentSpeed(Controller.Agent, GetStat(StatType.MovementSpeed).Value);

        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
    }

    public Stat GetStat(StatType statType)
    {
        Stat stat = null;

        for (int i = CharacterStats.Count - 1; i >= 0; i--)
        {
            if (CharacterStats[i].StatType == statType)
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
                CharacterStats[i].StatType = usedCharacter.CharacterStats[i].StatType;

                if(usedCharacter.CharacterStats[i].Icon != null)
                    CharacterStats[i].Icon = usedCharacter.CharacterStats[i].Icon;

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