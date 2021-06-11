using DarkTonic.MasterAudio;
using UnityEngine;

public class SoundTravelAcrossScene : MonoBehaviour
{
    public Transform[] emittersPosition;
    [SoundGroup] public string sfxToPlay;

    public bool eventHasBeenHandled = false;
    public bool delayCanDecrease = false;
    public float delay = 3.5f;

    void Start()
    {
        DeactivateMovingObject();
    }

    void Update()
    {
        if (GameManager.Instance.ItIsABossWave && !eventHasBeenHandled)
        {
            ActivateMovingObject();
            delayCanDecrease = true;
            eventHasBeenHandled = true;
        }

        if (delayCanDecrease)
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
            {
                DeactivateMovingObject();
                delayCanDecrease = false;
            }
        }

        if (!GameManager.Instance.ItIsABossWave && eventHasBeenHandled)
        {
            eventHasBeenHandled = false;
            delay = 3.5f;
        }
    }

    void PlaySFX()
    {
        foreach (Transform item in emittersPosition)
        {
            UtilityClass.PlaySoundGroupImmediatly(sfxToPlay, item);
        }
    }

    void ActivateMovingObject()
    {
        foreach (Transform item in emittersPosition)
        {
            item.gameObject.SetActive(true);
        }

        PlaySFX();
    }

    void DeactivateMovingObject()
    {
        foreach (Transform item in emittersPosition)
        {
            item.gameObject.SetActive(false);
        }

        MasterAudio.FadeOutAllOfSound(sfxToPlay, 0.2f);
    }
}