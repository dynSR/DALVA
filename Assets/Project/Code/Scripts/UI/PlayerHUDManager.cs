using DarkTonic.MasterAudio;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    private Transform player;

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

    [Header("MINI MAP")]
    public GameObject minimapGameObject;

    [Header("PLAYER FRAME")]
    public GameObject playerFrame;

    [Header("INVENTORY")]
    [SerializeField] private TextMeshProUGUI inventoryPlayerRessourcesValueText;

    [Header("STELETOOLTIP")]
    [SerializeField] private GameObject steleTooltip;

    [Header("UI SOUNDS")]
    [SoundGroup] public string openingShopSoundGroup;
    [SoundGroup] public string closingShopSoundGroup;

    public GameObject ShopWindow { get => shopWindow; }
    public TextMeshProUGUI ShopPlayerRessourcesValueText { get => shopPlayerRessourcesValueText; }
    public TextMeshProUGUI InventoryPlayerRessourcesValueText { get => inventoryPlayerRessourcesValueText; }

    public bool IsShopWindowOpen { get => isShopWindowOpen; set => isShopWindowOpen = value; }
    public Transform Player { get => player; private set => player = value; }
    public GameObject SteleTooltip { get => steleTooltip; }
    public ShopInformationPanel ShopInformationPanel { get => shopInformationPanel; }

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

            Player = transform.parent.transform;
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

            OnOpeningShopWindow();

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
    public void RepositionShopWindow(float yValue)
    {
        RectTransform shopWindowRectTransform = ShopWindow.GetComponent<RectTransform>();

        shopWindowRectTransform.position = new Vector3(shopWindowRectTransform.position.x, yValue, shopWindowRectTransform.position.z);
    }

    void OnOpeningShopWindow()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();

        foreach (ItemPanel itemPanel in shop.itemPanels)
        {
            StartCoroutine(itemPanel.EnableAllCanvasGroup());
        }

        minimapGameObject.SetActive(false);
        UIManager.Instance.waveIndicationUI.SetActive(false);

        cursorLogic.SetCursorToNormalAppearance();

        IsShopWindowOpen = true;
        UtilityClass.PlaySoundGroupImmediatly(openingShopSoundGroup, transform);

        ResetShopWindowAnchoredPosition();
        shop.RefreshShopData();
        ResetShopWindowSelectionsOnBoxes();


        HidePlayerFrame();

        for (int i = 0; i < shop.ShopBoxesIcon.Count; i++)
        {
            shop.ShopBoxesIcon[i].ToggleOff();
            StartCoroutine(shop.ShopBoxesIcon[i].EnableCanvasGroup());

            if (!shop.ShopBoxesIcon[i].Tooltip.activeInHierarchy) continue;

            shop.ShopBoxesIcon[i].HideTooltip(shop.ShopBoxesIcon[i].Tooltip);
        }

        GameManager.Instance.SetTimeScale(0);
    }

    void OnClosingShopWindow()
    {
        GameManager.Instance.SetTimeScale(1);

        ShopManager shop = ShopWindow.GetComponent<ShopManager>();

        for (int i = 0; i < shop.ShopBoxesIcon.Count; i++)
        {
            StartCoroutine(shop.ShopBoxesIcon[i].DisableCanvasGroup());
            shop.ShopBoxesIcon[i].IsSelected = false;
            shop.ShopBoxesIcon[i].ToggleOff();
        }

        minimapGameObject.SetActive(true);
        UIManager.Instance.waveIndicationUI.SetActive(true);

        DisplayPlayerFrame();

        StartCoroutine(ResetSelectedShopItem(shop));

        foreach (ItemPanel itemPanel in shop.itemPanels)
        {
            StartCoroutine(itemPanel.DisableAllCanvasGroup());
        }

        IsShopWindowOpen = false;

        if (!GameManager.Instance.tutorielDisplayed)
            UtilityClass.PlaySoundGroupImmediatly(closingShopSoundGroup, transform);
    }

    private IEnumerator ResetSelectedShopItem(ShopManager shop)
    {
        yield return new WaitForEndOfFrame();

        if (shop.ShopItemIsSelected)
        {
            ShopInformationPanel.HideContent();
            shop.ShopItemIsSelected = false;
            shop.SelectedItem = null;
        }
    }

    void ResetShopWindowSelectionsOnBoxes()
    {
        ShopManager shop = ShopWindow.GetComponent<ShopManager>();
        shop.PlayerInventory.ResetAllSelectedIcons();
    }

    void ResetShopWindowAnchoredPosition()
    {
        RepositionShopWindow(443);
    }
    #endregion


    public void ToggleCameraStateOnButtonClick()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

        CameraController cameraController = UtilityClass.GetMainCamera().GetComponent<CameraController>();

        cameraController.SetCameraLockState();
    }

    private void DisplayPlayerFrame()
    {
        CanvasGroup cG = playerFrame.GetComponent<CanvasGroup>();
        cG.blocksRaycasts = true;
        cG.alpha = 1;
    }

    private void HidePlayerFrame()
    {
        CanvasGroup cG = playerFrame.GetComponent<CanvasGroup>();
        cG.blocksRaycasts = false;
        cG.alpha = 0;
    }

    //Its on a button
    public void SetGameManagerToPlayModeOnClosingElement()
    {
        GameManager.Instance.SetGameToPlayMod();
    }
}