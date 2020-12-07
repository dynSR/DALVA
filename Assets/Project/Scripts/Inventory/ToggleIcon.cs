using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ToggleIcon : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Shop shop;
    private InventoryBox InventoryBox => GetComponentInParent<InventoryBox>();
    private CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();

    private bool isIconShown = false;
    private bool LeftClickIsPressed => Input.GetMouseButtonDown(0);
    private bool CanToggleIcon => LeftClickIsPressed && shop.IsShopWindowOpened && InventoryBox.StoredItem != null;

    void Awake()
    {
        CanvasGroup.alpha = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanToggleIcon)
        {
            Debug.Log("Pointer Click");

            if (!isIconShown)
            {
                CanvasGroup.alpha = 1f;
                isIconShown = true;
            }
            else if (isIconShown)
            {
                CanvasGroup.alpha = 0f;
                isIconShown = false;
            }
        }
    }
}
