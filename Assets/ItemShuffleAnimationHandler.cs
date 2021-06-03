using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class ItemShuffleAnimationHandler : MonoBehaviour
{
    public ShopManager shopManager;

    [SoundGroup] public string startShuffleSFX;
    [SoundGroup] public string endShuffleSFX;

    public Button resetButton;

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
}