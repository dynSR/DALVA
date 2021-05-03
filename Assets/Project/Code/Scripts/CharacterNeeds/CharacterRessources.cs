using TMPro;
using UnityEngine;

public class CharacterRessources : MonoBehaviour
{
    [Header("PLAYER SHOP AND INVENTORY")]
    [SerializeField] private ShopManager playerShop;
    [SerializeField] private InventoryManager playerInventory;

    [Header("PLAYER RESSOURCES")]
    [SerializeField] private int currentAmountOfPlayerRessources = 500;
    [SerializeField] private float passiveEarningDelay = 1f;
    [SerializeField] private float passiveRessourcesEarnOvertime = 5f;

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
        AddRessources((int)passiveRessourcesEarnOvertime);
    }

    public void AddRessources(int amountToAdd)
    {
        CurrentAmountOfPlayerRessources += amountToAdd;
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);

        if(PlayerHUD.IsShopWindowOpen)
            playerShop.RefreshShopData();

        if(PlayerHUD.SteleTooltip.activeInHierarchy)
            PlayerHUD.SteleTooltip.GetComponent<SteleTooltip>().SetCostTextColor();
    }

    public void RemoveRessources(int amountToRemove)
    {
        CurrentAmountOfPlayerRessources -= amountToRemove;
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);

        if (CurrentAmountOfPlayerRessources <= 0) CurrentAmountOfPlayerRessources = 0;

        if (PlayerHUD.IsShopWindowOpen)
            playerShop.RefreshShopData();

        if (PlayerHUD.SteleTooltip.activeInHierarchy)
            PlayerHUD.SteleTooltip.GetComponent<SteleTooltip>().SetCostTextColor();
    }

    private void UpdatePlayerRessourcesValueText(int value)
    {
        CurrentAmountOfPlayerRessources = value;
        ShopPlayerRessourcesValueText.text = value.ToString();
        InventoryPlayerRessourcesValueText.text = value.ToString();
    }
}