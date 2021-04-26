using UnityEngine.EventSystems;

public class InventoryIcon : SelectIcon, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryBox ParentInventoryBox => GetComponentInParent<InventoryBox>();
    private InventoryManager ParentPlayerInventory => ParentInventoryBox.PlayerInventory;
    private Item ItemInParentInventoryBox => ParentInventoryBox.StoredItem;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(ItemInParentInventoryBox != null)
            base.OnPointerEnter(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!IsSelected && ItemInParentInventoryBox != null)
            ParentPlayerInventory.ResetSomeSelectedIcons();

        if (ItemInParentInventoryBox != null)
            base.OnPointerDown(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (ItemInParentInventoryBox != null)
            base.OnPointerExit(eventData);
    }

    protected override void SetSelection()
    {
        if (ItemInParentInventoryBox != null)
        {
            ParentPlayerInventory.Shop.InventoryItemIsSelected = true;
            ParentPlayerInventory.Shop.SelectedInventoryBox = ParentInventoryBox;
        }
    }

    public override void ResetSelection()
    {
        if (ItemInParentInventoryBox != null)
        {
            ParentPlayerInventory.Shop.InventoryItemIsSelected = false;
            ParentPlayerInventory.Shop.SelectedInventoryBox = null;
        }
    }
}
