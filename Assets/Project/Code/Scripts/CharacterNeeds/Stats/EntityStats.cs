using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityTeam
{
    DALVA,
    HULRYCK,
    NEUTRAL
}

public class EntityStats : MonoBehaviour, IDamageable, IKillable, ICurable, IRegenerable, IShieldable
{
    public delegate void StatValueChangedHandler(float newValue, float maxValue);
    public event StatValueChangedHandler OnHealthValueChanged;

    public delegate void DamageTakenHandler();
    public event DamageTakenHandler OnDamageTaken;

    public delegate void LifeStateHandler();
    public event LifeStateHandler OnEntityDeath;
    public event LifeStateHandler OnEntityRespawn;

    public delegate void ShieldValueHandler(float shieldValue, float maxHealth);
    public event ShieldValueHandler OnShieldValueChanged;

    #region Refs
    private CharacterController Controller => GetComponent<CharacterController>();
    private InteractionSystem Interactions => GetComponent<InteractionSystem>();
    //private VisibilityState VisibilityState => GetComponent<VisibilityState>();
    private EntityDetection EntityDetection => GetComponent<EntityDetection>();
    private Collider MyCollider => GetComponent<Collider>();
    #endregion

    [Header("CHARACTER INFORMATIONS")]
    [SerializeField] private EntityTeam entityTeam;
    [SerializeField] private BaseEntity usedEntity;
    [SerializeField] private List<AbilityLogic> entityAbilities;
    [SerializeField] private float entityLevel = 1f;
    public EntityTeam EntityTeam { get => entityTeam; set => entityTeam = value; }
    public BaseEntity UsedEntity { get => usedEntity; private set => usedEntity = value; }
    public List<AbilityLogic> EntityAbilities { get => entityAbilities; }
    public float EntityLevel { get => entityLevel; set => entityLevel = value; }

    [Header("STATS")]
    private float healthPercentage;
    public List<Stat> entityStats;
    public float HealthPercentage { get => healthPercentage; set => healthPercentage = value; }

    [Header("LIFE PARAMETERS")]
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private float timeToRespawn;
    public float TimeToRespawn { get => timeToRespawn; private set => timeToRespawn = value; }
    public Transform SourceOfDamage { get; set; }

    [Header("MARK")]
    [SerializeField] private GameObject allyMarkObject;
    [SerializeField] private GameObject enemyMarkObject;
    public float ExtentedMarkTime = 0f /* { get; set; }*/;
    public bool EntityIsMarked = false/* { get; set; }*/;

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

    public Vector3 CharacterHalfSize => transform.position + new Vector3(0, Controller.Agent.height / 2, 0);

    private void OnEnable()
    {
        if(EventSounds != null)
            EventSounds.RegisterReceiver();
    }

    private void Awake()
    {
        GetAllCharacterAbilities();
        InitStats();
    }

    private void Start()
    {
        if (deathHUD != null)
            deathHUD.SetActive(false);
    }

    private void Update() { 
        OnDeath(); 
        if (Input.GetKeyDown(KeyCode.L)) TakeDamage(transform, 0, 0, 50, 50, 0, 175, 0, 0, 0); 
        if (Input.GetKeyDown(KeyCode.M)) Heal(transform, 50f, GetStat(StatType.HealAndShieldEffectiveness).Value);
        if (Input.GetKeyDown(KeyCode.K)) ApplyShieldOnTarget(transform, 50f, GetStat(StatType.HealAndShieldEffectiveness).Value);
        if (Input.GetKeyDown(KeyCode.J)) Controller.StunTarget();
        if (Input.GetKeyDown(KeyCode.N)) Controller.RootTarget();
    }

