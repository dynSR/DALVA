using System.Collections;
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
    [SerializeField] private TextMeshProUGUI selectedItemDescription02;

    [Header("CONTENTS")]
    [SerializeField] private GameObject firstPartContent;
    [SerializeField] private GameObject secondPartContent;

    public CanvasGroup CGroup => GetComponent<CanvasGroup>();

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
        shopManager.Player.GetComponent<CharacterRessources>().OnCharacterRessourcesChanged -= SetBuyButton;
    }

    private void Awake() => HideContent();

    void SetInformationPanel(Item selectedItem)
    {
        DisplayContent();

        SetContentInformations(selectedItem);

        buyButton.gameObject.SetActive(true);

        SetBuyButton();
    }

    void SetBuyButton()
    {
        Item selectedItem = shopManager.SelectedItem;
        UIButtonSound UIButtonSoundScript = buyButton.GetComponent<UIButtonSound>();

        if (shopManager.CanPurchaseItem(selectedItem) && !shopManager.IsItemAlreadyInInventory(selectedItem))
        {
            buyButton.interactable = true;

            buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().HighlightColor);

            UIButtonSoundScript.enabled = true;
        }
        else
        {
            buyButton.interactable = false;

            buyButton.GetComponent<UIButtonHighlight>().ChangeTextColor(buyButton.GetComponent<UIButtonHighlight>().NormalColor);

            UIButtonSoundScript.enabled = false;
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

        var pieces = selectedItem.ItemDescription.Split(new[] { '\n' }, 4, System.StringSplitOptions.None);

        if (selectedItem.ItemModifiers.Count == 1)
        {
            selectedItemDescription.text = pieces[0];
            selectedItemDescription02.text = string.Empty;
        }
        else if (selectedItem.ItemModifiers.Count == 2)
        {
            selectedItemDescription.text = pieces[0] + '\n' + pieces[1];
            selectedItemDescription02.text = string.Empty;
        }
        else if (selectedItem.ItemModifiers.Count == 3)
        {
            selectedItemDescription.text = pieces[0] + '\n' + pieces[1];
            selectedItemDescription02.text = pieces[2];
        }
        else if (selectedItem.ItemModifiers.Count == 4)
        {
            selectedItemDescription.text = pieces[0] + '\n' + pieces[1];
            selectedItemDescription02.text = pieces[2] + '\n' + pieces[3];
        }
    }

    void DisplayContent()
    {
        firstPartContent.SetActive(true);
        secondPartContent.SetActive(true);
    }

    public void HideContent()
    {   
        firstPartContent.SetActive(false);
        secondPartContent.SetActive(false);

        buyButton.gameObject.SetActive(false);
    }
    #endregion
}
