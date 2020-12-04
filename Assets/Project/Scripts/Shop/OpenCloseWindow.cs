using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseWindow : MonoBehaviour
{
    [SerializeField] private KeyCode toggleShopWindowKey;
    private GameObject ShopWindowGameObject => transform.GetChild(0).gameObject;

    void Update()
    {
        ToggleShwopWindow();
    }

    void ToggleShwopWindow()
    {
        if (Input.GetKeyDown(toggleShopWindowKey))
        {
            ShopWindowGameObject.SetActive(!ShopWindowGameObject.activeSelf);
            ResetShopWindowAnchoredPosition();
        }
    }

    public void OpenShopWindow()
    {
        ShopWindowGameObject.SetActive(true);
        ResetShopWindowAnchoredPosition();
    }

    public void CloseShopWindow()
    {
        ShopWindowGameObject.SetActive(false);
    }

    void ResetShopWindowAnchoredPosition()
    {
        ShopWindowGameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
