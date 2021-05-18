using TMPro;
using UnityEngine;

public class CharacterRessources : MonoBehaviour
{
    public delegate void CharacterRessourcesHandler();
    public event CharacterRessourcesHandler OnCharacterRessourcesChanged;

    [Header("PLAYER SHOP AND INVENTORY")]
    [SerializeField] private ShopManager playerShop;
    [SerializeField] private InventoryManager playerInventory;

    [Header("PLAYER RESSOURCES")]
    [SerializeField] private int currentAmountOfPlayerRessources = 500;
    [SerializeField] private float passiveEarningDelay = 1f;
    [SerializeField] private float passiveRessourcesEarnOvertime = 5f;

    [Header("PLAYER RESSOURCES FEEDBACK TEXTS")]
    [SerializeField] private GameObject ressourcesLossFeedbackPlayerHUD;
    [SerializeField] private GameObject ressourcesLossFeedbackUIManager;

    private PlayerHUDManager PlayerHUD => GetComponentInChildren<PlayerHUDManager>();
    private TextMeshProUGUI ShopPlayerRessourcesValueText => PlayerHUD.ShopPlayerRessourcesValueText;
    private TextMeshProUGUI InventoryPlayerRessourcesValueText => PlayerHUD.InventoryPlayerRessourcesValueText;
    public InventoryManager PlayerInventory { get => playerInventory; set => playerInventory = value; }
    public int CurrentAmountOfPlayerRessources { get => currentAmountOfPlayerRessources; set => currentAmountOfPlayerRessources = value; }

    private void OnEnable()
    {
        playerShop.OnBuyingAnItem += UpdatePlayerRessourcesValueText;
        playerShop.OnSellingAnItem += UpdatePlayerRessourcesValueText;
        playerShop.OnShopActionCancel += UpdatePlayerRessourcesValueText;
    }

    private void OnDisable()
    {
        playerShop.OnBuyingAnItem -= UpdatePlayerRessourcesValueText;
        playerShop.OnSellingAnItem -= UpdatePlayerRessourcesValueText;
        playerShop.OnShopActionCancel -= UpdatePlayerRessourcesValueText;
    }

    private void Start()
    {
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);
        InvokeRepeating(nameof(AddRessourcesOvertime), passiveEarningDelay, 1);
    }

    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddRessources(500);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            RemoveRessources(250);
        }
    }

    private void AddRessourcesOvertime()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;
        
        AddRessources((int)passiveRessourcesEarnOvertime);
    }

    public void AddRessources(int amountToAdd)
    {
        CurrentAmountOfPlayerRessources += amountToAdd;
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);

        if(PlayerHUD.IsShopWindowOpen)
            playerShop.RefreshShopData();

        //if(PlayerHUD.SteleTooltip.activeInHierarchy)
        //    PlayerHUD.SteleTooltip.GetComponent<SteleTooltip>().SetCostTextColor();

        OnCharacterRessourcesChanged?.Invoke();
    }

    public void RemoveRessources(int amountToRemove)
    {
        CurrentAmountOfPlayerRessources -= amountToRemove;
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);

        if (CurrentAmountOfPlayerRessources <= 0) CurrentAmountOfPlayerRessources = 0;

        if (PlayerHUD.IsShopWindowOpen)
            playerShop.RefreshShopData();

        OnCharacterRessourcesChanged?.Invoke();

        //if (PlayerHUD.SteleTooltip.activeInHierarchy)
        //    PlayerHUD.SteleTooltip.GetComponent<SteleTooltip>().SetCostTextColor();
    }

    private void UpdatePlayerRessourcesValueText(int value)
    {
        //Means that a purchase has been done
        if(CurrentAmountOfPlayerRessources > value)
            SetLossRessourcesFeedback(value);

        CurrentAmountOfPlayerRessources = value;
        OnCharacterRessourcesChanged?.Invoke();
        ShopPlayerRessourcesValueText.text = value.ToString();
        InventoryPlayerRessourcesValueText.text = value.ToString();
    }

    private void SetLossRessourcesFeedback(int newValue)
    {
        //Maybe add the logic for when we add gold to inventory
        // --> Changing color / "+" or "-"
        //Maybe play the animation in reverse to add contrast : in =/= out

        //PlayerHUD ------------------------------------------------------------------------------
        TextMeshProUGUI playerHUDTextToSetup = ressourcesLossFeedbackPlayerHUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        playerHUDTextToSetup.text = (newValue - CurrentAmountOfPlayerRessources).ToString();

        Animator playerHUDAnimator = ressourcesLossFeedbackPlayerHUD.GetComponent<Animator>();
        playerHUDAnimator.SetTrigger("TriggerFeedback");

        //UIManager ------------------------------------------------------------------------------
        TextMeshProUGUI UIManagerTextToSetup = ressourcesLossFeedbackUIManager.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        UIManagerTextToSetup.text = (newValue - CurrentAmountOfPlayerRessources).ToString();

        Animator UIManagerAnimator = ressourcesLossFeedbackUIManager.GetComponent<Animator>();
        UIManagerAnimator.SetTrigger("TriggerFeedback");

    }
}