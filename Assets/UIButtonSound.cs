using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [Header("UI BUTTON SOUNDS")]
    [SoundGroup][SerializeField] private string onHoverSound;
    [SoundGroup][SerializeField] private string onClickSound;

    [Header("SETTINGS")]
    [SerializeField] private bool playSoundOnHover = true;
    [SerializeField] private bool playSoundOnClick = true;

    public bool PlaySoundOnClick { get => playSoundOnClick; set => playSoundOnClick = value; }
    public bool PlaySoundOnHover { get => playSoundOnHover; set => playSoundOnHover = value; }

    void Start() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(PlaySoundOnClick)
            UtilityClass.PlaySoundGroupImmediatly(onClickSound, transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(PlaySoundOnHover)
            UtilityClass.PlaySoundGroupImmediatly(onHoverSound, transform);
    }
}