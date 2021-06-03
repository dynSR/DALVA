using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum HarvestState { WaitsUntilHarvestingIsPossible, IsHarvesting, Reinitialization, PlayerIsHarvestingRessources }

public class HarvesterLogic : InteractiveBuilding
{
    public delegate void HarvestEvent(float current, float maximum);
    public event HarvestEvent OnHarvestingRessources;

    [Header("RESSOURCES")]
    [SerializeField] private float maxHarvestableRessourcesValue;
    public float CurrentHarvestedRessourcesValue { get; set; }

    [Header("TIMERS")]
    [SerializeField] private float delayBeforeHarvesting = 30f;
    float timeSpentHarvesting = 0f;
    [SerializeField] private float totalTimeToHarvest = 10f;
    [SerializeField] private float harvestingTimeMultiplier = 1f;

    [Header("FEEDBACKS")]
    [SerializeField] private Image harvestingFeedbackImage;
    [SerializeField] private GameObject glowEffectObject;
    [SerializeField] private GameObject harvestingEffectObject;
    [SerializeField] private GameObject maxEffectObject;
    public HarvestState harvestState; //Its in public for debug purpose
    private bool LimitReached => CurrentHarvestedRessourcesValue >= maxHarvestableRessourcesValue;

    void Start() => InitHarvester();

    protected override void LateUpdate()
    {
        base.LateUpdate();

        switch (harvestState)
        {
            case HarvestState.WaitsUntilHarvestingIsPossible:
                StartCoroutine(WaitingState(delayBeforeHarvesting, HarvestState.IsHarvesting));
                break;
            case HarvestState.IsHarvesting:
                HarvestRessourcesOverTime();
                break;
            case HarvestState.Reinitialization:
                ReinitializeHarvester();
                break;
            case HarvestState.PlayerIsHarvestingRessources:
                Interaction();
                break;
        }
    }

    #region Harvest Functionnement
    private void InitHarvester()
    {
        IsInteractable = false;

        CurrentHarvestedRessourcesValue = 0;

        OnHarvestingRessources?.Invoke(CurrentHarvestedRessourcesValue, maxHarvestableRessourcesValue);

        ResetHarvestingFeedback();
    }

    private void HarvestRessourcesOverTime()
    {
        if (InteractingPlayer != null)
        {
            harvestState = HarvestState.PlayerIsHarvestingRessources;
            if (harvestingEffectObject.activeInHierarchy) harvestingEffectObject.SetActive(false);
            return;
        }

        if (CurrentHarvestedRessourcesValue >= 1f)
        {
            IsInteractable = true;
            glowEffectObject.SetActive(true);
        }

        if (!LimitReached && GameManager.Instance.GameIsInPlayMod())
        {
            if (!harvestingEffectObject.activeInHierarchy) harvestingEffectObject.SetActive(true);
            CurrentHarvestedRessourcesValue += Time.deltaTime * harvestingTimeMultiplier;
            OnHarvestingRessources?.Invoke(CurrentHarvestedRessourcesValue, maxHarvestableRessourcesValue);
        }
        else if (LimitReached && harvestingEffectObject.activeInHierarchy)
        {
            harvestingEffectObject.SetActive(false);
            maxEffectObject.SetActive(true);
        }
    }

    private void Interaction()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

            timeSpentHarvesting += Time.deltaTime;
        harvestingFeedbackImage.fillAmount = timeSpentHarvesting / totalTimeToHarvest;

        GetComponentInChildren<Animator>().SetBool("Harvesting", true);

        if (timeSpentHarvesting >= totalTimeToHarvest)
        {
            EntityStats interactingPlayerStat = InteractingPlayer.GetComponent<EntityStats>();

            GiveRessourcesToPlayer((int)CurrentHarvestedRessourcesValue);
            harvestState = HarvestState.Reinitialization;
            Popup.Create(interactingPlayerStat.CharacterHalfSize, interactingPlayerStat.Popup, CurrentHarvestedRessourcesValue, StatType.RessourcesGiven, interactingPlayerStat.GetStat(StatType.RessourcesGiven).Icon);
        }
    }

    private void GiveRessourcesToPlayer(int amnt)
    {
        InteractingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources += amnt;
    }

    public override void ResetAfterInteraction()
    {
        base.ResetAfterInteraction();

        harvestState = HarvestState.IsHarvesting;
        GetComponentInChildren<Animator>().SetBool("Harvesting", false);
        ResetHarvestingFeedback();
    }

    private void ResetHarvestingFeedback()
    {
        harvestingFeedbackImage.fillAmount = 0f / totalTimeToHarvest;
        timeSpentHarvesting = 0f;
    }

    private void ReinitializeHarvester()
    {
        InitHarvester();

        glowEffectObject.SetActive(false);
        maxEffectObject.SetActive(false);
        GetComponentInChildren<Animator>().SetBool("Harvesting", false);

        timeSpentHarvesting = 0;

        StartCoroutine(WaitingState(ReinitializationDelay, HarvestState.IsHarvesting));

        if (InteractingPlayer != null)
        {
            InteractingPlayer.Target = null;
            InteractingPlayer.ResetInteractionState();
            InteractingPlayer = null;
        }
    }

    private IEnumerator WaitingState(float delay, HarvestState newHarvestState)
    {
        yield return new WaitUntil(() => GameManager.Instance.GameIsInPlayMod());

        yield return new WaitForSeconds(delay);
        yield return new WaitForEndOfFrame();

        harvestState = newHarvestState;
    }
    #endregion
}