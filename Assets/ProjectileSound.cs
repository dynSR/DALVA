using DarkTonic.MasterAudio;
using UnityEngine;

public class ProjectileSound : MonoBehaviour
{
    [Header("SFX TO PLAY")]
    [SoundGroup] public string sfxToPlayOnEnable;
    [SoundGroup] public string sfxToPlayOnTriggerEnter;

    [Header("SETTINGS")]
    public bool playSoundOnEnable = false;
    public bool playSoundOnTriggerEnter = false;

    private void OnEnable()
    {
        if (playSoundOnEnable)
            UtilityClass.PlaySoundGroupImmediatly(sfxToPlayOnEnable, transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playSoundOnTriggerEnter && other.gameObject.layer == 12)
        {
            UtilityClass.PlaySoundGroupImmediatly(sfxToPlayOnTriggerEnter, transform);
        }
    }
}
