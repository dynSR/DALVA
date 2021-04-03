using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HarvesterBillboard : Billboard
{
    [Header("HARVESTER BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI harvestedRessourcesText;
    [SerializeField] private Image filledImage;
    private HarvesterLogic harvester;

    private void OnEnable()
    {
        harvester.OnHarvestingRessources += SetHarvesterUIElements;
    }

    private void OnDisable()
    {
        harvester.OnHarvestingRessources -= SetHarvesterUIElements;
    }

    protected override void Awake()
    {
        harvester = GetComponentInParent<HarvesterLogic>();
        base.Awake();
    }
    protected override void Start() => base.Start();

    protected override void LateUpdate() => base.LateUpdate();

    private void SetHarvesterUIElements(float current, float maximum)
    {
        filledImage.fillAmount = current / maximum;
        harvestedRessourcesText.text = current.ToString("0") + " / " + maximum.ToString();
    }
}