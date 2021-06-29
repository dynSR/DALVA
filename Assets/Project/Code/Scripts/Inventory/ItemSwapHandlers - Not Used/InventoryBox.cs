using UnityEngine;
using UnityEngine.UI;

public class InventoryBox : MonoBehaviour
{
    public GameObject equipVFX;
    private int storedItemTransactionID;
    public InventoryManager PlayerInventory { get => GetComponentInParent<InventoryManager>(); }
    public Item StoredItem { get; set; }
    private Image StoredItemIcon => transform.GetChild(1).GetComponent<Image>();
    private CanvasGroup CanvasGrp => transform.GetChild(1).GetComponent<CanvasGroup>();
    public int StoredItemTransactionID { get => storedItemTransactionID; set => storedItemTransactionID = value; }

    private void Start()
    {
        CanvasGrp.alpha = 0;
    }

    public void ChangeInventoryBoxStoredItem(Item newItemToStore, Sprite newStoredItemIcon)
    {
        //Activate VFX on the box
        if (!equipVFX.activeInHierarchy)
        {
            equipVFX.SetActive(true);
        }
        else if (equipVFX.activeInHierarchy)
        {
            equipVFX.SetActive(false);
            equipVFX.SetActive(true);
        }

        StoredItem = newItemToStore;
        StoredItem.InventoryBox = this;

        CanvasGrp.alpha = 1;
        StoredItemIcon.sprite = newStoredItemIcon;
    }

    public void ResetInventoryBoxStoredItem(InventoryBox inventoryBoxToReset)
    {
        if (inventoryBoxToReset.StoredItem != null)
        {
            inventoryBoxToReset.StoredItem.InventoryBox = null;
            inventoryBoxToReset.StoredItem = null;

            inventoryBoxToReset.CanvasGrp.alpha = 0;
            inventoryBoxToReset.StoredItemIcon.sprite = null;
        }
    }
}
