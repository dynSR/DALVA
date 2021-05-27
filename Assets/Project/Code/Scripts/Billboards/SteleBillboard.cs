using System.Collections.Generic;
using UnityEngine;

public class SteleBillboard : Billboard
{
    [Header("BUTTONS GROUP")]
    [SerializeField] private GameObject buttonsSectionsHolder;
    [SerializeField] private GameObject inactiveButtonSection;
    [SerializeField] private GameObject evolutionOneButtonSection;
    [SerializeField] private GameObject evolutionTwoButtonSection;
    [SerializeField] private GameObject finalEvolutionButtonSection;
    [SerializeField] private GameObject onlySellButtonSection;

    [Header("MISC")]
    [SerializeField] private GameObject healthBar;
    private SteleLogic stele;

    protected override void OnEnable()
    {
        stele.OnInteraction += DisplayBuildButtons;
        stele.OnEndOFInteraction += HideBuildButtons;

        stele.OnPurchase += HidePlayerSteleTooltip;
        stele.OnSell += HidePlayerSteleTooltip;

        //stele.OnActivation += DisplayBuildButtons();
        //stele.OnActivation += DisplayHealthBar;
    }

    protected override void OnDisable()
    {
        stele.OnInteraction -= DisplayBuildButtons;
        stele.OnEndOFInteraction -= HideBuildButtons;

        stele.OnPurchase -= HidePlayerSteleTooltip;
        stele.OnSell -= HidePlayerSteleTooltip;

        //stele.OnActivation -= DisplayBuildButtons();
        //stele.OnActivation -= DisplayHealthBar;
    }

    protected override void Awake()
    {
        base.Awake();
        stele = GetComponentInParent<SteleLogic>();
    }
    protected override void Start() => base.Start();

    protected override void LateUpdate() => base.LateUpdate();

    public void DisplayBuildButtons()
    {
        switch (stele.SteleLevel)
        {
            case SteleLevel.EvolutionI:
                evolutionOneButtonSection.SetActive(true);
                break;
            case SteleLevel.EvolutionII:
                evolutionTwoButtonSection.SetActive(true);
                break;
            case SteleLevel.FinalEvolution:
                finalEvolutionButtonSection.SetActive(true);
                break;
            case SteleLevel.OnlySell:
                onlySellButtonSection.SetActive(true);
                break;
            default:
                inactiveButtonSection.SetActive(true);
                break;
        }

        buttonsSectionsHolder.SetActive(true);
    }

    public void HideBuildButtons()
    {
        buttonsSectionsHolder.SetActive(false);
    }

    private void HidePlayerSteleTooltip()
    {
        GameManager.Instance.Player.GetComponentInChildren<PlayerHUDManager>().SteleTooltip.SetActive(false);
    }

    //On Button
    public void HideButtonSection(GameObject sectionToHide)
    {
        sectionToHide.SetActive(false);
    }
}
