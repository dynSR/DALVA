using UnityEngine.EventSystems;

public class ShopIcon : SelectIcon, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void ItemSelectionHandler(Item selectedItem);
    public static event ItemSelectionHandler OnSelectingAnItem;
    public static event ItemSelectionHandler OnDeselectingAnItem;

    private ShopManager ShopManager => ShopWindow.GetComponentInChildren<ShopManager>();
    private ShopButtonLogic ShopButton => GetComponentInParent<ShopButtonLogic>();

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
        //if(IsSelected)
        //{
        //    //Call Event here
        //    OnSelectingAnItem?.Invoke(ShopManager.SelectedItem);
        //}

        ShopManager.ShopItemIsSelected = true;
        ShopManager.SelectedItem = ShopButton.ShopButtonItem;

       
    }

    public override void ResetSelection()
    {
        ShopManager.ShopItemIsSelected = false;
        ShopManager.SelectedItem = null;

        //OnDeselectingAnItem?.Invoke(null);
    }
}
