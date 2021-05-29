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
        //InvokeRepeating(nameof(AddRessourcesOvertime), passiveEarningDelay, 1);
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
        if (CurrentAmountOfPlayerRessources <= 0) CurrentAmountOfPlayerRessources = 0;

        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);

        if (PlayerHUD.IsShopWindowOpen)
            playerShop.RefreshShopData();

        OnCharacterRessourcesChanged?.Invoke();

        //if (PlayerHUD.SteleTooltip.activeInHierarchy)
        //    PlayerHUD.SteleTooltip.GetComponent<SteleTooltip>().SetCostTextColor();
    }

    private void UpdatePlayerRessourcesValueText(int value)
    {
        //Debug.Log("! UpdatePlayerRessourcesValueText !");
        //Debug.Log("! VALUE ! : " + value);
        //Means that a purchase has been done
        SetRessourcesFeedback(value);

        CurrentAmountOfPlayerRessources = value;
        OnCharacterRessourcesChanged?.Invoke();
        ShopPlayerRessourcesValueText.text = value.ToString();
        InventoryPlayerRessourcesValueText.text = value.ToString();
    }

    public void SetRessourcesFeedback(int newValue)
    {
        //Maybe add the logic for when we add gold to inventory
        // --> Changing color / "+" or "-"
        //Maybe play the animation in reverse to add contrast : in =/= out

        //Debug.Log("!!! SetRessourcesFeedback !!!");

        //PlayerHUD -----------------------------------------------------------------------------------------------------------------
        TextMeshProUGUI playerHUDTextToSetup = ressourcesLossFeedbackPlayerHUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Animator playerHUDAnimator = ressourcesLossFeedbackPlayerHUD.GetComponent<Animator>();
        //UIManager -----------------------------------------------------------------------------------------------------------------
        TextMeshProUGUI UIManagerTextToSetup = ressourcesLossFeedbackUIManager.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Animator UIManagerAnimator = ressourcesLossFeedbackUIManager.GetComponent<Animator>();

        if (CurrentAmountOfPlayerRessources > newValue)
        {
            //Debug.Log("!!! SetRessourcesFeedback - CurrentAmountOfPlayerRessources > newValue !!!");

            //PlayerHUD ------------------------------------------------------------------------------
            playerHUDTextToSetup.color = Color.red;
            playerHUDTextToSetup.text = (newValue - CurrentAmountOfPlayerRessources).ToString();
            playerHUDAnimator.SetTrigger("TriggerFeedback");

            //UIManager ------------------------------------------------------------------------------
            UIManagerTextToSetup.color = Color.red;
            UIManagerTextToSetup.text = (newValue - CurrentAmountOfPlayerRessources).ToString();
            UIManagerAnimator.SetTrigger("TriggerFeedback");
        }
        else if (CurrentAmountOfPlayerRessources < newValue)
        {
            //Debug.Log("!!! SetRessourcesFeedback - CurrentAmountOfPlayerRessources > newValue !!!");

            //PlayerHUD ------------------------------------------------------------------------------
            playerHUDTextToSetup.color = Color.green;
            playerHUDTextToSetup.text = "+" + (newValue - CurrentAmountOfPlayerRessources).ToString();
            playerHUDAnimator.SetTrigger("TriggerFeedbackOut");

            //UIManager ------------------------------------------------------------------------------
            UIManagerTextToSetup.color = Color.green;
            UIManagerTextToSetup.text = "+" + (newValue - CurrentAmountOfPlayerRessources).ToString();
            UIManagerAnimator.SetTrigger("TriggerFeedbackOut");
        }
    }

    public void ResetPlayerHUDFeedbackAnimator()
    {
        ressourcesLossFeedbackPlayerHUD.GetComponent<Animator>().ResetTrigger("TriggerFeedback");
    }
}