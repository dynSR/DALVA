using TMPro;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("STATUS EFFECT PARAMETERS")]
    [SerializeField] private Transform statusEffectLayoutGroup;
    [SerializeField] private GameObject statusEffectContainer;

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
    public Transform Player { get => player; }

    void OnEnable()
    {
        StatusEffectHandler.OnApplyingStatusEffect += UpdateStatusEffectUI;
    }

    void OnDisable()
    {
        StatusEffectHandler.OnApplyingStatusEffect -= UpdateStatusEffectUI;
    }

    void Update()
    {
        if (UtilityClass.IsKeyPressed(toggleInputKey))
            ToggleAWindow(IsShopWindowOpen, ShopWindow);
    }

    #region Status effects
    public void UpdateStatusEffectUI(StatusEffect statusEffect)
    {
        if (statusEffectLayoutGroup.childCount > 0)
        {
            //Debug.Log("Container already exists");
            for (int i = statusEffectLayoutGroup.childCount - 1; i >= 0; i--)
            {
                if (statusEffectLayoutGroup.GetChild(i).GetComponent<StatusEffectContainer>().ContainedStatusEffectSystem == statusEffect)
                {
                    statusEffectLayoutGroup.GetChild(i).GetComponent<StatusEffectContainer>().ResetTimer();
                }
            }
        }
        else
        {
            //Debug.Log("Create Container");

            GameObject containerInstance = Instantiate(statusEffectContainer);
            containerInstance.transform.SetParent(statusEffectLayoutGroup);

            StatusEffectContainer container = containerInstance.GetComponent<StatusEffectContainer>();

            container.StatusEffectHandler = statusEffect.TargetStatusEffectHandler;

            container.ContainedStatusEffectSystem = statusEffect;

            statusEffect.StatusEffectContainer = container;
        }
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
        window.SetActive(true);

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
        ShopWindow.GetComponent<ShopManager>().PlayerInventory.ResetAllSelectedIcons();
    }

    void ResetShopWindowAnchoredPosition()
    {
        ShopWindow.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    #endregion
}