using DarkTonic.MasterAudio;
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
    public event SteleInteractionHandler OnPurchase;

    [Header("CURRENT STATE")]
    [SerializeField] private SteleState steleState;
    [SerializeField] private SteleLevel steleLevel;
    [SerializeField] private SteleEffect steleEffect;

    [Header("OTHER ATTRIBUTES")]
    [SerializeField] private Transform effectEntitySpawnLocation;
    [SerializeField] private GameObject activationVFX;
    [SerializeField] private GameObject activatedVFX;

    [Header("SFX")]
    [SoundGroup] [SerializeField] private string activationSFX;
    [SoundGroup] [SerializeField] private string upgradeSFX;
    [SoundGroup] [SerializeField] private string sellingSFX;

    private bool interactionIsHandled = false;
    public SteleState SteleState { get => steleState; private set => steleState = value; }
    public SteleLevel SteleLevel { get => steleLevel; private set => steleLevel = value; }
    public SteleEffect SteleEffect { get => steleEffect; private set => steleEffect = value; }
    public GameObject SpawnedEffectObject { get; set; }
    public List<SpawnedEffectTransformData> spawnedEffectTransformDatas;
    private CharacterRessources interactingPlayerRessources;

    [System.Serializable]
    public class EffectDescription
    {
        public string effectName;
        public SteleEffect steleEffect;
        [Multiline]
        public string description;
        public Sprite effectIcon;
    }

    [System.Serializable]
    public class SpawnedEffectTransformData
    {
        public string effectName;
        public Transform position;
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
            interactingPlayerRessources = InteractingPlayer.GetComponent<CharacterRessources>();
        }
        else if (InteractingPlayer == null && interactionIsHandled)
        {
            OnEndOFInteraction?.Invoke();
            interactionIsHandled = false;
        }
    }

    #region Button Methods
    public void SetSteleToActiveMode()
    {
        if (interactingPlayerRessources.CurrentAmountOfPlayerRessources < CurrentPurchaseCost()) return;

        SteleState = SteleState.Active;

        SetSteleTeam();

        PurchaseSteleEffect();

        SetSteleLevel(1);

        activationVFX.SetActive(true);
        UtilityClass.PlaySoundGroupImmediatly(activationSFX, transform);

        activatedVFX.SetActive(true);

        //InteractingPlayer.Target = null;
        //InteractingPlayer = null;
    }

    public void SetSteleLevel(int steleLevel)
    {
        SteleLevel = (SteleLevel)steleLevel;
    }

    public void SetSteleEffect(int steleEffect)
    {
        SteleEffect = (SteleEffect)steleEffect;
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

    #region Ressources
    public int CurrentPurchaseCost()
    {
        int cost = 0;

        switch (SteleLevel)
        {
            case SteleLevel.Default:
                cost = 150;
                break;
            case SteleLevel.EvolutionI:
                cost = 300;
                break;
            case SteleLevel.EvolutionII:
                cost = 600;
                break;
            case SteleLevel.FinalEvolution:
                cost = 1050;
                break;
            case SteleLevel.OnlySell:
                cost = 1050;
                break;
        }

        //Debug.Log("!! CURRENT COST - STELE EFFECT !! " + cost);

        return cost;
    }

    public int CurrentSellCost()
    {
        int cost = 0;

        switch (SteleLevel)
        {
            case SteleLevel.EvolutionI:
                cost = 150 / 2;
                break;
            case SteleLevel.EvolutionII:
                cost = 300 / 2;
                break;
            case SteleLevel.FinalEvolution:
                cost = 600 / 2;
                break;
            case SteleLevel.OnlySell:
                cost = 1050 / 2;
                break;
        }

        //Debug.Log("!! CURRENT COST - STELE EFFECT !! " + cost);

        return cost;
    }

    private void PurchaseSteleEffect()
    {
        interactingPlayerRessources.RemoveRessources(CurrentPurchaseCost());
        interactingPlayerRessources.SetRessourcesFeedback(interactingPlayerRessources.CurrentAmountOfPlayerRessources - CurrentPurchaseCost());
        OnPurchase?.Invoke();
    }

    public void SellEffect()
    {
        Destroy(SpawnedEffectObject);

        UtilityClass.PlaySoundGroupImmediatly(sellingSFX, transform);

        interactingPlayerRessources.AddRessources(CurrentSellCost());
        interactingPlayerRessources.SetRessourcesFeedback(interactingPlayerRessources.CurrentAmountOfPlayerRessources + CurrentSellCost());

        SteleLevel = SteleLevel.Default;
        SetSteleEffect(0);

        if (InteractingPlayer != null)
            InteractingPlayer = null;

        activatedVFX.SetActive(false);
    }

    public void UpgradeEffect()
    {
        SteleAmelioration steleAmeliorationScript = SpawnedEffectObject.GetComponent<SteleAmelioration>();

        steleAmeliorationScript.UpgradeEffect();

        if (activationVFX.activeInHierarchy) activationVFX.SetActive(false);
        activationVFX.SetActive(true);
        UtilityClass.PlaySoundGroupImmediatly(upgradeSFX, transform);

        PurchaseSteleEffect();

        switch (SteleLevel)
        {
            case SteleLevel.EvolutionII:
                steleAmeliorationScript.renderers.transform.GetChild(0).gameObject.SetActive(false);
                steleAmeliorationScript.renderers.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case SteleLevel.FinalEvolution:
                steleAmeliorationScript.renderers.transform.GetChild(1).gameObject.SetActive(false);
                steleAmeliorationScript.renderers.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
    }
    #endregion
    #endregion

    private void SetSteleToInactiveMode()
    {
        IsInteractable = true;
        SteleState = SteleState.Inactive;
    }

    public void SetSpawnedEffectTransformValues(int concernedIndex)
    {
        //index = valeurs pour le transform
        //effet position = listDatas[concernedIndex]
        Debug.Log("SetSpawnedEffectTransformValues", transform);

        //Spawned
        SpawnedEffectObject.transform.position = spawnedEffectTransformDatas[concernedIndex].position.position;
        SpawnedEffectObject.transform.rotation = spawnedEffectTransformDatas[concernedIndex].position.rotation;
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