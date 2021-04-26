using UnityEngine.EventSystems;

public class ShopIcon : SelectIcon, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void ItemSelectionHandler(Item selectedItem);
    public static event ItemSelectionHandler OnSelectingAnItem;
    public static event ItemSelectionHandler OnDeselectingAnItem;

    private ShopManager ShopManager => ShopWindow.GetComponentInChildren<ShopManager>();
    private ItemButton itemButton;
    public ItemButton ItemButton { get => itemButton; }

    private void Awake()
    {
        if (GetComponentInParent<ItemButton>() != null) itemButton = GetComponentInParent<ItemButton>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        Tooltip.GetComponent<TooltipSetter>().SetTooltip(
               itemButton.ButtonItem.ItemName,
               itemButton.ButtonItem.ItemDescription,
               itemButton.ButtonItem.ItemCost.ToString("0"));
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ShopManager.ResetSelectionIcon(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    protected override void SetSelection()
    {
        ShopManager.ShopItemIsSelected = true;
        ShopManager.SelectedItem = itemButton.ButtonItem;

        OnSelectingAnItem?.Invoke(ShopManager.SelectedItem);
    }

    public override void ResetSelection()
    {
        ShopManager.ShopItemIsSelected = false;
        ShopManager.SelectedItem = null;

        OnDeselectingAnItem?.Invoke(null);
    }
}
