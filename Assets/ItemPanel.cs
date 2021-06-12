using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPanel : MonoBehaviour
{
    public ShopManager shopManager;

    public List<Item> itemInGame;
    public List<ItemPanelButton> itemPanelComponentsCreated = new List<ItemPanelButton>();

    public GameObject itemPanelComponentsParentObject;
    public GameObject itemPanelComponentObject;

    [HideInInspector]
    public TooltipSetter tooltipToAttribute;

    private void OnEnable()
    {
        shopManager.OnShuffleDone += SetIfItemIsInShop;
        shopManager.OnShuffle += SetAllItemToUndisponableState;
    }

    private void OnDisable()
    {
        shopManager.OnShuffleDone -= SetIfItemIsInShop;
        shopManager.OnShuffle -= SetAllItemToUndisponableState;
    }

    private void Awake()
    {
        ResetItemParameters();
    }

    void Start()
    {
        itemInGame = itemInGame.OrderBy(itemInGame => itemInGame.ItemCost).ToList();
        InstantiateItemPanelComponents();
        SetIfItemIsInShop();
    }

    void InstantiateItemPanelComponents()
    {
        for (int i = 0; i < itemInGame.Count; i++)
        {
            GameObject itemPanelComponentInstance = Instantiate(itemPanelComponentObject, itemPanelComponentsParentObject.transform);

            ItemPanelButton itemPanelButton = itemPanelComponentInstance.GetComponentInChildren<ItemPanelButton>();

            if(!itemPanelComponentsCreated.Contains(itemPanelButton))
                itemPanelComponentsCreated.Add(itemPanelButton);

            itemPanelButton.tooltipSetter = tooltipToAttribute;

            itemPanelButton.AttributedItem = itemInGame[i];
            itemPanelButton.buttonImage.sprite = itemInGame[i].ItemIcon;
        }
    }

    public void SetIfItemIsInShop()
    {
        Debug.Log("SetIfItemIsInShop");
        Debug.Log(shopManager.itemCreated.Count);

        for (int i = 0; i < itemPanelComponentsCreated.Count; i++)
        {
            if (itemPanelComponentsCreated[i].AttributedItem.ItemIsInShop)
            {
                itemPanelComponentsCreated[i].ItemIsInShop();
                Debug.Log("Item is in shop " + itemPanelComponentsCreated[i].AttributedItem.ItemName + " " + itemPanelComponentsCreated[i].AttributedItem.ItemIsInShop);
            }
            else if (!itemPanelComponentsCreated[i].AttributedItem.ItemIsInShop)
            {
                itemPanelComponentsCreated[i].ItemIsntInShop();
                Debug.Log("Item is not in shop " + itemPanelComponentsCreated[i].AttributedItem.ItemName + " " + itemPanelComponentsCreated[i].AttributedItem.ItemIsInShop);
            }
        }
    }

    private void SetAllItemToUndisponableState()
    {
        for (int i = 0; i < itemPanelComponentsCreated.Count; i++)
        {
            itemPanelComponentsCreated[i].ItemIsntInShop();
        }
    }

    private void OnApplicationQuit()
    {
        ResetItemParameters();
    }

    void ResetItemParameters()
    {
        for (int i = 0; i < itemInGame.Count; i++)
        {
            if(itemInGame[i].ItemIsInShop)
            {
                itemInGame[i].ItemIsInShop = false;
            }
        }
    }

    public IEnumerator EnableAllCanvasGroup()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < itemPanelComponentsCreated.Count; i++)
        {
            itemPanelComponentsCreated[i].EnableCanvasGroup();
        }
    }

    public IEnumerator DisableAllCanvasGroup()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < itemPanelComponentsCreated.Count; i++)
        {
            itemPanelComponentsCreated[i].DisableCanvasGroup();
        }
    }
}