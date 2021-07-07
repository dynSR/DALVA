using Cinemachine;
using DarkTonic.MasterAudio;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialHandler : MonoBehaviour
{
    [Header("COMPONENTS")]
    public Camera mainCameraBrain;
    public SpawnerSystem spawner;
    public GameObject skipTutorialButton;
    public Transform tutorialUIObject;
    [SerializeField] private GameObject[] windows;
    [SerializeField] private GameObject[] uiManagerComponentsToActivate;

    [Header("SETTINGS")]
    public bool skipTutorials = false;
    public bool harvesterTutorialHasBeenDone = false;
    public bool steleTutorialHasBeenDone = false;
    public bool enemyInteractionHasBeenExplained = false;

    #region Singleton
    public static TutorialHandler Instance;

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

    public void DeactivateGivenObject(GameObject gObj)
    {
        gObj.SetActive(false);
    }

    public void SetSkipTutorialToTrue()
    {
        skipTutorials = true;
    }

    public void OpenAWindow(int windowIndex)
    {
        if (!windows[windowIndex].activeInHierarchy)
            windows[windowIndex].SetActive(true);
    }

    public void CloseAWindow(int windowIndex)
    {
        if (windows[windowIndex].activeInHierarchy)
            windows[windowIndex].SetActive(false);
    }
    
    public void SetTutorielMod(int boolValue)
    {
        if (boolValue == 0)
        {
            GameManager.Instance.tutorielDisplayed = false;
        }
        else if (boolValue == 1)
        {
            GameManager.Instance.tutorielDisplayed = true;
        }
    }

    public void ReactivateUIManagerComponents()
    {
        foreach (GameObject item in uiManagerComponentsToActivate)
        {
            item.SetActive(true);
        }

        PlayerHUDManager.Instance.minimapGameObject.SetActive(true);
    }

    public IEnumerator DesactivateUIManagerComponents(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject item in uiManagerComponentsToActivate)
        {
            item.SetActive(false);
        }

        PlayerHUDManager.Instance.minimapGameObject.SetActive(false);
    }

    public void ActivateMainCamera()
    {
        UtilityClass.GetMainCamera().gameObject.tag = "Untagged";

        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.enabled = true;
        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.gameObject.tag = "MainCamera";

        UtilityClass.GetMainCamera().GetComponent<UniversalAdditionalCameraData>().renderType = CameraRenderType.Base;
    }

    public void DeactivateMainCamera()
    {
        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.enabled = false;
        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.gameObject.tag = "Untagged";

        mainCameraBrain.gameObject.tag = "MainCamera";
    }

    public void DisplayShop()
    {
        PlayerHUDManager.Instance.OpenWindow(PlayerHUDManager.Instance.ShopWindow);
        PlayerHUDManager.Instance.minimapGameObject.SetActive(false);
        UIManager.Instance.waveIndicationUI.SetActive(false);
    }

    public void SetCMVirtualPriorityValue(GameObject _object)
    {
        CinemachineVirtualCamera virtualCam = _object.GetComponent<CinemachineVirtualCamera>();
        virtualCam.Priority = 13;
    }

    public void ResetCMVirtualPriorityValue(GameObject _object)
    {
        CinemachineVirtualCamera virtualCam = _object.GetComponent<CinemachineVirtualCamera>();
        virtualCam.Priority = 9;
    }

    public void DisplayObject(GameObject _object)
    {
        _object.SetActive(true);
    }

    public void HideObject (GameObject _object)
    {
        _object.SetActive(false);
    }

    public void SetTutorials()
    {
        if (GameManager.Instance.tutorialsAreEnabled)
        {
            OpenAWindow(0);
            SetTutorielMod(1);
            DeactivateMainCamera();
            StartCoroutine(DesactivateUIManagerComponents(0.01f));
            skipTutorialButton.SetActive(true);
        }
    }

    public void CloseTutorialBySkippingIt()
    {
        SetTutorielMod(0);

        ActivateMainCamera();

        ReactivateUIManagerComponents();
        DisplayShop();

        SetSkipTutorialToTrue();

        EnablePHUDShopItemPanels();

        foreach (Transform item in tutorialUIObject)
        {
            item.gameObject.SetActive(false);
        }

        tutorialUIObject.gameObject.SetActive(false);
    }

    public void EnablePHUDShopItemPanels ()
    {
        PlayerHUDManager.Instance.EnableShopItemPanels();
    }

    public void DisablePHUDShopItemPanels()
    {
        PlayerHUDManager.Instance.DisableShopItemPanels();
    }
}