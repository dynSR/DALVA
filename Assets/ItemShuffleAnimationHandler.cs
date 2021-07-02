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
        UtilityClass.PlaySoundGroupImmediatly(startShuffleSFX, transform);
    }

    public void EndShuffleSoundCall()
    {
        UtilityClass.PlaySoundGroupImmediatly(endShuffleSFX, transform);
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
        }
    }

    public void EnableClosingShopButtons ()
    {
        foreach (Button item in closingShopButtons)
        {
            item.enabled = true;
        }
    }
}