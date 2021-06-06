using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObjects/Items", order = 0)]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [Multiline][SerializeField] private string itemDescription;
    [SerializeField] private int itemCost;
    [SerializeReference] private bool itemIsAnAbility = false;
    [SerializeReference] private bool itemIsInShop = false;

    [Header("ITEM AS AN ABILITY")]
    [SerializeField] private int abilityIndex = 0;
    [SerializeField] private AbilityEffect abilityEffectToAssign;

    [Header("ITEM AS AN EQUIPEMENT")]
    [SerializeField] private List<StatModifier> itemModifiers;

    private InventoryBox inventoryBox = null;
    
    public string ItemName { get => itemName; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    public string ItemDescription { get => itemDescription; }
    public int ItemCost { get => itemCost; }
    public InventoryBox InventoryBox { get => inventoryBox; set => inventoryBox = value; }
    public bool ItemIsAnAbility { get => itemIsAnAbility; }
    public AbilityEffect AbilityEffectToAssign { get => abilityEffectToAssign; }
    public int AbilityIndex { get => abilityIndex; }
    public bool ItemIsInShop { get => itemIsInShop; set => itemIsInShop = value; }

    #region Equipment
    public void EquipItemAsEquipement(EntityStats c)
    {
        if (!ItemIsAnAbility)
        {
            for (int i = 0; i < c.entityStats.Count; i++)
            {
                for (int j = 0; j < itemModifiers.Count; j++)
                {
                    if (c.entityStats[i].StatType == itemModifiers[j].StatType)
                    {
                        c.entityStats[i].AddModifier(new StatModifier(itemModifiers[j].Value, itemModifiers[j].StatType, itemModifiers[j].Type, this));

                        if (itemModifiers[j].StatType == StatType.MovementSpeed)
                            c.UpdateNavMeshAgentSpeed(StatType.MovementSpeed);

                        c.UpdateStats();
                    }
                }
            }
        }
    }

    public void UnequipItemAsEquipement(EntityStats c)
    {
        if (!ItemIsAnAbility)
        {
            for (int i = c.entityStats.Count - 1; i >= 0; i--)
            {
                Debug.Log("Can't find any item -!-");
                c.entityStats[i].RemoveAllModifiersFromSource(this);
                c.entityStats[i].MaxValue = c.entityStats[i].CalculateValue();

                for (int j = 0; j < itemModifiers.Count; j++)
                {
                    if (itemModifiers[j].StatType == StatType.MovementSpeed)
                        c.UpdateNavMeshAgentSpeed(StatType.MovementSpeed);
                }

                c.UpdateStats();
            }
        }   
    }
    #endregion

    #region Ability
    public void EquipItemAsAbility(AbilityLogic ability)
    {
        if (ItemIsAnAbility)
        {
            ability.UsedEffectIndex = AbilityEffectToAssign;
        }
    }

    public void UnequipItemAsAbility(int index,  EntityStats playerStats, AbilityEffect abilityEffect)
    {
        if (ItemIsAnAbility)
        {
            playerStats.EntityAbilities[index].UsedEffectIndex = abilityEffect;
        }
    }
    #endregion
}