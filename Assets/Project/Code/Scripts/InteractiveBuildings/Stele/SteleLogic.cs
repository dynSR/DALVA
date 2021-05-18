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
    NoEffectSetYet,
    Guardian,
    Frost,
    Weakness,
    Sentinel,
    Rampart,
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
    [SerializeField] private SteleEffect steleEffect;

    [Header("OTHER ATTRIBUTES")]
    [SerializeField] private Transform effectEntitySpawnLocation;
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private GameObject activatedVFX;
    [SerializeField] private List<GameObject> runes;
    private bool interactionIsHandled = false;
    //private bool isDead = false;
    //public int HealthPoints { get => healthPoints; set => healthPoints = value; }
    public SteleState SteleState { get => steleState; private set => steleState = value; }
    public SteleLevel SteleLevel { get => steleLevel; private set => steleLevel = value; }
    public SteleEffect SteleEffect { get => steleEffect; private set => steleEffect = value; }
    public GameObject SpawnedEffectObject { get; set; }
    public List<SpawnedEffectTransformData> spawnedEffectTransformDatas;

    [System.Serializable]
    public class EffectDescription
    {
        public string effectName;
        public SteleEffect steleEffect;
        [Multiline]
        public string description;
        public int effectCost;
        public Sprite effectIcon;
    }

    [System.Serializable]
    public class SpawnedEffectTransformData
    {
        public string effectName;
        public Vector3 position;
        public float yAxisValue;
    }

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
    public void SetSteleToActiveMode(int cost)
    {
        if (InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources < cost) return;

        SteleState = SteleState.Active;

        SetSteleTeam();

        PurchaseSteleEffect(cost);

        activationVFX.SetActive(true);
        //Throw build/actiavtion sound event here -!-

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

    public void SetSteleEffect(int steleEffect)
    {
        SteleEffect = (SteleEffect)steleEffect;
    }

    public void ActiveRuneEffect(GameObject rune)
    {
        rune.SetActive(true);
        activatedVFX.SetActive(true);
    }

    public void DeactivateRuneEffect()
    {
        for (int i = 0; i < runes.Count; i++)
        {
            if (runes[i].activeInHierarchy)
            {
                runes[i].SetActive(false);
                activatedVFX.SetActive(false);
            }
        }
    }

    public void SpawnEntityEffect(GameObject entityToSpawn)
    {
        SpawnedEffectObject = Instantiate(
            entityToSpawn, 
            new Vector3(effectEntitySpawnLocation.position.x, 
            effectEntitySpawnLocation.position.y + entityToSpawn.transform.position.y,
            effectEntitySpawnLocation.position.z), 
            entityToSpawn.transform.rotation);

        SteleAmelioration steleAmelioration = SpawnedEffectObject.GetComponent<SteleAmelioration>();

        steleAmelioration.Stele = this;
    }

    public void PurchaseSteleEffect(int purchaseCost)
    {
        InteractingPlayer.GetComponent<CharacterRessources>().RemoveRessources(purchaseCost);
    }

    public void SellEffect(int amountToRefund)
    {
        Destroy(SpawnedEffectObject);
        SteleLevel = SteleLevel.Default;
        InteractingPlayer.GetComponent<CharacterRessources>().AddRessources(amountToRefund);
        SetSteleEffect(0);
        DeactivateRuneEffect();
    }

    public void UpgradeEffect()
    {
        SpawnedEffectObject.GetComponent<SteleAmelioration>().UpgradeEffect();

        if (activationVFX.activeInHierarchy) activationVFX.SetActive(false);
        activationVFX.SetActive(true);
        //Throw upgrade sound event here -!-
    }

    public void SetFinalEvolutionValueOfSteleEffect(int value)
    {
        SpawnedEffectObject.GetComponent<SteleAmelioration>().FinalEvolutionNumber = value;
    }
    #endregion

    private void SetSteleToInactiveMode()
    {
        IsInteractable = true;

        //isDead = false;
        SteleState = SteleState.Inactive;
    }

    public void SetSpawnedEffectTransformValues(int concernedIndex)
    {
        //index = valeurs pour le transform
        //effet position = listDatas[concernedIndex]
        //effet rotation.Y = listDatas[concernedIndex]

        //Spawned
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