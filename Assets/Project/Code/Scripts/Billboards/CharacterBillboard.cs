using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBillboard : BillBoard
{
    [Header("CHARACTER BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Image characterHealthBarFilledImage;
    [SerializeField] private TextMeshProUGUI characterLevelText;

    protected override void Start()
    {
        base.Start();

        //playerNameText.text = GetPhotonNetworkUsername(); --> à ajouter dans la class Utility
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }
}