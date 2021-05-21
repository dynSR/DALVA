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

    void Start() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(playSoundOnClick)
            UtilityClass.PlaySoundGroupImmediatly(onClickSound, transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(playSoundOnHover)
            UtilityClass.PlaySoundGroupImmediatly(onHoverSound, transform);
    }
}