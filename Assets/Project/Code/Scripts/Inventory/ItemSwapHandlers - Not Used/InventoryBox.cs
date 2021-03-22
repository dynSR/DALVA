﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBox : MonoBehaviour
{
    private int storedItemTransactionID;
    public InventoryManager PlayerInventory { get => GetComponentInParent<InventoryManager>(); }
    public Item StoredItem { get; set; }
    private Image StoredItemIcon => transform.GetChild(0).GetComponent<Image>();
    private CanvasGroup CanvasGrp => transform.GetChild(0).GetComponent<CanvasGroup>();
    public int StoredItemTransactionID { get => storedItemTransactionID; set => storedItemTransactionID = value; }

    private void Start()
    {
        CanvasGrp.alpha = 0;
    }

    public void ChangeInventoryBoxStoredItem(Item newItemToStore, Sprite newStoredItemIcon)
    {
        StoredItem = newItemToStore;
        StoredItem.InventoryBox = this;

        CanvasGrp.alpha = 1;
        StoredItemIcon.sprite = newStoredItemIcon;
    }

    public void ResetInventoryBoxStoredItem(InventoryBox inventoryBoxToReset)
    {
        inventoryBoxToReset.StoredItem.InventoryBox = null;
        inventoryBoxToReset.StoredItem = null;

        inventoryBoxToReset.CanvasGrp.alpha = 0;
        inventoryBoxToReset.StoredItemIcon.sprite = null;
    }
}