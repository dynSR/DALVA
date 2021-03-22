using System.Collections.Generic;
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

    [SerializeField] private List<StatModifier> itemModifiers;

    private InventoryBox inventoryBox = null;
    
    public string ItemName { get => itemName; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public string ItemDescription { get => itemDescription; }
    public int ItemCost { get => itemCost; }
    public int AmountOfGoldRefundedOnSale { get => amountOfGoldRefundedOnSale; }
    public InventoryBox InventoryBox { get => inventoryBox; set => inventoryBox = value; }

    public void Equip(CharacterStat c)
    {
        for (int i = 0; i < c.CharacterStats.Count; i++)
        {
            for (int j = 0; j < itemModifiers.Count; j++)
            {
                if (c.CharacterStats[i]._StatType == itemModifiers[j].StatType)
                {
                    c.CharacterStats[i].AddModifier(new StatModifier(itemModifiers[j].Value, itemModifiers[j].StatType, itemModifiers[j].Type, this));
                }
            }
        }
    }

    public void Unequip(CharacterStat c)
    {
        Debug.Log("Casting Unequip");
        for (int i = c.CharacterStats.Count - 1; i >= 0; i--)
        {
            Debug.Log("Can't find any item -!-");
            c.CharacterStats[i].RemoveAllModifiersFromSource(this);
        }
    }
}