using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteleState
{
    Inactive, 
    Active,
    StandBy,
}

public enum SteleLevel
{
    Default,
    EvolutionI,
    EvolutionII,
    FinalEvolution,
    OnlySell
}

public enum SteleEffect
{
    Guardian,
    Frost,
    Weakness,
    Inferno,
    Sentinel,
    Ramapart,
}

public class SteleLogic : InteractiveBuilding/*, IKillable, IDamageable*/
{
    public delegate void SteleInteractionHandler();
    public event SteleInteractionHandler OnInteraction;
    public event SteleInteractionHandler OnEndOFInteraction;

    //public delegate void SteleHealthHandler(float value, float max);
    //public event SteleHealthHandler OnHealthValueChanged;

    //public delegate void SteleLifeStatusHandler();
    //public event SteleLifeStatusHandler OnActivation;
    //public event SteleLifeStatusHandler OnSteleDeath;

    //[Header("HEALTH PARAMTERS")]
    //[SerializeField] private int healthPoints = 0; //debug
    //[SerializeField] private int maxHealthPoints = 0; //debug

    [Header("CURRENT STATE")]
    [SerializeField] private SteleState steleState;
    [SerializeField] private SteleLevel steleLevel;

    [Header("OTHER ATTRIBUTES")]
    [SerializeField] private Transform effectEntitySpawnLocation;
    public GameObject spawnedEffectObject;
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private List<GameObject> runes;
    private bool interactionIsHandled = false;
    //private bool isDead = false;
    //public int HealthPoints { get => healthPoints; set => healthPoints = value; }
    public SteleState SteleState { get => steleState; private set => steleState = value; }
    public SteleLevel SteleLevel { get => steleLevel; private set => steleLevel = value; }

    #region Ref
    private Outline Outline => GetComponent<Outline>();
    #endregion

    void Start()
    {
        SetSteleToInactiveMode();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (InteractingPlayer != null && !interactionIsHandled)
        {
            OnInteraction?.Invoke();
            interactionIsHandled = true;
        }
        else if (InteractingPlayer == null && interactionIsHandled)
        {
            OnEndOFInteraction?.Invoke();
            interactionIsHandled = false;
        }
    }

    #region Button Methods
    public void SetSteleToActiveMode(int steleHealthPointsRelativeToEffect)
    {
        SteleState = SteleState.Active;

        SetSteleTeam();

        activationVFX.SetActive(true);

        InteractingPlayer.Target = null;
        InteractingPlayer = null;

        //OnActivation?.Invoke();

        //maxHealthPoints = steleHealthPointsRelativeToEffect;
        //HealthPoints = maxHealthPoints;
    }

    public void SetSteleLevel(int steleLevel)
    {
        SteleLevel = (SteleLevel)steleLevel;
    }

    public void ActiveRuneEffect(GameObject rune)
    {
        rune.SetActive(true);
    }

    public void SpawnEntityEffect(GameObject entityToSpawn)
    {
        Instantiate(entityToSpawn, effectEntitySpawnLocation.position, Quaternion.identity);
        spawnedEffectObject = entityToSpawn;
    }

    public void SellEffect()
    {
        Destroy(spawnedEffectObject);
    }
    #endregion

    private void SetSteleToInactiveMode()
    {
        IsInteractable = true;

        //isDead = false;
        SteleState = SteleState.Inactive;
    }

    IEnumerator SetSteleToStandByMode()
    {
        yield return new WaitForSeconds(ReinitializationDelay);
        yield return new WaitForEndOfFrame();

        SetSteleToInactiveMode();
    }

    private void SetSteleTeam()
    {
        if (InteractingPlayer.GetComponent<EntityStats>().EntityTeam == EntityTeam.DALVA)
        {
            EntityTeam = EntityTeam.DALVA;
            //EntityDetection.TypeOfEntity = TypeOfEntity.AllyStele;
            Outline.OutlineColor = Color.blue;
        }
        else if (InteractingPlayer.GetComponent<EntityStats>().EntityTeam == EntityTeam.HULRYCK
            /*InteractingPlayer.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyPlayer*/)
        {
            EntityTeam = EntityTeam.HULRYCK;
            //EntityDetection.TypeOfEntity = TypeOfEntity.EnemyStele;
            Outline.OutlineColor = Color.red;
        }
    }

    //public void OnDeath()
    //{
    //    OnSteleDeath?.Invoke();
    //    EntityTeam = EntityTeam.NEUTRAL;
    //    IsInteractable = false;
    //    StartCoroutine(SetSteleToStandByMode());

    //    for (int i = 0; i < runes.Count; i++)
    //    {
    //        if (runes[i].activeInHierarchy)
    //            runes[i].SetActive(false);
    //    }
    //}

    //public void TakeDamage(
    //    Transform character, 
    //    float targetPhysicalResistances, 
    //    float targetMagicalResistances, 
    //    float characterPhysicalPower, 
    //    float characterMagicalPower, 
    //    float characterCriticalStrikeChance, 
    //    float characterCriticalStrikeMultiplier, 
    //    float characterPhysicalPenetration, 
    //    float characterMagicalPenetration, 
    //    float damageReduction)
    //{
    //    HealthPoints -= (int)characterPhysicalPower;
    //    OnHealthValueChanged?.Invoke(HealthPoints, maxHealthPoints);
    //    Debug.Log("STELE TOOK DAMAGE");

    //    if (HealthPoints == 0)
    //    {
    //        isDead = true;
    //        OnDeath();
    //    }
    //}
}