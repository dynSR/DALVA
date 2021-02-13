using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ToggleSelectionIcon : MonoBehaviour, IPointerDownHandler
{
    [Header("PLAYER SHOP")]
    [SerializeField] private OpenCloseShopWindow shopWindow;
    private bool isSelected;

   private InventoryBox ParentInventoryBox => GetComponentInParent<InventoryBox>();
    private Inventory ParentPlayerInventory => ParentInventoryBox.PlayerInventory;
    private Item ItemInParentInventoryBox => ParentInventoryBox.StoredItem;
    private Transform ToggleChildGameObject => transform.GetChild(0).transform;

    private bool isIconShown = false;
    private bool LeftClickIsPressed => Input.GetMouseButtonDown(0);

    public bool IsSelected { get => isSelected; set => isSelected = value; }

    void Awake()
    {
        ToggleChildGameObject.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (shopWindow.IsShopWindowOpen)
        {
            if (LeftClickIsPressed && ItemInParentInventoryBox != null)
            {
                if (!IsSelected)
                {
                    Debug.Log("Pointer Click");
                    //ParentPlayerInventory.ResetSelectionIcons(); // Uncomment if you want to use the swap system
                    ToggleOn();
                }
                else if (IsSelected)
                {
                    ToggleOff();
                }
            }
        }
    }

    #region Toggle On / Off
    public void ToggleOn()
    {
        Debug.Log("ICON WASNT SHOWN");
        DisplayIcon();
        IsSelected = true;

        ParentPlayerInventory.Shop.SelectedInventoryBox = ParentInventoryBox;
    }

    public void ToggleOff()
    {
        Debug.Log("ICON WAS ALREADY SHOWN");
        HideIcon();
        IsSelected = false;

        ParentPlayerInventory.Shop.SelectedInventoryBox = null;
    }
    #endregion

    #region Display / Hide
    public void DisplayIcon()
    {
        ToggleChildGameObject.gameObject.SetActive(true);
        isIconShown = true;
    }

    public void HideIcon()
    {
        ToggleChildGameObject.gameObject.SetActive(false);
        isIconShown = false;
    }
    #endregion
}
