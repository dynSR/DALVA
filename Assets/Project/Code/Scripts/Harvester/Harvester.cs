using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HarvestState { WaitsUntilHarvestingIsPossible, IsHarvesting, Reinitialization, APlayerIsCollectingHarvestedRessources, Standby }

//Interaction avec stèle 
//Cast Capture
//ajouter une réf au joueur en train de capturer le récolteur + ajouter un state et une animation sur le character controller 

public class Harvester : MonoBehaviour
{
    public delegate void HarvestEvent(float current, float maximum);
    public static event HarvestEvent OnHarvestingRessources;

    [Header("HARVESTER VARIABLES")]
    [SerializeField] private float currentHarvestableRessourcesValue = 0f;
    [SerializeField] private float maxHarvestableRessourcesValue;
    [SerializeField] private float delayBeforeReinitialization = 5f;
    [SerializeField] private float delayBeforeHarvesting = 2f;
    public HarvestState harvestState; //Its in public for debug purpose

    public Transform PlayerFound { get; set; }

    public float CurrentHarvestableRessourcesValue { get => currentHarvestableRessourcesValue; set => currentHarvestableRessourcesValue = Mathf.Clamp(value, 0, maxHarvestableRessourcesValue); }

    private bool LimitReached => currentHarvestableRessourcesValue >= maxHarvestableRessourcesValue;

    void Start()
    {
        InitHarvester();
    }

    void Update()
    {
        switch (harvestState)
        {
            case HarvestState.WaitsUntilHarvestingIsPossible:
                StartCoroutine(WaitingState(delayBeforeHarvesting, HarvestState.IsHarvesting));
                break;
            case HarvestState.IsHarvesting:
                HarvestOverTime();
                break;
            case HarvestState.Reinitialization:
                ReinitializeHarvest();
                break;
            case HarvestState.APlayerIsCollectingHarvestedRessources:
                if (!PlayerFoundIsNoLongerCollecting())
                    ResetAfterPlayerIsNoLongerCollecting();
                break;
            default:
                break;
        }
    }

    #region Harvest Functionnement
    private void InitHarvester()
    {
        CurrentHarvestableRessourcesValue = 0;

        OnHarvestingRessources?.Invoke(CurrentHarvestableRessourcesValue, maxHarvestableRessourcesValue);

        harvestState = HarvestState.WaitsUntilHarvestingIsPossible;
    }

    private void HarvestOverTime()
    {
        if (LimitReached)
        {
            harvestState = HarvestState.Standby;
            return;
        }

        CurrentHarvestableRessourcesValue += Time.deltaTime;

        OnHarvestingRessources?.Invoke(CurrentHarvestableRessourcesValue, maxHarvestableRessourcesValue);
    }

    private IEnumerator WaitingState(float delay, HarvestState newHarvestState)
    {
        yield return new WaitForSeconds(delay);

        this.harvestState = newHarvestState;
    }

    private void ReinitializeHarvest()
    {
        CurrentHarvestableRessourcesValue = 0;

        OnHarvestingRessources?.Invoke(CurrentHarvestableRessourcesValue, maxHarvestableRessourcesValue);

        harvestState = HarvestState.IsHarvesting;
    }

    private void ResetAfterPlayerIsNoLongerCollecting()
    {
        if (LimitReached)
            harvestState = HarvestState.Standby;
        else
            harvestState = HarvestState.IsHarvesting;

        PlayerFound = null;
    }

    private bool PlayerFoundIsNoLongerCollecting()
    {
        if (PlayerFound.GetComponent<CharacterInteractions>().IsCollecting)
        {
            return true;
        }
        else return false;
    }
    #endregion
}