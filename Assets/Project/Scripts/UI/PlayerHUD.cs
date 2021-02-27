using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("STATUS EFFECT PARAMETERS")]
    [SerializeField] private Transform statusEffectLayoutGroup;
    [SerializeField] private GameObject statusEffectGameObject;

    [Header("SHOP")]
    [SerializeField] private KeyCode toggleInputKey;
    [SerializeField] private GameObject shopWindow;
    public bool isShopWindowOpen = false;
    [SerializeField] private TextMeshProUGUI shopPlayerRessourcesValueText;

    [Header("INVENTORY")]
    [SerializeField] private TextMeshProUGUI inventoryPlayerRessourcesValueText;

    private GameObject ShopWindow { get => shopWindow; }
    public TextMeshProUGUI ShopPlayerRessourcesValueText { get => shopPlayerRessourcesValueText; }
    public TextMeshProUGUI InventoryPlayerRessourcesValueText { get => inventoryPlayerRessourcesValueText; }

    public bool IsShopWindowOpen { get => isShopWindowOpen; set => isShopWindowOpen = value; }
    
    void OnEnable()
    {
        StatusEffectHandler.OnApplyingStatusEffectEvent += UpdateStatusEffectUI;
    }

    void OnDisable()
    {
        StatusEffectHandler.OnApplyingStatusEffectEvent -= UpdateStatusEffectUI;
    }

    void Update()
    {
        if (UtilityClass.IsKeyPressed(toggleInputKey))
            ToggleAWindow(IsShopWindowOpen, ShopWindow);
    }

    #region Status effects
    public void UpdateStatusEffectUI(StatusEffectSystem statusEffect)
    {
        GameObject statusEffectFeedbackInstance = Instantiate(statusEffectGameObject) as GameObject;
        statusEffectFeedbackInstance.transform.SetParent(statusEffectLayoutGroup);

        statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>().ContainedStatusEffectSystem = statusEffect;
        statusEffect.StatusEffectContainer = statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>();
    }
    #endregion

    #region Player HUD windows behaviour
    #region Toggle
    void ToggleAWindow(bool value, GameObject window)
    {
        if (!value)
        {
            OpenWindow(window);
        }
        else if (value)
        {
            CloseWindow(window);
        }
    }
    #endregion
    #region Open - Close
    public void OpenWindow(GameObject window)
    {
        ShopWindow.SetActive(true);

        if (window == ShopWindow)
            OnOpenningShopWindow();
    }

    public void CloseWindow(GameObject window)
    {
        window.SetActive(false);

        if (window == ShopWindow)
            OnClosingShopWindow();
    }
    #endregion
    #endregion

    #region Shop Window
    void OnOpenningShopWindow()
    {
        IsShopWindowOpen = true;
        ResetShopWindowAnchoredPosition();
    }

    void OnClosingShopWindow()
    {
        IsShopWindowOpen = false;
        ResetShopWindowSelectionsOnBoxes();
    }

    void ResetShopWindowSelectionsOnBoxes()
    {
        ShopWindow.GetComponent<Shop>().PlayerInventory.ResetAllBoxesSelectionIcons();
    }

    void ResetShopWindowAnchoredPosition()
    {
        ShopWindow.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    #endregion
}