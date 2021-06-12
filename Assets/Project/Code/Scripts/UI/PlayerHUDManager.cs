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

    CursorLogic cursorLogic;

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

            statusEffectHandler = Player.GetComponent<StatusEffectHandler>();
            cursorLogic = Player.GetComponent<CursorLogic>();
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

    public void ToggleShopWindowOnButtonClick()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

        if (IsShopWindowOpen)
        {
            Debug.Log("Shop was opened");
            CloseWindow(ShopWindow);
        }
        else if (!IsShopWindowOpen && !GameManager.Instance.tutorielDisplayed)
        {
            Debug.Log("Shop was closed");
            OpenWindow(ShopWindow);
        }
    }
    #endregion
    #region Open - Close
    public void OpenWindow(GameObject window)
    {
        if (window == ShopWindow)
        {
            CanvasGroup cG = window.GetComponent<CanvasGroup>();
            cG.alpha = 1;
            cG.blocksRaycasts = true;

            OnOpenningShopWindow();

            return;
        }

        window.SetActive(true);
    }

    public void CloseWindow(GameObject window)
    {
        if (window == ShopWindow)
        {
            CanvasGroup cG = window.GetComponent<CanvasGroup>();
            cG.alpha = 0;
            cG.blocksRaycasts = false;

            OnClosingShopWindow();
            return;
        }

        window.SetActive(false);
    }
    #endregion
    #endregion

    #region Shop Window
    void OnOpenningShopWindow()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();

        StartCoroutine(shop.itemPanel.EnableAllCanvasGroup());

        cursorLogic.SetCursorToNormalAppearance();

        IsShopWindowOpen = true;
        UtilityClass.PlaySoundGroupImmediatly(oppeningShopSoundGroup, transform);

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
            shop.ShopBoxesIcon[i].EnableCanvasGroup();

            if (!shop.ShopBoxesIcon[i].Tooltip.activeInHierarchy) continue;

            shop.ShopBoxesIcon[i].HideTooltip(shop.ShopBoxesIcon[i].Tooltip);
        }

        GameManager.Instance.SetTimeScale(0);
    }

    void OnClosingShopWindow()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();

        for (int i = 0; i < shop.ShopBoxesIcon.Count; i++)
        {
            shop.ShopBoxesIcon[i].DisableCanvasGroup();
        }

        StartCoroutine(shop.itemPanel.DisableAllCanvasGroup());

        IsShopWindowOpen = false;

        shop.SelectedItem = null;

        if (!GameManager.Instance.tutorielDisplayed)
            UtilityClass.PlaySoundGroupImmediatly(closingShopSoundGroup, transform);

        GameManager.Instance.SetTimeScale(1);
    }

    void ResetShopWindowSelectionsOnBoxes()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();
        shop.PlayerInventory.ResetAllSelectedIcons();
    }

    void ResetShopWindowAnchoredPosition()
    {
        Vector2 offset = Vector2.zero;
        ShopWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset.x + 150, offset.y +10);
    }
    #endregion


    public void ToggleCameraStateOnButtonClick()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

        CameraController cameraController = UtilityClass.GetMainCamera().GetComponent<CameraController>();

        cameraController.SetCameraLockState();
    }
}