using UnityEngine;
using UnityEngine.EventSystems;

public class ShopIcon : SelectIcon, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void ItemSelectionHandler(Item selectedItem);
    public static event ItemSelectionHandler OnSelectingAnItem;
    public static event ItemSelectionHandler OnDeselectingAnItem;

    private ShopManager ShopManager => ShopWindow.GetComponentInChildren<ShopManager>();
    private ItemButton itemButton;

    ShopInformationPanel shopInformationPanelRef;
    PlayerHUDManager playerHUDManager;
    public ItemButton ItemButton { get => itemButton; }

    private void Awake()
    {
        if (GetComponentInParent<ItemButton>() != null) itemButton = GetComponentInParent<ItemButton>();
    }

    void Start()
    {
        shopInformationPanelRef = PlayerHUDManager.Instance.ShopInformationPanel;
        playerHUDManager = PlayerHUDManager.Instance;
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

        // Mettre ici la désactivation de la partie information
        DesactivateShopInformationPanel();

        // Mettre ici l'activation de la partie information
        ActivateShopInformationPanel();
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

    void ActivateShopInformationPanel()
    {
        if (UtilityClass.LeftClickIsPressed() && playerHUDManager.ShopInformationPanel.CGroup.alpha == 0)
        {
            StartCoroutine(shopInformationPanelRef.ActivateObject());

            playerHUDManager.RepositionShopWindow(425);
        }
    }

    void DesactivateShopInformationPanel()
    {
        if (UtilityClass.LeftClickIsPressed() 
            && playerHUDManager.ShopWindow.GetComponent<ShopManager>().SelectedItem == null)
        {
            StartCoroutine(shopInformationPanelRef.DesactivateObject());

            playerHUDManager.RepositionShopWindow(495);
        }
    }
}
