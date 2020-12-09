using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryBoxPointerDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Inventory Inventory => GetComponentInParent<Inventory>();
    public Inventory PlayerInventory { get => Inventory; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer in " + gameObject.name);
        PlayerInventory.NewInventoryBox = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer out of " + gameObject.name);
        PlayerInventory.NewInventoryBox = null;
    }
}
