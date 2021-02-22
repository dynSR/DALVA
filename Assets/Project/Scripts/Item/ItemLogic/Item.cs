using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/ItemObject", order = 1)]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private int itemCost;
    [SerializeField] private int amountOfGoldRefundedOnSale;
    [SerializeField] private Sprite itemIcon;

    public InventoryBox inventoryBox = null;
    

    public string ItemName { get => itemName; }
    public string ItemDescription { get => itemDescription; }
    public int ItemCost { get => itemCost; }

    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public InventoryBox InventoryBox { get => inventoryBox; set => inventoryBox = value; }
    

    protected virtual void UseItem()
    {
        //Do whatever you want here
    }
}
