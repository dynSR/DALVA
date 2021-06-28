using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class IntroMapHandler : MonoBehaviour
{
    private CinemachineVirtualCamera CinemachineVirtualCamera => GetComponentInChildren<CinemachineVirtualCamera>();
    private Animator MyAnimator => GetComponent<Animator>();

    public Outline [ ] outlines;
    public List<Color> colors;

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

            foreach (var color in outlines)
            {
                colors.Add(color.OutlineColor);
            }
        }
    }
    #endregion

    public void CallDisplayEntireMapMethod()
    {
        StartCoroutine(DisplayEntireMap());
    }

    IEnumerator DisplayEntireMap()
    {
        yield return new WaitForEndOfFrame();

        UIManager.Instance.gameObject.SetActive(false);
        PlayerHUDManager.Instance.gameObject.SetActive(false);

        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();

        float timer = cinemachineBrain.m_DefaultBlend.BlendTime;

        UtilityClass.GetMainCamera().transform.GetChild(2).gameObject.SetActive(false);

        CinemachineVirtualCamera.Priority = 12;

        Debug.Log(cinemachineBrain.name);
        Debug.Log(cinemachineBrain.m_DefaultBlend.BlendTime);

        yield return new WaitForSeconds(timer * 0.75f);

        MyAnimator.SetTrigger("Intro");
    }

    public void CallFocusOnCharacterCoroutine()
    {
        StartCoroutine(FocusOnCharacter());
    }

    private IEnumerator FocusOnCharacter()
    {
        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        float timer = cinemachineBrain.m_DefaultBlend.BlendTime;

        CinemachineVirtualCamera.Priority = 9;
        UtilityClass.GetMainCamera().GetComponent<CinemachineVirtualCamera>().Priority = 12;

        yield return new WaitForSeconds(timer);

        UIManager.Instance.gameObject.SetActive(true);
        PlayerHUDManager.Instance.gameObject.SetActive(true);

        UtilityClass.GetMainCamera().GetComponent<UniversalAdditionalCameraData>().renderType = CameraRenderType.Base;

        UtilityClass.GetMainCamera().transform.GetChild(2).gameObject.SetActive(true);

        GameManager.Instance.ShopPhase();
    }

    public void SetOutlinesColorToWhite ()
    {
        foreach (var color in outlines)
        {
            color.OutlineColor = Color.white;
        }
    }

    public void SetOutlinesDefaultColor ()
    {
        for (int i = 0; i < colors.Count; i++)
        {
            outlines [ i ].OutlineColor = colors [ i ];
        }
    }
}