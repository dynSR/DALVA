using Cinemachine;
using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] windows;
    [SerializeField] private GameObject[] uiManagerComponentsToActivate;
    public Camera mainCameraBrain;
    private bool firstSpawnExplained = false;
    private bool bossWaveExplained = false;
    public GameObject enemyPortalPosObj;
    [SoundGroup] [SerializeField] private string portalLoopSFX;
    public bool skipTutorials = false;
    public SpawnerSystem spawner;
    public GameObject skipTutorialButton;

    private void OnEnable()
    {
        spawner.OnFirstWaveSpawned += DisplayWaveTutorial;
        spawner.OnFirstBossWaveSpawned += DisplayBossTutorial;
    }

    private void OnDisable()
    {
        spawner.OnFirstWaveSpawned -= DisplayWaveTutorial;
        spawner.OnFirstBossWaveSpawned -= DisplayBossTutorial;
    }

    private void Start()
    {
        if (GameManager.Instance.mapIsEasy)
        {
            OpenAWindow(0);
            SetTutorielMod(1);
            DeactivateMainCamera();
            StartCoroutine(DesactivateUIManagerComponents(0.01f));
            skipTutorialButton.SetActive(true);
        }
    }

    private void DisplayWaveTutorial()
    {
        if (!firstSpawnExplained && !skipTutorials)
        {
            GameManager.Instance.SetGameToTutorialMod();

            firstSpawnExplained = true;
            OpenAWindow(7);

            SetCMVirtualPriorityValue(enemyPortalPosObj);

            GameManager.Instance.SetGameToStandbyMod();

            GameManager.Instance.tutorielDisplayed = true;

            DeactivateMainCamera();
            Time.timeScale = 0;

            MasterAudio.FadeSoundGroupToVolume(portalLoopSFX, 0, 0.15f, null, true);
        }
    }

    private void DisplayBossTutorial()
    {
        if (!bossWaveExplained && !skipTutorials)
        {
            GameManager.Instance.SetGameToTutorialMod();

            bossWaveExplained = true;
            OpenAWindow(8);

            SetCMVirtualPriorityValue(enemyPortalPosObj);

            GameManager.Instance.SetGameToStandbyMod();

            GameManager.Instance.tutorielDisplayed = true;

            DeactivateMainCamera();
            Time.timeScale = 0;

            MasterAudio.PauseSoundGroup(portalLoopSFX);
        }
    }

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
    }

    public IEnumerator DesactivateUIManagerComponents(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject item in uiManagerComponentsToActivate)
        {
            item.SetActive(false);
        }
    }

    public void ActivateMainCamera()
    {
        UtilityClass.GetMainCamera().gameObject.tag = "Untagged";

        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.enabled = true;
        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.gameObject.tag = "MainCamera";
    }

    public void DeactivateMainCamera()
    {
        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.enabled = false;
        GameManager.Instance.Player.GetComponent<PlayerController>().CharacterCamera.gameObject.tag = "Untagged";

        mainCameraBrain.gameObject.tag = "MainCamera";
    }

    public void DisplayShop()
    {
        GameManager.Instance.ShopPhase();
    }

    public void SetCMVirtualPriorityValue(GameObject _object)
    {
        CinemachineVirtualCamera virtualCam = _object.GetComponent<CinemachineVirtualCamera>();
        virtualCam.Priority = 11;
    }

    public void ResetCMVirtualPriorityValue(GameObject _object)
    {
        CinemachineVirtualCamera virtualCam = _object.GetComponent<CinemachineVirtualCamera>();
        virtualCam.Priority = 10;
    }
}