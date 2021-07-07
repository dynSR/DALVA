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
        UtilityClass.PlaySoundGroupImmediatly(soundName, transform);
    }
}