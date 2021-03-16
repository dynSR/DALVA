using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObjects/Items", order = 0)]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private string itemDescription;
    [SerializeField] private int itemCost;
    [SerializeField] private int amountOfGoldRefundedOnSale;

    [SerializeField] private Item itemFirstEvolution;
    [SerializeField] private Item itemSecondEvolution;


    private InventoryBox inventoryBox = null;
    
    public string ItemName { get => itemName; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public string ItemDescription { get => itemDescription; }
    public int ItemCost { get => itemCost; }
    public int AmountOfGoldRefundedOnSale { get => amountOfGoldRefundedOnSale; }
    public InventoryBox InventoryBox { get => inventoryBox; set => inventoryBox = value; }
   

    
}