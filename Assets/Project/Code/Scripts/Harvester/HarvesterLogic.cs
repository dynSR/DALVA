using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HarvestState { WaitsUntilHarvestingIsPossible, IsHarvesting, Reinitialization, PlayerIsHarvestingRessources }

public class HarvesterLogic : MonoBehaviour
{
    public delegate void HarvestEvent(float current, float maximum);
    public static event HarvestEvent OnHarvestingRessources;

    [Header("RESSOURCES")]
    [SerializeField] private float maxHarvestableRessourcesValue;
    public float CurrentHarvestedRessourcesValue { get; set; }

    [Header("TIMERS")]
    [SerializeField] private float delayBeforeHarvesting = 30f;
    [SerializeField] private float reinitializationDelay = 45f;
    float timeSpentHarvesting = 0f;
    [SerializeField] private float totalTimeToHarvest = 10f;

    [Header("FEEDBACKS")]
    [SerializeField] private Image harvestingFeedbackImage;
    public HarvestState harvestState; //Its in public for debug purpose

    public PlayerInteractions interactingPlayer;
    public bool IsInteractable => CurrentHarvestedRessourcesValue >= .5f && harvestState == HarvestState.IsHarvesting;
    private bool LimitReached => CurrentHarvestedRessourcesValue >= maxHarvestableRessourcesValue;

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
        if (interactingPlayer != null)
        {
            harvestState = HarvestState.PlayerIsHarvestingRessources;
            return;
        }

        if (!LimitReached)
        {
            CurrentHarvestedRessourcesValue += Time.deltaTime;

            OnHarvestingRessources?.Invoke(CurrentHarvestedRessourcesValue, maxHarvestableRessourcesValue);
        }
    }

    private void Interaction()
    {
        timeSpentHarvesting += Time.deltaTime;
        harvestingFeedbackImage.fillAmount = timeSpentHarvesting / totalTimeToHarvest;

        if (timeSpentHarvesting >= totalTimeToHarvest)
        {
            CharacterStat interactingPlayerStat = interactingPlayer.GetComponent<CharacterStat>();

            GiveRessourcesToPlayer((int)CurrentHarvestedRessourcesValue);
            harvestState = HarvestState.Reinitialization;
            Popup.Create(interactingPlayerStat.InFrontOfCharacter, interactingPlayerStat.Popup, CurrentHarvestedRessourcesValue, StatType.RessourcesGiven, interactingPlayerStat.GetStat(StatType.RessourcesGiven).Icon);
        }
    }

    private void GiveRessourcesToPlayer(int amnt)
    {
        interactingPlayer.GetComponent<CharacterRessources>().CurrentAmountOfPlayerRessources += amnt;
    }

    public void ResetAfterInteraction()
    {
        harvestState = HarvestState.IsHarvesting;

        interactingPlayer = null;

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

        timeSpentHarvesting = 0;

        StartCoroutine(WaitingState(reinitializationDelay, HarvestState.IsHarvesting));

        if (interactingPlayer != null)
        {
            interactingPlayer.Target = null;
            interactingPlayer.ResetInteractionState();
            interactingPlayer = null;
        }
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
        yield return new WaitForEndOfFrame();

        harvestState = newHarvestState;
    }
    #endregion
}