    private void LateUpdate() => RegenerateHealth(transform, GetStat(StatType.HealthRegeneration).Value);

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
        float characterMagicalPenetration, 
        float characterIncomingDamageReduction)
    {
        if (CanTakeDamage)
        {
            bool isAttackCritical = false;

            #region Physical Damage
            if (characterPhysicalPower > 0)
            {                
                float randomValue = Random.Range(0, 100);

                if (characterCriticalStrikeChance > 0 
                    && randomValue <= characterCriticalStrikeChance)
                {
                    isAttackCritical = true;
                    characterPhysicalPower *= characterCriticalStrikeMultiplier / 100;
                }

                //Calculate target resistances / penetration
                float currentTargetPhysicalResistances = targetPhysicalResistances - (targetPhysicalResistances * (characterPhysicalPenetration / 100));
                Debug.Log("Target physical resistances " + currentTargetPhysicalResistances);

                if (currentTargetPhysicalResistances > 0)
                {
                   characterPhysicalPower *= 100 / (100 + currentTargetPhysicalResistances);
                }
                else if (currentTargetPhysicalResistances <= 0)
                {
                    characterPhysicalPower *= 2 - 100 / (100 - currentTargetPhysicalResistances);
                }

                if (GetStat(StatType.IncreasedDamageTaken).Value > 0) characterPhysicalPower += characterPhysicalPower * (GetStat(StatType.IncreasedDamageTaken).Value / 100);
                if (characterIncomingDamageReduction > 0) characterPhysicalPower -= characterPhysicalPower * (characterIncomingDamageReduction / 100);
            }
            #endregion

            #region Magical Damage
            if (characterMagicalPower > 0)
            {
                //Calculate target resistances / penetration
                float currentTargetMagicalResistance = targetMagicalResistances - (targetMagicalResistances * (characterMagicalPenetration / 100));
                Debug.Log("Target magical resistances : " + currentTargetMagicalResistance);

                if (currentTargetMagicalResistance > 0)
                {
                    characterMagicalPower *= 100 / (100 + currentTargetMagicalResistance);
                }
                else if (currentTargetMagicalResistance <= 0)
                {
                    characterMagicalPower *= 2 - 100 / (100 - currentTargetMagicalResistance);
                }

                if (GetStat(StatType.IncreasedDamageTaken).Value > 0) characterMagicalPower += characterMagicalPower * (GetStat(StatType.IncreasedDamageTaken).Value / 100);
                if (characterIncomingDamageReduction > 0) characterMagicalPower -= characterMagicalPower * (characterIncomingDamageReduction / 100);
            }
            #endregion

            if (isAttackCritical)
                global::Popup.Create(CharacterHalfSize, Popup, characterPhysicalPower, StatType.PhysicalPower, Popup.GetComponent<Popup>().PhysicalDamageIcon, true);

            if (characterPhysicalPower > 0)
                global::Popup.Create(CharacterHalfSize, Popup, characterPhysicalPower, StatType.PhysicalPower, Popup.GetComponent<Popup>().PhysicalDamageIcon);
            if (characterMagicalPower > 0)
                global::Popup.Create(new Vector3(CharacterHalfSize.x, CharacterHalfSize.y - 0.5f, CharacterHalfSize.z), Popup, characterMagicalPower, StatType.MagicalPower, Popup.GetComponent<Popup>().MagicalDamageIcon);

            this.SourceOfDamage = sourceOfDamage;

            GetStat(StatType.Health).Value -= ((int)characterPhysicalPower + (int)characterMagicalPower);

            #region LifeSteal
            EntityStats sourceOfDamageStats = sourceOfDamage.GetComponent<EntityStats>();
            float physicalLifeSteal = 0f;
            float magicalLifeSteal = 0f;

            if (sourceOfDamageStats.GetStat(StatType.PhysicalLifesteal) != null && sourceOfDamageStats.GetStat(StatType.PhysicalLifesteal).Value > 0)
                physicalLifeSteal = characterPhysicalPower * sourceOfDamageStats.GetStat(StatType.PhysicalLifesteal).Value / 100;

            if (sourceOfDamageStats.GetStat(StatType.MagicalLifesteal) != null && sourceOfDamageStats.GetStat(StatType.MagicalLifesteal).Value > 0)
                magicalLifeSteal = characterMagicalPower * sourceOfDamageStats.GetStat(StatType.MagicalLifesteal).Value / 100;

            if (physicalLifeSteal > 0f || magicalLifeSteal > 0f)
                sourceOfDamageStats.Heal(sourceOfDamage, physicalLifeSteal + magicalLifeSteal, sourceOfDamageStats.GetStat(StatType.HealAndShieldEffectiveness).Value);
            #endregion

            #region Events

            OnDamageTaken?.Invoke();

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);

            HealthPercentage = CalculateLifePercentage();
            #endregion

            Debug.Log("Health = " + GetStat(StatType.Health).Value + " physical damage = " + (int)characterPhysicalPower + " magic damage = " + (int)characterMagicalPower);
        }
        else if (!CanTakeDamage)
        {
            StartCoroutine(CreateDamagePopUpWithDelay(0.25f, 0, 0, null, true));
        }
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float value, StatType statType, Sprite icon, bool targetIsInvulnerable = false)
    {
        yield return new WaitForSeconds(delay);

        global::Popup.Create(CharacterHalfSize, Popup, value, statType, icon, targetIsInvulnerable);
    }
    #endregion

    #region Heal / Regeneration Section
    public void RegenerateHealth(Transform target, float regenerationThreshold)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats != null && targetStats.GetStat(StatType.Health).Value < targetStats.GetStat(StatType.Health).MaxValue)
        {
            if (targetStats.GetStat(StatType.Health).Value == targetStats.GetStat(StatType.Health).MaxValue) return;

            if (targetStats.GetStat(StatType.HealAndShieldEffectiveness) != null && targetStats.GetStat(StatType.HealAndShieldEffectiveness).Value > 0)
                regenerationThreshold += regenerationThreshold * targetStats.GetStat(StatType.HealAndShieldEffectiveness).Value / 100;

            targetStats.GetStat(StatType.Health).Value += regenerationThreshold;

            if (targetStats.GetStat(StatType.Health).Value + regenerationThreshold >= targetStats.GetStat(StatType.Health).MaxValue)
            {
                regenerationThreshold = targetStats.GetStat(StatType.Health).MaxValue - targetStats.GetStat(StatType.Health).Value;
                targetStats.GetStat(StatType.Health).Value += regenerationThreshold;
            }

            HealthPercentage = CalculateLifePercentage();

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);
        }
    }

    public void Heal(Transform target, float healAmount, float healEffectiveness)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats != null)
        {
            if (targetStats.GetStat(StatType.Health).Value == targetStats.GetStat(StatType.Health).MaxValue) return;

            if (healEffectiveness > 0)
                healAmount += healAmount * healEffectiveness / 100;

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

            HealthPercentage = CalculateLifePercentage();

            global::Popup.Create(new Vector3(CharacterHalfSize.x, CharacterHalfSize.y - 1f, CharacterHalfSize.z), Popup, healAmount, 0, null, false, true);

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
        }
    }
    #endregion

    #region Shield Section
    public void ApplyShieldOnTarget(Transform target, float shieldValue, float shieldEffectiveness)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats.GetStat(StatType.Shield) != null && targetStats.GetStat(StatType.HealAndShieldEffectiveness) != null)
        {
            if (shieldEffectiveness > 0)
                shieldValue += shieldValue * shieldEffectiveness / 100;
            else targetStats.GetStat(StatType.Shield).Value += shieldValue;

            OnShieldValueChanged?.Invoke(targetStats.GetStat(StatType.Shield).Value, targetStats.GetStat(StatType.Health).MaxValue);
        }
    }

    public void RemoveShieldOnTarget(Transform target, float shieldValue)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats.GetStat(StatType.Shield) != null)
        {
            if (targetStats.GetStat(StatType.Shield).Value - shieldValue <= 0) targetStats.GetStat(StatType.Shield).Value = 0;
            else targetStats.GetStat(StatType.Shield).Value -= shieldValue;

            OnShieldValueChanged?.Invoke(targetStats.GetStat(StatType.Shield).Value, targetStats.GetStat(StatType.Health).MaxValue);
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

            EntityDetection.enabled = false;

            if (EntityDetection.Outline.enabled)
                EntityDetection.Outline.enabled = false;

            if (GetComponent<CursorLogic>() != null) GetComponent<CursorLogic>().SetCursorToNormalAppearance();

            Controller.Agent.ResetPath();
            GetStat(StatType.Health).Value = 0f;

            StartCoroutine(ProcessDeathTimer(TimeToRespawn));

            MasterAudio.FireCustomEvent(deathCustomEvent, transform);

            OnEntityDeath?.Invoke();
        }
    }

    void GiveRessourcesToAPlayerOnDeath(float valueToGive)
    {
        if (SourceOfDamage != null
            && SourceOfDamage.GetComponent<CharacterRessources>() != null)
        {
            StartCoroutine(CreateDamagePopUpWithDelay(0.5f, valueToGive, StatType.RessourcesGiven, GetStat(StatType.RessourcesGiven).Icon));
            SourceOfDamage.GetComponent<CharacterRessources>().AddRessources((int)valueToGive);

            if (SourceOfDamage.GetComponent<EntityStats>().RessourcesGainedVFX != null)
                SourceOfDamage.GetComponent<EntityStats>().RessourcesGainedVFX.SetActive(true);

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

        MyCollider.enabled = false;

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

        yield return new WaitForSeconds(1.3f);

        Respawn();

        yield return new WaitForSeconds(0.25f);

        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);
    }

    private void Respawn()
    {
        Debug.Log("Respawn");

        GetStat(StatType.Health).Value = GetStat(StatType.Health).CalculateValue();
        HealthPercentage = CalculateLifePercentage();

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
        EntityDetection.enabled = true;
        MyCollider.enabled = true;

        //if (GetComponent<EntityDetection>().TypeOfEntity != TypeOfEntity.AllyPlayer || GetComponent<EntityDetection>().TypeOfEntity != TypeOfEntity.EnemyPlayer)
        //{
        //    VisibilityState.SetToVisible();
        //}

        //Set Position At Spawn Location
        if (spawnLocation != null)
            transform.position = spawnLocation.position;

        SourceOfDamage = null;

        Debug.Log("is Dead " + IsDead);

        isDeathEventHandled = false;

        OnEntityRespawn?.Invoke();
    }
    #endregion

    #region Handle entity mark
    public IEnumerator MarkEntity(float markDuration, EntityTeam sourceTeam)
    {
        //Debug
        if (allyMarkObject == null || enemyMarkObject == null) Debug.LogError("No mark object detected, need to assign them");

        if (EntityIsMarked) ExtentedMarkTime += markDuration;
        else if (!EntityIsMarked) { ExtentedMarkTime = markDuration; EntityIsMarked = true; }

        ActiveMarkFeedback(sourceTeam);

        do
        {
            ExtentedMarkTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (ExtentedMarkTime > 0);

        ExtentedMarkTime = 0f;

        DeactivateMarkFeedback();

        EntityIsMarked = false;
    }

    public void ActiveMarkFeedback(EntityTeam sourceTeam)
    {
        if (sourceTeam == EntityTeam) allyMarkObject.SetActive(true);
        else if (sourceTeam != EntityTeam) enemyMarkObject.SetActive(true);
    }

    public void DeactivateMarkFeedback()
    {
        if (allyMarkObject.activeInHierarchy) allyMarkObject.SetActive(false);
        else if (enemyMarkObject.activeInHierarchy) enemyMarkObject.SetActive(false);
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

        HealthPercentage = CalculateLifePercentage();

        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);
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

    private float CalculateLifePercentage()
    {
        float currentLifePercentage = 0f;

        currentLifePercentage = GetStat(StatType.Health).Value / GetStat(StatType.Health).MaxValue/* * 100f*/;

        return currentLifePercentage;
    }

    public void AscendEntity(EntityType newBaseEntity)
    {
        UsedEntity.EntityType = newBaseEntity;

        switch (newBaseEntity)
        {
            //Prowler
            case EntityType.Archer:
                //Augmenter la portée
                break;
            case EntityType.DaggerMaster:
                //Les attaques de base(2) et les compétences(5) appliquent l'effet "
                //"Faiblesse"". (3 secondes) Faiblesse: réduit de 1 % la RP.Max 15 % (15 marques)"
                break;

            //Warrior
            case EntityType.Berzerk:
                break;
            case EntityType.Coloss:
                break;
            
            //Mage
            case EntityType.Priest:
                break;
            case EntityType.Sorcerer:
                break;
        }
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