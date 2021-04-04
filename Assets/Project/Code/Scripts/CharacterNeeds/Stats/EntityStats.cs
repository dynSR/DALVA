using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityTeam
{
    DALVA,
    HULRYCK
}

public class EntityStats : MonoBehaviour, IDamageable, IKillable, ICurable, IRegenerable
{
    public delegate void StatValueChangedHandler(float newValue, float maxValue);
    public event StatValueChangedHandler OnHealthValueChanged;

    public delegate void DamageTakenHandler();
    public event DamageTakenHandler OnDamageTaken;

    #region Refs
    private CharacterController Controller => GetComponent<CharacterController>();
    private InteractionSystem Interactions => GetComponent<InteractionSystem>();
    private VisibilityState VisibilityState => GetComponent<VisibilityState>();
    #endregion

    [Header("CHARACTER INFORMATIONS")]
    [SerializeField] private BaseEntity usedEntity;
    [SerializeField] private List<AbilityLogic> entityAbilities;
    public BaseEntity UsedEntity { get => usedEntity; }
    public List<AbilityLogic> EntityAbilities { get => entityAbilities; }

    [Header("STATS")]
    public List<Stat> entityStats;

    [Header("LIFE PARAMETERS")]
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private float timeToRespawn;
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

    [Header("VFX")]
    [SerializeField] private GameObject healVFX;
    [SerializeField] private GameObject respawnVFX;
    [SerializeField] private GameObject ressourcesGainedVFX;
    public GameObject RessourcesGainedVFX { get => ressourcesGainedVFX; }

    [Header("SOUNDS")]
    [MasterCustomEvent] public string spawnCustomEvent;
    [MasterCustomEvent] public string deathCustomEvent;
    EventSounds EventSounds => GetComponent<EventSounds>();

    [Header("PLAYER OPTIONS")]
    [SerializeField] private bool centerCameraOnRespawn = false;

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
        foreach (AbilityLogic abilityFound in GetComponents<AbilityLogic>())
        {
            EntityAbilities.Add(abilityFound);
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

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float value, StatType statType, Sprite icon, bool isASpecialPopup = false)
    {
        yield return new WaitForSeconds(delay);

        global::Popup.Create(InFrontOfCharacter, Popup, value, statType, icon);

        if (isASpecialPopup)
        {
            if (SourceOfDamage.GetComponent<EntityStats>().RessourcesGainedVFX != null)
                SourceOfDamage.GetComponent<EntityStats>().RessourcesGainedVFX.SetActive(true);
        }

        //Debug.Log(gameObject.name + " Life is : " + CurrentHealth);
    }
    #endregion

    #region Heal / Regeneration Section
    public void RegenerateHealth(Transform target, float regenerationThreshold)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

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
        EntityStats targetStats = target.GetComponent<EntityStats>();

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
            StartCoroutine(CreateDamagePopUpWithDelay(0.5f, valueToGive, StatType.RessourcesGiven, GetStat(StatType.RessourcesGiven).Icon, true));

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

        if (transform.GetComponent<PlayerController>() != null)
            transform.GetComponent<PlayerController>().IsPlayerInHisBase = true;
    }

    private IEnumerator ProcessDeathTimer(float delay)
    {
        Die();

        //yield return new WaitForSeconds(0.5f);
        //VisibilityState.SetToInvisible();

        yield return new WaitForSeconds(delay - 0.5f);

        MasterAudio.FireCustomEvent(spawnCustomEvent, spawnLocation);

        if (transform.GetComponent<PlayerController>() != null)
        {
            if (centerCameraOnRespawn)
            {
                transform.GetComponent<PlayerController>().CharacterCamera.GetComponent<CameraController>().MoveCameraToASpecificMiniMapPosition(transform.position);
            }
        }

        yield return new WaitForSeconds(1.3f);

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

        //Désafficher le HUD de mort après la mort
        if (deathHUD != null)
            deathHUD.SetActive(false);

        //Spawn Respawn VFX
        if (respawnVFX != null)
            respawnVFX.SetActive(true);

        //Visibility State
        if (GetComponent<EntityDetection>() != null)
        {
            GetComponent<EntityDetection>().enabled = true;

            if (GetComponent<EntityDetection>().TypeOfEntity != TypeOfEntity.AllyPlayer || GetComponent<EntityDetection>().TypeOfEntity != TypeOfEntity.EnemyPlayer)
            {
                VisibilityState.SetToVisible();
            }
        }
            

        //Set Position At Spawn Location
        if(spawnLocation != null)
            transform.position = spawnLocation.position;

        SourceOfDamage = null;

        Debug.Log("is Dead " + IsDead);

        isDeathEventHandled = false;
    }
    #endregion

    #region Handle Stats
    void InitStats()
    {
        CanTakeDamage = true;

        for (int i = 0; i < entityStats.Count; i++)
        {
            entityStats[i].InitValue();
        }

        if(GetStat(StatType.MovementSpeed) != null)
            Controller.SetNavMeshAgentSpeed(Controller.Agent, GetStat(StatType.MovementSpeed).Value);

        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
    }

    public Stat GetStat(StatType statType)
    {
        Stat stat = null;

        for (int i = entityStats.Count - 1; i >= 0; i--)
        {
            if (entityStats[i].StatType == statType)
            {
                stat = entityStats[i];
            }
        }

        return stat;
    }
    #endregion

    #region Editor Purpose
    public void RefreshCharacterStats()
    {
        if (usedEntity != null)
        {
            entityStats.Clear();

            for (int i = 0; i < usedEntity.EntityStats.Count; i++)
            {
                Stat stat = new Stat();

                entityStats.Add(stat);

                entityStats[i].Name = usedEntity.EntityStats[i].Name;
                entityStats[i].StatType = usedEntity.EntityStats[i].StatType;

                if(usedEntity.EntityStats[i].Icon != null)
                    entityStats[i].Icon = usedEntity.EntityStats[i].Icon;

                if (usedEntity.EntityStats[i].BaseValue > 0)
                {
                    entityStats[i].BaseValue = usedEntity.EntityStats[i].BaseValue;
                }
                else
                {
                    entityStats[i].BaseValue = 0;
                }
            }
        }
    }
    #endregion
}