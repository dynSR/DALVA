using TMPro;
using UnityEngine;

public class CharacterRessources : MonoBehaviour
{
    [Header("PLAYER SHOP")]
    [SerializeField] private InventoryManager playerInventory;

    [Header("PLAYER RESSOURCES")]
    [SerializeField] private int currentAmountOfPlayerRessources = 500;

    private TextMeshProUGUI ShopPlayerRessourcesValueText => GetComponentInChildren<PlayerHUDManager>().ShopPlayerRessourcesValueText;
    //private TextMeshProUGUI InventoryPlayerRessourcesValueText => GetComponentInChildren<PlayerHUD>().InventoryPlayerRessourcesValueText;
    public InventoryManager PlayerInventory { get => playerInventory; set => playerInventory = value; }
    public int CurrentAmountOfPlayerRessources { get => currentAmountOfPlayerRessources; set => currentAmountOfPlayerRessources = value; }

    private void OnEnable()
    {
        ShopManager.OnBuyingAnItem += UpdatePlayerRessourcesValueText;
        ShopManager.OnSellingAnItem += UpdatePlayerRessourcesValueText;
        ShopManager.OnShopActionCancel += UpdatePlayerRessourcesValueText;
    }

    private void OnDisable()
    {
        ShopManager.OnBuyingAnItem -= UpdatePlayerRessourcesValueText;
        ShopManager.OnSellingAnItem -= UpdatePlayerRessourcesValueText;
        ShopManager.OnShopActionCancel -= UpdatePlayerRessourcesValueText;
    }

    private void Start()
    {
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddRessources(500);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            RemoveRessources(250);
        }
    }

    private void AddRessources(int amountToAdd)
    {
        CurrentAmountOfPlayerRessources += amountToAdd;
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);
    }

    private void RemoveRessources(int amountToRemove)
    {
        CurrentAmountOfPlayerRessources -= amountToRemove;
        UpdatePlayerRessourcesValueText(CurrentAmountOfPlayerRessources);
    }

    private void UpdatePlayerRessourcesValueText(int value)
    {
        CurrentAmountOfPlayerRessources = value;
        ShopPlayerRessourcesValueText.text = value.ToString();
        //InventoryPlayerRessourcesValueText.text = value.ToString();
    }
}