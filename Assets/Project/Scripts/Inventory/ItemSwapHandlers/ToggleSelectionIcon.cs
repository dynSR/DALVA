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
            if (UtilityClass.LeftClickIsPressedOnUIElement(eventData) && ItemInParentInventoryBox != null)
            {
                if (!IsSelected)
                {
                    Debug.Log("Pointer Click");
                    ParentPlayerInventory.ResetAllBoxesSelectionIcons();
                    ToggleOn();
                }
                else if (IsSelected)
                {
                    ToggleOff();
                }
            }
        }
    }

    #region Toggle On / Off Selection
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

    #region Display / Hide Icon
    public void DisplayIcon()
    {
        ToggleChildGameObject.gameObject.SetActive(true);
    }

    public void HideIcon()
    {
        ToggleChildGameObject.gameObject.SetActive(false);
    }
    #endregion
}