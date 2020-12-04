using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Canvas playerHUD;
    [SerializeField] private Inventory inventory;
    private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
    public Sprite ItemIcon { get; set;}

    void Awake()
    {
        ItemIcon = GetComponent<Image>().sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Set last Inventory Box
        inventory.lastInventoryBox = transform.parent.gameObject;
        CanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Item Icon");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        CanvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
       if (inventory.newInventoryBox != null)
       {
            inventory.SwapInventoryBoxItem();
       }

        inventory.lastInventoryBox = null;
    }
}
