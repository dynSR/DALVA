using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetShopButton : UIButtonWithTooltip, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ShopManager shop;
    [SerializeField] private TextMeshProUGUI resetDrawCostText;


    private void OnEnable()
    {
        shop.OnShopResetDraw += SetResetDrawCostText;
    }

    private void OnDisable()
    {
        shop.OnShopResetDraw -= SetResetDrawCostText;
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
}