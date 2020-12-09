using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseShopWindow : MonoBehaviour
{
    [SerializeField] private KeyCode toggleWindowKey;
    [SerializeField] private bool isShopWindowOpen;

    private GameObject shopWindow => transform.GetChild(0).gameObject;

    public bool IsShopWindowOpen { get => isShopWindowOpen; set => isShopWindowOpen = value; }

    void Update()
    {
        ToggleWindow();
    }

    void ToggleWindow()
    {
        if (Input.GetKeyDown(toggleWindowKey))
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

    public void OpenWindow()
    {
        shopWindow.SetActive(true);
        IsShopWindowOpen = true;

        ResetWindowAnchoredPosition();
    }

    public void CloseWindow()
    {
        shopWindow.SetActive(false);
        IsShopWindowOpen = false;

        shopWindow.GetComponent<Shop>().PlayerInventory.ResetSelectionIcons();
    }

    void ResetWindowAnchoredPosition()
    {
        shopWindow.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
