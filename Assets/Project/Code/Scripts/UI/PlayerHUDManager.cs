using TMPro;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("STATUS EFFECT PARAMETERS")]
    [SerializeField] private Transform statusEffectLayoutGroup;
    [SerializeField] private GameObject statusEffectContainer;
    private StatusEffectHandler statusEffectHandler;

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

    private void Awake()
    {
        statusEffectHandler = player.GetComponent<StatusEffectHandler>();
    }

    void OnEnable()
    {
        statusEffectHandler.OnApplyingStatusEffect += UpdateStatusEffectUI;
    }

    void OnDisable()
    {
        statusEffectHandler.OnApplyingStatusEffect -= UpdateStatusEffectUI;
    }

    void Update()
    {
        if (UtilityClass.IsKeyPressed(toggleInputKey))
            ToggleAWindow(IsShopWindowOpen, ShopWindow);
    }

    #region Status effects
    public void UpdateStatusEffectUI(StatusEffect statusEffect)
    {
        if (statusEffectLayoutGroup.childCount == 0) CreateContainer(statusEffect);

        if (statusEffectLayoutGroup.childCount >= 1)
        {
            for (int i = 0; i < statusEffectLayoutGroup.childCount; i++)
            {
                StatusEffectContainer statusEffectContainer = statusEffectLayoutGroup.GetChild(i).GetComponent<StatusEffectContainer>();
                StatusEffect containedStatusEffect = statusEffectContainer.ContainedStatusEffect;

                if (containedStatusEffect != statusEffect && !statusEffectHandler.IsEffectAlreadyApplied(statusEffect))
                {
                    CreateContainer(statusEffect);
                    Debug.Log("Not the Same Effect ID > Create Container");
                    return;
                }

                //Si un de ces effets est le même que celui appliqué > Reset 
                if (containedStatusEffect.StatusEffectId == statusEffect.StatusEffectId)
                {
                    Debug.Log("Same Effect ID");
                    statusEffectContainer.ResetTimer();

                    //Stackable increment something here
                    if(containedStatusEffect.IsStackable)
                    {
                        //Do something here
                    }
                }
            }
        }
    }

    private void CreateContainer(StatusEffect statusEffect)
    {
        Debug.Log("Create Container");

        GameObject containerInstance = Instantiate(statusEffectContainer);
        containerInstance.transform.SetParent(statusEffectLayoutGroup);

        StatusEffectContainer container = containerInstance.GetComponent<StatusEffectContainer>();

        container.StatusEffectHandler = statusEffect.TargetStatusEffectHandler;

        container.ContainedStatusEffect = statusEffect;

        statusEffect.StatusEffectContainer = container;
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
        Vector2 offset = Vector2.zero;
        ShopWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset.x, offset.y + 50f);
    }
    #endregion
}