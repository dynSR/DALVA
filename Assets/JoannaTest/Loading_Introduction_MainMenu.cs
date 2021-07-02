using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Loading_Introduction_MainMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject UIComponent;

    private void OnEnable ()
    {
        videoPlayer.loopPointReached += HideVideoClip;
    }

    private void OnDisable ()
    {
        videoPlayer.loopPointReached -= HideVideoClip;
    }

    private void HideVideoClip(VideoPlayer vp)
    {
        gameObject.SetActive(false);
        UIComponent.SetActive(true);
    }
}
