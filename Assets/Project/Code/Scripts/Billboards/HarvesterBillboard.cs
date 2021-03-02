using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HarvesterBillboard : BillBoard
{
    [Header("HARVESTER BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI harvestedRessourcesText;
    [SerializeField] private Image harvestedRessourcesBarFilledImage;

    private void OnEnable()
    {
        Harvester.OnHarvestingRessources += SetHarvesterUIElements;
    }

    private void OnDisable()
    {
        Harvester.OnHarvestingRessources -= SetHarvesterUIElements;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void SetHarvesterUIElements(float current, float maximum)
    {
        harvestedRessourcesBarFilledImage.fillAmount = current / maximum;
        harvestedRessourcesText.text = current.ToString("0") + " / " + maximum.ToString();
    }
}