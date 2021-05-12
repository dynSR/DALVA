using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInformationPanel : MonoBehaviour
{
    [Header("SHOP")]
    [SerializeField] private ShopManager shopManager;

    [Header("BUTTON TO BUY")]
    [SerializeField] private Button buyButton;

    [Header("SELECTED ITEM ATTRIBUTES")]
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemCost;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;

    [Header("CONTENTS")]
    [SerializeField] private GameObject firstPartContent;
    [SerializeField] private GameObject secondPartContent;

    private void OnEnable()
    {
        ShopIcon.OnSelectingAnItem += SetInformationPanel;
        ShopIcon.OnDeselectingAnItem += ResetInformationPanel;
        shopManager.Player.GetComponent<CharacterRessources>().OnCharacterRessourcesChanged += SetBuyButton;
    }

    private void OnDisable()
    {
        ShopIcon.OnSelectingAnItem -= SetInformationPanel;
        ShopIcon.OnDeselectingAnItem -= ResetInformationPanel;
    }

    private void Awake() => HideContent();

    void SetInformationPanel(Item selectedItem)
    {
        DisplayContent();

        SetContentInformations(selectedItem);

        buyButton.gameObject.SetActive(true);

        SetBuyButton();

        //if (shopManager.CanPurchaseItem(selectedItem) && !shopManager.IsItemAlreadyInInventory(selectedItem))
        //{
        //    buyButton.interactable = true;
        //    buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().HighlightColor);
        //}
        //else buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().NormalColor);
    }

    void SetBuyButton()
    {
        Item selectedItem = shopManager.SelectedItem;

        if (shopManager.CanPurchaseItem(selectedItem) && !shopManager.IsItemAlreadyInInventory(selectedItem))
        {
            buyButton.interactable = true;
            buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().HighlightColor);
        }
        else
        {
            buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().NormalColor);
            buyButton.interactable = false; 
        }
    }

    void ResetInformationPanel(Item selectedItem)
    {
        if (buyButton.gameObject.activeInHierarchy)
            buyButton.gameObject.SetActive(false);

        buyButton.interactable = false;
        buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().NormalColor);
        HideContent();
    }

    #region Setup
    void SetContentInformations(Item selectedItem)
    {
        selectedItemName.text = selectedItem.ItemName;
        selectedItemCost.text = selectedItem.ItemCost.ToString();
        selectedItemDescription.text = selectedItem.ItemDescription;
    }

    void DisplayContent()
    {
        firstPartContent.SetActive(true);
        secondPartContent.SetActive(true);
    }

    void HideContent()
    {
        firstPartContent.SetActive(false);
        secondPartContent.SetActive(false);
    }
    #endregion
}
