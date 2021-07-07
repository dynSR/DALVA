using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class ItemShuffleAnimationHandler : MonoBehaviour
{
    public ShopManager shopManager;
    public GameObject endOfDrawEffect;

    [SoundGroup] public string startShuffleSFX;
    [SoundGroup] public string endShuffleSFX;

    public Button resetButton;
    public Button [ ] closingShopButtons;
    public GameObject [ ] disabledButtonFeedbacks;

    private void Awake()
    {
        DisableResetButton();
    }

    public void ShopShuffleCall()
    {
        shopManager.ShuffleItemsInShop();
    }

    public void StartShuffleSoundCall()
    {
        if (PlayerHUDManager.Instance.IsShopWindowOpen)
            UtilityClass.PlaySoundGroupImmediatly(startShuffleSFX, transform);

        shopManager.isDoingADraw = true;
    }

    public void EndShuffleSoundCall()
    {
        if (PlayerHUDManager.Instance.IsShopWindowOpen)
            UtilityClass.PlaySoundGroupImmediatly(endShuffleSFX, transform);

        shopManager.isDoingADraw = false;
    }

    public void EnableResetButton()
    {
        resetButton.enabled = true;
    }

    public void DisableResetButton()
    {
        resetButton.enabled = false;
    }

    public void ActivateEndOfDrawEffect()
    {
        if (!endOfDrawEffect.activeInHierarchy)
            endOfDrawEffect.SetActive(true);
        else
        {
            endOfDrawEffect.SetActive(false);
            endOfDrawEffect.SetActive(true);
        }
    }

    public void DisableClosingShopButtons()
    {
        foreach (Button item in closingShopButtons)
        {
            item.enabled = false;
            item.GetComponent<UIButtonHighlight>().enabled = false;
        }

        foreach (GameObject item in disabledButtonFeedbacks)
        {
            item.SetActive(true);
        }
    }

    public void EnableClosingShopButtons ()
    {
        foreach (Button item in closingShopButtons)
        {
            item.enabled = true;
            item.GetComponent<UIButtonHighlight>().enabled = true;
        }

        foreach (GameObject item in disabledButtonFeedbacks)
        {
            item.SetActive(false);
        }
    }
}