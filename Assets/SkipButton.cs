using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class SkipButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float timeToSkip = 3f;
    private float currentTimer = 0;
    private bool canUpdateTimer = false;
    public Image imageFeedback;

    private VideoPlayer ParentVideoPlayer => transform.parent.GetComponentInParent<VideoPlayer>();
    private Loading_Introduction_MainMenu Loading_Introduction_MainMenu => transform.parent.GetComponentInParent<Loading_Introduction_MainMenu>();

    void Update()
    {
        if (canUpdateTimer)
        {
            UpdateFeedback();

            if (currentTimer >= timeToSkip)
            {
                currentTimer = timeToSkip;

                ResetFeedback();

                ParentVideoPlayer.Stop();

                Loading_Introduction_MainMenu.UIComponent.SetActive(true);
                transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerDown (PointerEventData eventData)
    {
        canUpdateTimer = true;
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        ResetFeedback();
    }

    void UpdateFeedback()
    {
        imageFeedback.gameObject.SetActive(true);

        currentTimer += Time.deltaTime;

        imageFeedback.fillAmount = currentTimer / timeToSkip;
    }

    void ResetFeedback()
    {
        canUpdateTimer = false;

        imageFeedback.gameObject.SetActive(false);

        currentTimer = 0;
        imageFeedback.fillAmount = 0 / timeToSkip;
    }
}