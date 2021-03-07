using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HarvestState { WaitsUntilHarvestingIsPossible, IsHarvesting, Reinitialization, PlayerIsHarvestingRessources, Standby }

public class HarvesterLogic : MonoBehaviour
{
    public delegate void HarvestEvent(float current, float maximum);
    public static event HarvestEvent OnHarvestingRessources;

    [Header("RESSOURCES")]
    [SerializeField] private float currentHarvestedRessourcesValue = 0f;
    [SerializeField] private float maxHarvestableRessourcesValue;

    [Header("TIMERS")]
    [SerializeField] private float delayBeforeHarvesting = 30f;
    [SerializeField] private float reinitializationDelay = 45f;
    float timeSpentHarvesting = 0f;
    [SerializeField] private float totalTimeToHarvest = 10f;

    [Header("FEEDBACKS")]
    [SerializeField] private Image harvestingFeedbackImage;
    public HarvestState harvestState; //Its in public for debug purpose

    public Transform PlayerFound;
    public bool IsInteractable => harvestState == HarvestState.Standby || harvestState == HarvestState.IsHarvesting;

    public float CurrentHarvestedRessourcesValue { get => currentHarvestedRessourcesValue; set => currentHarvestedRessourcesValue = Mathf.Clamp(value, 0, maxHarvestableRessourcesValue); }

    private bool LimitReached => currentHarvestedRessourcesValue >= maxHarvestableRessourcesValue;

    private EntityDetection EntityDetection => GetComponent<EntityDetection>();

    void Start() => InitHarvester();

    void Update()
    {
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
            default:
                break;
        }

        EnableEntityDetection();
    }

    #region Harvest Functionnement
    private void InitHarvester()
    {
        CurrentHarvestedRessourcesValue = 0;

        OnHarvestingRessources?.Invoke(CurrentHarvestedRessourcesValue, maxHarvestableRessourcesValue);

        ResetHarvestingFeedback();
    }

    private void HarvestRessourcesOverTime()
    {
        if (LimitReached)
        {
            harvestState = HarvestState.Standby;
            return;
        }

        CurrentHarvestedRessourcesValue += Time.deltaTime;

        OnHarvestingRessources?.Invoke(CurrentHarvestedRessourcesValue, maxHarvestableRessourcesValue);
    }

    private void Interaction()
    {
        if (PlayerFound.GetComponent<InteractionsSystem>().IsHarvesting)
        {
            timeSpentHarvesting += Time.deltaTime;
            harvestingFeedbackImage.fillAmount = timeSpentHarvesting / totalTimeToHarvest;

            if (timeSpentHarvesting >= totalTimeToHarvest)
            {
                GiveRessourcesToPlayer((int)CurrentHarvestedRessourcesValue);
                harvestState = HarvestState.Reinitialization;
            }
        }
        else
        {
            Debug.Log("INTERACTION IS OVER");
            ResetAfterInteraction();
        }
    }

    private void GiveRessourcesToPlayer(int amnt)
    {
        PlayerFound.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources += amnt;
    }

    private void ResetAfterInteraction()
    {
        if (LimitReached)
            harvestState = HarvestState.Standby;
        else if(!LimitReached)
            harvestState = HarvestState.IsHarvesting;

        print("Reset player found");
        PlayerFound = null;
        ResetHarvestingFeedback();
    }

    private void ResetHarvestingFeedback()
    {
        harvestingFeedbackImage.fillAmount = 0 / totalTimeToHarvest;
        timeSpentHarvesting = 0f;
    }

    private void ReinitializeHarvester()
    {
        InitHarvester();

        timeSpentHarvesting = 0;

        StartCoroutine(WaitingState(reinitializationDelay, HarvestState.IsHarvesting));

        if (PlayerFound == null) return;

        PlayerFound.GetComponent<TargetHandler>().Target = null;
        PlayerFound.GetComponent<InteractionsSystem>().ResetInteractionState();
        PlayerFound = null;
    }

    private void EnableEntityDetection()
    {
        if (IsInteractable)
        {
            EntityDetection.enabled = true;
        }
        else
        {
            EntityDetection.enabled = false;
        }
    }

    private IEnumerator WaitingState(float delay, HarvestState newHarvestState)
    {
        yield return new WaitForSeconds(delay);

        this.harvestState = newHarvestState;
    }
    #endregion
}