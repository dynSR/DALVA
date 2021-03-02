using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryBoxPointerDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryManager Inventory => GetComponentInParent<InventoryManager>();
    public InventoryManager PlayerInventory { get => Inventory; }

    private ToggleSelectionIcon BoxSelectionIcon => transform.GetChild(0).GetComponent<ToggleSelectionIcon>();

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer in " + gameObject.name);
        PlayerInventory.NewInventoryBox = gameObject;

        if (!BoxSelectionIcon.IsSelected)
            BoxSelectionIcon.DisplayIcon();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer out of " + gameObject.name);
        PlayerInventory.NewInventoryBox = null;

        if (!BoxSelectionIcon.IsSelected)
            BoxSelectionIcon.HideIcon();
    }
}
