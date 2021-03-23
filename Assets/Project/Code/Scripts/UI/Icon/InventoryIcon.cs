using UnityEngine.EventSystems;

public class InventoryIcon : SelectIcon, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryBox ParentInventoryBox => GetComponentInParent<InventoryBox>();
    private InventoryManager ParentPlayerInventory => ParentInventoryBox.PlayerInventory;
    private Item ItemInParentInventoryBox => ParentInventoryBox.StoredItem;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
        base.OnPointerDown(eventData);
        ParentPlayerInventory.ResetSomeSelectedIcons();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
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
