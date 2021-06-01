using DarkTonic.MasterAudio;
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
    [SerializeField] private ShopInformationPanel shopInformationPanel;

    [Header("INVENTORY")]
    [SerializeField] private TextMeshProUGUI inventoryPlayerRessourcesValueText;

    [Header("STELETOOLTIP")]
    [SerializeField] private GameObject steleTooltip;

    [Header("UI SOUNDS")]
    [SoundGroup] public string oppeningShopSoundGroup;
    [SoundGroup] public string closingShopSoundGroup;

    public GameObject ShopWindow { get => shopWindow; }
    public TextMeshProUGUI ShopPlayerRessourcesValueText { get => shopPlayerRessourcesValueText; }
    public TextMeshProUGUI InventoryPlayerRessourcesValueText { get => inventoryPlayerRessourcesValueText; }

    public bool IsShopWindowOpen { get => isShopWindowOpen; set => isShopWindowOpen = value; }
    public Transform Player { get => player; }
    public GameObject SteleTooltip { get => steleTooltip; }

    #region Singleton
    public static PlayerHUDManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            statusEffectHandler = player.GetComponent<StatusEffectHandler>();
        }
    }
    #endregion

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
        //To comment
        if (UtilityClass.IsKeyPressed(toggleInputKey) && GameManager.Instance.GameIsInPlayMod())
            ToggleAWindow(IsShopWindowOpen, ShopWindow);
    }

    #region Status effects
    public void UpdateStatusEffectUI(StatusEffect statusEffect)
    {
        if (statusEffectLayoutGroup.childCount == 0) CreateContainer(statusEffect);

        if (statusEffectLayoutGroup.childCount > 0)
        {

            for (int i = 0; i < statusEffectLayoutGroup.childCount; i++)
            {
                if (!statusEffectHandler.IsEffectAlreadyApplied(statusEffect))
                {
                    CreateContainer(statusEffect);
                    Debug.Log("Not the Same Effect ID > Create Container");
                    return;
                }

                if (statusEffect.StatusEffectId == statusEffectHandler.AppliedStatusEffects[i].statusEffect.StatusEffectId)
                {
                    Debug.Log("Same Effect ID");
                    StatusEffect foundStatusEffect = statusEffectHandler.AppliedStatusEffects[i].statusEffect;

                    foundStatusEffect.StatusEffectContainer.ResetTimer();

                    //Stackable increment something here
                    if (foundStatusEffect.IsStackable)
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

        if (statusEffect.Type == StatusEffectType.Harmless) containerInstance.transform.SetParent(statusEffectLayoutGroup.GetChild(0));
        else if (statusEffect.Type == StatusEffectType.Harmful) containerInstance.transform.SetParent(statusEffectLayoutGroup.GetChild(1));

        StatusEffectContainer container = containerInstance.GetComponent<StatusEffectContainer>();

        container.StatusEffectHandler = statusEffect.TargetStatusEffectHandler;

        container.ContainedStatusEffect = statusEffect;

        statusEffect.StatusEffectContainer = container;
    }
    #endregion

    #region Player HUD windows behaviour
    #region Toggle
    public void ToggleAWindow(bool value, GameObject window)
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
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();

        IsShopWindowOpen = true;
        UtilityClass.PlaySoundGroupImmediatly(oppeningShopSoundGroup, UtilityClass.GetMainCamera().transform);
        //MasterAudio.PlaySound(oppeningShopSoundGroup);

        if (shop.ShuffleItemsOnlyWhenFifthWaveIsOver && GameManager.Instance.WaveDone == 6)
        {
            shop.ResetDrawAndShuffleAgain();
        }

        ResetShopWindowAnchoredPosition();
        shop.RefreshShopData();
        ResetShopWindowSelectionsOnBoxes();

        if(shop.ShopItemIsSelected)
        {
            shopInformationPanel.HideContent();
            shop.ShopItemIsSelected = false;
        }

        for (int i = 0; i < shop.ShopBoxesIcon.Count; i++)
        {
            shop.ShopBoxesIcon[i].ToggleOff();

            if (!shop.ShopBoxesIcon[i].Tooltip.activeInHierarchy) continue;

            shop.ShopBoxesIcon[i].HideTooltip(shop.ShopBoxesIcon[i].Tooltip);
        }
    }

    void OnClosingShopWindow()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();

        IsShopWindowOpen = false;

        shop.SelectedItem = null;

        UtilityClass.PlaySoundGroupImmediatly(closingShopSoundGroup, UtilityClass.GetMainCamera().transform);
        //MasterAudio.PlaySound(closingShopSoundGroup);
    }

    void ResetShopWindowSelectionsOnBoxes()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();
        shop.PlayerInventory.ResetAllSelectedIcons();
    }

    void ResetShopWindowAnchoredPosition()
    {
        Vector2 offset = Vector2.zero;
        ShopWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset.x, offset.y + 50f);
    }
    #endregion
}