using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ToggleSelectionIcon : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private OpenCloseShopWindow shopWindow;

    private Inventory PlayerInventory => GetComponentInParent<InventoryBox>().PlayerInventory;
    private Transform ToggleGameObject => transform.GetChild(0).transform;

    private bool isIconShown = false;
    private bool LeftClickIsPressed => Input.GetMouseButtonDown(0);

    void Awake()
    {
        ToggleGameObject.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (shopWindow.IsShopWindowOpen)
        {
            if (LeftClickIsPressed)
            {
                Debug.Log("Pointer Click");
                PlayerInventory.ResetSelectionIcons();
                ToggleAnIcon();
            }
        }
    }

    void ToggleAnIcon()
    {
        if (!isIconShown)
        {
            ToggleGameObject.gameObject.SetActive(true);
            isIconShown = true;
        }
        else if (isIconShown)
        {
            ToggleGameObject.gameObject.SetActive(false);
            isIconShown = false;
        }
    }

    public void HideIcon()
    {
        ToggleGameObject.gameObject.SetActive(false);
        isIconShown = false;
    }
}
