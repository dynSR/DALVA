using TMPro;
using UnityEngine;

public class CharacterRessources : MonoBehaviour
{
    [Header("PLAYER SHOP")]
    [SerializeField] private InventoryManager playerInventory;

    [Header("PLAYER RESSOURCES")]
    [SerializeField] private int amountOfPlayerRessources = 500;

    private TextMeshProUGUI ShopPlayerRessourcesValueText => GetComponentInChildren<PlayerHUDManager>().ShopPlayerRessourcesValueText;
    //private TextMeshProUGUI InventoryPlayerRessourcesValueText => GetComponentInChildren<PlayerHUD>().InventoryPlayerRessourcesValueText;
    public InventoryManager PlayerInventory { get => playerInventory; set => playerInventory = value; }
    public int AmountOfPlayerRessources { get => amountOfPlayerRessources; set => amountOfPlayerRessources = value; }

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
        UpdatePlayerRessourcesValueText(AmountOfPlayerRessources);
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
        AmountOfPlayerRessources += amountToAdd;
        UpdatePlayerRessourcesValueText(AmountOfPlayerRessources);
    }

    private void RemoveRessources(int amountToRemove)
    {
        AmountOfPlayerRessources -= amountToRemove;
        UpdatePlayerRessourcesValueText(AmountOfPlayerRessources);
    }

    private void UpdatePlayerRessourcesValueText(int value)
    {
        AmountOfPlayerRessources = value;
        ShopPlayerRessourcesValueText.text = value.ToString();
        //InventoryPlayerRessourcesValueText.text = value.ToString();
    }
}