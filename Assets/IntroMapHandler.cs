using UnityEngine;
using Cinemachine;
using System.Collections;

public class IntroMapHandler : MonoBehaviour
{
    private CinemachineVirtualCamera CinemachineVirtualCamera => GetComponentInChildren<CinemachineVirtualCamera>();
    private Animator MyAnimator => GetComponent<Animator>();

    #region Singleton
    public static IntroMapHandler Instance;

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

    public void CallDisplayEntireMapMethod()
    {
        StartCoroutine(DisplayEntireMap());
    }

    IEnumerator DisplayEntireMap()
    {
        GameManager.Instance.SetGameToStandbyMod();

        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();

        UtilityClass.GetMainCamera().gameObject.SetActive(false);
        CinemachineVirtualCamera.Priority = 11;

        yield return new WaitForSeconds((cinemachineBrain.m_DefaultBlend.BlendTime * 1.25f));

        MyAnimator.SetTrigger("Intro");
    }
}
