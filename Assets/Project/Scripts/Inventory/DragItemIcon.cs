using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas playerHUD;
    
    private Inventory PlayerInventory => transform.GetComponentInParent<InventoryBox>().PlayerInventory;
    private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
    private CanvasGroup ToggleImageCanvasGroup => GetComponentInParent<CanvasGroup>();
    public Sprite ItemIcon { get; set;}

    private bool LeftClickIsHeld => Input.GetMouseButton(0);
    
    void Awake()
    {
        ItemIcon = GetComponent<Image>().sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LeftClickIsHeld)
        {
            ToggleImageCanvasGroup.blocksRaycasts = false;

            PlayerInventory.LastInventoryBox = transform.parent.gameObject;
            CanvasGroup.blocksRaycasts = false;
        }  
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Item Icon");

        if (LeftClickIsHeld)
            transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        CanvasGroup.blocksRaycasts = true;

        ToggleImageCanvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
       if (PlayerInventory.NewInventoryBox != null)
       {
            if (PlayerInventory.NewInventoryBox.GetComponent<InventoryBox>().StoredItem != null)
                PlayerInventory.SwapInventoryBoxesItem();
            else
                PlayerInventory.PlaceItemHere();
       }

        PlayerInventory.LastInventoryBox = null;
    }
}
