using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetShopButton : UIButtonWithTooltip, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ShopManager shop;
    [SerializeField] private TextMeshProUGUI resetDrawCostText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;
    Color initialResetDrawCostTextColor;
    private CharacterRessources PlayerRessources => shop.Player.GetComponent<CharacterRessources>();

    Button buttonComponent => GetComponent<Button>();

    protected override void OnEnable()
    {
        base.OnEnable();
        shop.OnShopDrawSetResetCost += SetResetDrawCostText;
        PlayerRessources.OnCharacterRessourcesChanged += ToggleButtonStatus;
    }

    private void OnDisable()
    {
        shop.OnShopDrawSetResetCost -= SetResetDrawCostText;
        PlayerRessources.OnCharacterRessourcesChanged -= ToggleButtonStatus;
    }

    void Awake()
    {
        initialResetDrawCostTextColor = resetDrawCostText.color;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    void SetResetDrawCostText(int value)
    {
        resetDrawCostText.SetText(value.ToString("0"));
    }

    void ToggleButtonStatus()
    {
        UIButtonSound uiButtonSoundScript = GetComponent<UIButtonSound>();

        if (PlayerRessources.CurrentAmountOfPlayerRessources >= shop.ResetDrawCost)
        {
            resetDrawCostText.color = initialResetDrawCostTextColor;
            backgroundImage.color = enabledColor;
            uiButtonSoundScript.enabled = true;
        }
        else if (PlayerRessources.CurrentAmountOfPlayerRessources < shop.ResetDrawCost)
        {
            resetDrawCostText.color = Color.red;
            backgroundImage.color = disabledColor;
            uiButtonSoundScript.enabled = false;
        }
    }
}