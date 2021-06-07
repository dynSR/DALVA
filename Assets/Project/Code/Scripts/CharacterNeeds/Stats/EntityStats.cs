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

    public delegate void StatsValueHandler(EntityStats stats);
    public event StatsValueHandler OnStatsValueChanged;

    #region Refs
    public CharacterController Controller => GetComponent<CharacterController>();
    private InteractionSystem Interactions => GetComponent<InteractionSystem>();
    //private VisibilityState VisibilityState => GetComponent<VisibilityState>();
    private EntityDetection EntityDetection => GetComponent<EntityDetection>();
    private Collider MyCollider => GetComponent<Collider>();
    #endregion

    [Header("CHARACTER INFORMATIONS")]
    [SerializeField] private EntityTeam entityTeam;
    [SerializeField] private BaseEntity baseUsedEntity;
    [SerializeField] private List<AbilityLogic> entityAbilities;
    //[SerializeField] private float entityLevel = 1f;
    public EntityTeam EntityTeam { get => entityTeam; set => entityTeam = value; }
    public BaseEntity BaseUsedEntity { get => baseUsedEntity; private set => baseUsedEntity = value; }
    public List<AbilityLogic> EntityAbilities { get => entityAbilities; }
    //public float EntityLevel { get => entityLevel; set => entityLevel = value; }

    [Header("STATS")]
    private float healthPercentage;
    [SerializeField] private int damageAppliedToThePlaceToDefend = 0;
    public List<Stat> entityStats;
    //[SerializeField] private bool entityIsAscended = false;
    public float HealthPercentage { get => healthPercentage; set => healthPercentage = value; }
    //public bool EntityIsAscended { get => entityIsAscended; set => entityIsAscended = value; }

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
    public GameObject consumedMarkVFX;

    public bool IsDead => GetStat(StatType.Health).Value <= 0f;
    public bool CanTakeDamage { get; set; }
    private bool isDeathEventHandled = false;

    [Header("UI PARAMETERS")]
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject deathHUD;
    public GameObject Popup { get => popup; }

    [Header("VFX")]
    [SerializeField] private GameObject healVFX;
    [SerializeField] private GameObject shieldVFX;
    [SerializeField] private GameObject respawnVFX;
    [SerializeField] private GameObject ressourcesGainedVFX;
    public GameObject RessourcesGainedVFX { get => ressourcesGainedVFX; }
    public GameObject ShieldVFX { get => shieldVFX; }

    [Header("SOUNDS")]
    [SoundGroup] public string spawnSoundGroup;
    [SoundGroup] public string deathSoundGroup;

    public Vector3 CharacterHalfSize => transform.position + new Vector3(0, Controller.Agent.height / 2, 0);

    public int DamageAppliedToThePlaceToDefend { get => damageAppliedToThePlaceToDefend; set => damageAppliedToThePlaceToDefend = value; }

    private void Awake()
    {
        GetAllCharacterAbilities();
        InitStats();
    }

    private void Start()
    {
        if (deathHUD != null)
            deathHUD.SetActive(false);

        InvokeRepeating("RegenerateHealthOverTime", 1f, 1f);

        UpdateStats();
    }

    private void Update() 
    { 
        OnDeath();

        if (Input.GetKeyDown(KeyCode.L)) TakeDamage(transform, 0, 0, 50, 50, 0, 175, 0, 0); 
        if (Input.GetKeyDown(KeyCode.M)) Heal(transform, 50f, GetStat(StatType.HealAndShieldEffectiveness).Value);
        //if (Input.GetKeyDown(KeyCode.K)) ApplyShieldOnTarget(transform, 50f, GetStat(StatType.HealAndShieldEffectiveness).Value);
        //if (Input.GetKeyDown(KeyCode.J)) Controller.StunTarget();
        //if (Input.GetKeyDown(KeyCode.N)) Controller.RootTarget();
    }

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
            bool isAttackCritical = false;

            float combinedDamage;

            #region Physical Damage
            if (characterPhysicalPower > 0)
            {
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
            }
            #endregion

            if (characterCriticalStrikeChance > 0)
            {
                float randomValue = Random.Range(0, 100);

                if (randomValue <= characterCriticalStrikeChance)
                {
                    isAttackCritical = true;
                    combinedDamage = characterPhysicalPower + characterMagicalPower;
                    combinedDamage *= characterCriticalStrikeMultiplier / 100;
                }
                else combinedDamage = characterPhysicalPower + characterMagicalPower;
            }
            else combinedDamage = characterPhysicalPower + characterMagicalPower;

            #region Popup
            global::Popup.Create(CharacterHalfSize, Popup, combinedDamage, StatType.PhysicalPower, Popup.GetComponent<Popup>().PhysicalDamageIcon, isAttackCritical);
            #endregion

            if (sourceOfDamage != null)
                this.SourceOfDamage = sourceOfDamage;

            if (GetStat(StatType.Shield) == null || GetStat(StatType.Shield).Value <= 0)
                GetStat(StatType.Health).Value -= (int)combinedDamage;

            if (sourceOfDamage != null && GetStat(StatType.Shield) != null && GetStat(StatType.Shield).Value > 0)
            {
                if (combinedDamage <= GetStat(StatType.Shield).Value)
                {
                    RemoveShieldOnTarget(transform, combinedDamage);
                }
                else if (combinedDamage > GetStat(StatType.Shield).Value)
                {
                    float healthToRemoveMinusShield = combinedDamage - GetStat(StatType.Shield).Value;

                    RemoveShieldOnTarget(transform, GetStat(StatType.Shield).Value);
                    GetStat(StatType.Health).Value -= healthToRemoveMinusShield;
                }
            }

            #region LifeSteal
            EntityStats sourceOfDamageStats = null;
            float combinedLifeSteal = 0f;

            if (sourceOfDamage != null)
            {
                sourceOfDamageStats = sourceOfDamage.GetComponent<EntityStats>();

                if (sourceOfDamageStats.GetStat(StatType.PhysicalLifesteal) != null && sourceOfDamageStats.GetStat(StatType.MagicalLifesteal) != null)
                {
                    combinedLifeSteal = combinedDamage * ((sourceOfDamageStats.GetStat(StatType.PhysicalLifesteal).Value + sourceOfDamageStats.GetStat(StatType.MagicalLifesteal).Value) / 100);
                }
            }

            if (combinedLifeSteal > 0f)
                sourceOfDamageStats.Heal(sourceOfDamage, combinedLifeSteal, sourceOfDamageStats.GetStat(StatType.HealAndShieldEffectiveness).Value);
            #endregion

            #region Events

            OnDamageTaken?.Invoke();

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);

            HealthPercentage = CalculateLifePercentage();
            #endregion

            //Debug.Log("Health = " + GetStat(StatType.Health).Value + " physical damage = " + (int)characterPhysicalPower + " magic damage = " + (int)characterMagicalPower, transform);
        }
        else if (!CanTakeDamage)
        {
            StartCoroutine(CreateDamagePopUpWithDelay(0.25f, 0, 0, null, true));
        }
    }

    private IEnumerator CreateDamagePopUpWithDelay(float delay, float value, StatType statType, Sprite icon, bool targetIsInvulnerable = false)
    {
        yield return new WaitForSeconds(delay);

        global::Popup.Create(CharacterHalfSize, Popup, value, statType, icon, false, targetIsInvulnerable);
    }
    #endregion

    #region Heal / Regeneration Section
    public void RegenerateHealth(Transform target, float regenerationThreshold)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats != null && targetStats.GetStat(StatType.Health).Value < targetStats.GetStat(StatType.Health).MaxValue)
        {
            if (targetStats.GetStat(StatType.HealAndShieldEffectiveness) != null && targetStats.GetStat(StatType.HealAndShieldEffectiveness).Value > 0)
                regenerationThreshold += regenerationThreshold * targetStats.GetStat(StatType.HealAndShieldEffectiveness).Value / 100;

            if (targetStats.GetStat(StatType.Health).Value + regenerationThreshold >= targetStats.GetStat(StatType.Health).MaxValue)
            {
                regenerationThreshold = targetStats.GetStat(StatType.Health).MaxValue - targetStats.GetStat(StatType.Health).Value;
                targetStats.GetStat(StatType.Health).Value += regenerationThreshold;
            }
            else targetStats.GetStat(StatType.Health).Value += regenerationThreshold;

            HealthPercentage = CalculateLifePercentage();

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);
        }
    }

    void RegenerateHealthOverTime()
    {
        if (IsDead) return;

        RegenerateHealth(transform, GetStat(StatType.HealthRegeneration).Value);
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
                ActiveHealVFX();
            }
            else
            {
                targetStats.GetStat(StatType.Health).Value += healAmount;
                ActiveHealVFX();
            }

            HealthPercentage = CalculateLifePercentage();

            global::Popup.Create(new Vector3(CharacterHalfSize.x, CharacterHalfSize.y - 1f, CharacterHalfSize.z), Popup, healAmount, StatType.Health, null, false);

            Debug.Log("Health = " + GetStat(StatType.Health).Value + " Heal = " + (int)healAmount);

            OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).CalculateValue());
        }
    }

    private void ActiveHealVFX()
    {
        if(healVFX != null)
            healVFX.SetActive(true);
    }

    public void DeactiveHealVFX()
    {
        if (healVFX != null)
            healVFX.SetActive(false);
    }
    #endregion

    #region Shield Section
    public void ApplyShieldOnTarget(Transform target, float shieldValue, float shieldEffectiveness, bool comesFromAnEffect = false)
    {
        Debug.Log("APPLYING SHIELD ON TARGET");

        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats.GetStat(StatType.Shield) != null && targetStats.GetStat(StatType.HealAndShieldEffectiveness) != null)
        {
            float shieldValueBeforeCalcul = shieldValue;

            if (comesFromAnEffect) shieldValue = 0;
            else
            {
                if (shieldEffectiveness > 0)
                    shieldValue += shieldValue * shieldEffectiveness / 100;
                else targetStats.GetStat(StatType.Shield).Value += (int)shieldValue;

                shieldValueBeforeCalcul = shieldValue;
            }

            OnShieldValueChanged?.Invoke(targetStats.GetStat(StatType.Shield).Value, targetStats.GetStat(StatType.Health).MaxValue);

            global::Popup.Create(new Vector3(target.position.x, target.position.y + 1f, target.position.z), Popup, shieldValueBeforeCalcul, StatType.Shield, null);

            ActiveShieldVFX();
        }
    }

    public void RemoveShieldOnTarget(Transform target, float shieldValue)
    {
        EntityStats targetStats = target.GetComponent<EntityStats>();

        if (targetStats.GetStat(StatType.Shield) != null)
        {
            if (targetStats.GetStat(StatType.Shield).Value - shieldValue <= 0)
            {
                targetStats.GetStat(StatType.Shield).Value = 0;
                DeactiveShieldVFX();
            }
            else targetStats.GetStat(StatType.Shield).Value -= shieldValue;

            OnShieldValueChanged?.Invoke(targetStats.GetStat(StatType.Shield).Value, targetStats.GetStat(StatType.Health).MaxValue);
        }
    }

    private void ActiveShieldVFX()
    {
        if (shieldVFX != null)
            shieldVFX.SetActive(true);
    }

    public void DeactiveShieldVFX()
    {
        if (shieldVFX != null)
            shieldVFX.SetActive(false);
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

            if (!EntityDetection.Outline.enabled)
                EntityDetection.Outline.enabled = true;

            EntityDetection.Outline.OutlineColor = Color.black;

            if (GetComponent<CursorLogic>() != null) GetComponent<CursorLogic>().SetCursorToNormalAppearance();

            Controller.Agent.ResetPath();
            GetStat(StatType.Health).Value = 0f;

            //For NPCs
            NPCController npcController = GetComponent<NPCController>();

            if (npcController != null && !npcController.IsACampNPC && (npcController.IsABossWaveMember || npcController != null && GameManager.Instance.itsFinalWave))
            {
                GameManager.Instance.UpdateRemainingMonsterValue(-1);
            }

            StartCoroutine(ProcessDeathTimer(TimeToRespawn));

            UtilityClass.PlaySoundGroupImmediatly(deathSoundGroup, transform);
        }
    }

    void GiveRessourcesToAPlayerOnDeath(float valueToGive)
    {
        if (this != GameManager.Instance.Player.GetComponent<EntityStats>())
        {
            StartCoroutine(CreateDamagePopUpWithDelay(0.5f, valueToGive, StatType.RessourcesGiven, GetStat(StatType.RessourcesGiven).Icon));
            GameManager.Instance.Player.GetComponent<CharacterRessources>().AddRessources((int)valueToGive);

            //GameManager.Instance.Player.GetComponent<EntityStats>().RessourcesGainedVFX.SetActive(true);
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
        Controller.Agent.enabled = false;

        if (transform.GetComponent<PlayerController>() != null)
            transform.GetComponent<PlayerController>().IsPlayerInHisBase = true;

        if (Controller.StunVFX != null && Controller.StunVFX.activeInHierarchy) Controller.StunVFX.SetActive(false);
        if (Controller.RootedVFX != null && Controller.RootedVFX.activeInHierarchy) Controller.RootedVFX.SetActive(false);

        else if (Controller.StunVFX == null || Controller.RootedVFX == null) Debug.LogError("Need to fill the controller stun or rooted field -!-");

        DeactivateMarkFeedback();

        OnEntityDeath?.Invoke();
    }

    private IEnumerator ProcessDeathTimer(float delay)
    {
        Die();

        yield return new WaitForSeconds(delay - 0.5f);

        UtilityClass.PlaySoundGroupImmediatly(spawnSoundGroup, spawnLocation);

        yield return new WaitForSeconds(1.3f);

        //For NPCs
        NPCController npcController = GetComponent<NPCController>();

        if (!npcController || npcController && !npcController.IsACampNPC)
        {
            Respawn();
        }

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
        Controller.Agent.enabled = true;
        EntityDetection.enabled = true;
        MyCollider.enabled = true;

        if (Controller.IsStunned) Controller.UnStunTarget();
        if (Controller.IsRooted) Controller.UnRootTarget();

        //Set Position At Spawn Location
        if (spawnLocation != null)
            transform.localPosition = spawnLocation.position;

        SourceOfDamage = null;

        Debug.Log("is Dead " + IsDead);

        isDeathEventHandled = false;

        OnEntityRespawn?.Invoke();
    }
    #endregion

    #region Handle entity mark
    public IEnumerator MarkEntity(float markDuration)
    {
        //Debug
        if (allyMarkObject == null || enemyMarkObject == null) Debug.LogError("No mark object detected, need to assign them");

        if (EntityIsMarked) ExtentedMarkTime += markDuration;
        else if (!EntityIsMarked) 
        { 
            ExtentedMarkTime = markDuration; 
            ActivateMarkFeedback(); 
        }

        do
        {
            ExtentedMarkTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (ExtentedMarkTime > 0);

        ExtentedMarkTime = 0f;

        if (!IsDead)
            DeactivateMarkFeedback();
    }

    public void ActivateMarkFeedback()
    {
        Debug.Log("ActivateMarkFeedback");

        EntityIsMarked = true;

        if (EntityTeam == EntityTeam.DALVA) allyMarkObject.SetActive(true);
        else if (EntityTeam == EntityTeam.HULRYCK || EntityTeam == EntityTeam.NEUTRAL) enemyMarkObject.SetActive(true);
    }

    public void DeactivateMarkFeedback()
    {
        Debug.Log("DeactivateMarkFeedback");

        if (allyMarkObject.activeInHierarchy) allyMarkObject.SetActive(false);
        else if (enemyMarkObject.activeInHierarchy) enemyMarkObject.SetActive(false);

        EntityIsMarked = false;
    }

    public void ConsumeMark()
    {
        if (consumedMarkVFX != null && !consumedMarkVFX.activeInHierarchy) consumedMarkVFX.SetActive(true);
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

        if(GetStat(StatType.MovementSpeed) != null && Controller != null)
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

    public void UpdateStats()
    {
        OnStatsValueChanged?.Invoke(this);
        OnHealthValueChanged?.Invoke(GetStat(StatType.Health).Value, GetStat(StatType.Health).MaxValue);
    }

    public void UpdateNavMeshAgentSpeed(StatType statType)
    {
        if (statType == StatType.MovementSpeed)
        {
            Controller.SetNavMeshAgentSpeed(Controller.Agent, this.GetStat(StatType.MovementSpeed).Value);
        }
    }
    #endregion

    #region Editor Purpose
    public void RefreshCharacterStats()
    {
        if (baseUsedEntity != null)
        {
            entityStats.Clear();

            for (int i = 0; i < baseUsedEntity.EntityStats.Count; i++)
            {
                Stat stat = new Stat();

                entityStats.Add(stat);

                entityStats[i].Name = baseUsedEntity.EntityStats[i].Name;
                entityStats[i].StatType = baseUsedEntity.EntityStats[i].StatType;

                if(baseUsedEntity.EntityStats[i].Icon != null)
                    entityStats[i].Icon = baseUsedEntity.EntityStats[i].Icon;

                if (baseUsedEntity.EntityStats[i].BaseValue > 0)
                {
                    entityStats[i].BaseValue = baseUsedEntity.EntityStats[i].BaseValue;
                }
                else
                {
                    entityStats[i].BaseValue = 0;
                }

                if (baseUsedEntity.EntityStats[i].CapValue > 0)
                {
                    entityStats[i].CapValue = baseUsedEntity.EntityStats[i].CapValue;
                }
                else
                {
                    entityStats[i].CapValue = 0;
                }
            }
        }
    }
    #endregion
}