using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/ItemObject", order = 1)]
public class Item : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private Sprite itemIcon;

    public string ItemName { get => itemName; }
    public string ItemDescription { get => itemDescription; }
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }
    
    protected virtual void UseItem()
    {
        //Do whatever you want here
    }
}
