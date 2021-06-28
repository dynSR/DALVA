using UnityEngine;

public class InGameSceneTransitionManager : MonoBehaviour
{
    private Animator Animator => GetComponent<Animator>();

    #region Singleton
    public static InGameSceneTransitionManager Instance;

    private void Awake ()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private void Start ()
    {
        TriggerFadeOut();
    }

    public void TriggerFadeIn()
    {
        Animator.SetTrigger("FadeIn");
    }

    public void TriggerFadeOut()
    {
        Debug.Log("TriggerFadeOut");
        Animator.SetTrigger("FadeOut");
    }

    //Animation Event
    public void DisplayEntireMapAnimationEvent()
    {
        IntroMapHandler.Instance.CallDisplayEntireMapMethod();
    }
}