using UnityEngine.EventSystems;

public class ShopIcon : SelectIcon, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void ItemSelectionHandler(Item selectedItem);
    public static event ItemSelectionHandler OnSelectingAnItem;
    public static event ItemSelectionHandler OnDeselectingAnItem;

    private ShopManager ShopManager => ShopWindow.GetComponentInChildren<ShopManager>();
    private ItemButton ItemButton;
    private AbilityButton AbilityButton;

    private void Awake()
    {
        if(GetComponentInParent<AbilityButton>() != null) AbilityButton = GetComponentInParent<AbilityButton>();
        if (GetComponentInParent<ItemButton>() != null) ItemButton = GetComponentInParent<ItemButton>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    protected override void SetSelection()
    {
        ShopManager.ShopItemIsSelected = true;
        ShopManager.SelectedItem = ItemButton.ButtonItem;

        OnSelectingAnItem?.Invoke(ShopManager.SelectedItem);
    }

    public override void ResetSelection()
    {
        ShopManager.ShopItemIsSelected = false;
        ShopManager.SelectedItem = null;

        OnDeselectingAnItem?.Invoke(null);
    }
}
