using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryBoxPointerDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Inventory inventory;
    public Inventory Inventory { get => inventory; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer in " + gameObject.name);
        Inventory.NewInventoryBox = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer out of " + gameObject.name);
        Inventory.NewInventoryBox = null;
    }
}
