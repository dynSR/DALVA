using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseShopWindow : MonoBehaviour
{
    [Header("KEY TO OPEN/CLOSE THE SHOP")]
    [SerializeField] private KeyCode toggleInputKey;
    private bool isShopWindowOpen;

    private GameObject ShopWindow => transform.GetChild(0).gameObject;

    public bool IsShopWindowOpen { get => isShopWindowOpen; set => isShopWindowOpen = value; }

    void Update()
    {
        ToggleWindow();
    }

    #region Toggle
    void ToggleWindow()
    {
        if (Input.GetKeyDown(toggleInputKey))
        {
            if (!IsShopWindowOpen)
            {
                OpenWindow();
            }
            else if (IsShopWindowOpen)
            {
                CloseWindow();
            }
        }
    }
    #endregion

    #region Open - Close
    public void OpenWindow()
    {
        ShopWindow.SetActive(true);
        IsShopWindowOpen = true;

        ResetWindowAnchoredPosition();
    }

    public void CloseWindow()
    {
        ShopWindow.SetActive(false);
        IsShopWindowOpen = false;

        ShopWindow.GetComponent<Shop>().PlayerInventory.ResetAllBoxesSelectionIcons();
    }
    #endregion

    void ResetWindowAnchoredPosition()
    {
        ShopWindow.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
