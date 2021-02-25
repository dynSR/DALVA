using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObjects/Items", order = 1)]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    public int itemCost;
    public int amountOfGoldRefundedOnSale;
    [SerializeField] private Sprite itemIcon;

    private InventoryBox inventoryBox = null;
    
    public string ItemName { get => itemName; }
    public string ItemDescription { get => itemDescription; }
    public int ItemCost { get => itemCost; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public InventoryBox InventoryBox { get => inventoryBox; set => inventoryBox = value; }
    public int AmountOfGoldRefundedOnSale { get => amountOfGoldRefundedOnSale; }

}