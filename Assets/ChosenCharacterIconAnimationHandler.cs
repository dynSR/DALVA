using DarkTonic.MasterAudio;
using UnityEngine;

public class ChosenCharacterIconAnimationHandler : MonoBehaviour
{
    public void DisplayUIManagerComponent()
    {
        foreach (GameObject gO in UIManager.Instance.endScreenComponents)
        {
            gO.SetActive(true);
        }
    }

    public void FadeGameStateProperSFX(string soundName)
    {
        MasterAudio.FadeSoundGroupToVolume(soundName, 1, 1.5f, null, true, true);
    }
}