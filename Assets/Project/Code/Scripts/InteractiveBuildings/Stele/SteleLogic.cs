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
        UtilityClass.PlaySoundGroupImmediatly(activationSFX, transform);

        InteractingPlayer.Target = null;
        InteractingPlayer = null;
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
        UtilityClass.PlaySoundGroupImmediatly(sellingSFX, transform);
    }

    public void UpgradeEffect()
    {
        SteleAmelioration steleAmeliorationScript = SpawnedEffectObject.GetComponent<SteleAmelioration>();

        steleAmeliorationScript.UpgradeEffect();

        if (activationVFX.activeInHierarchy) activationVFX.SetActive(false);
        activationVFX.SetActive(true);
        UtilityClass.PlaySoundGroupImmediatly(upgradeSFX, transform);

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