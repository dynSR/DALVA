using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlayMod,
    Pause,
    StandbyMod,
    Victory,
    Defeat,
    Tutorial,
    Intro,
}

public class GameManager : MonoBehaviour
{
    public delegate void WaveSpawnHandler();
    public static event WaveSpawnHandler OnWaveSpawningSoon;

    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("COUCOU", transform);
            Destroy(this);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            if (Spawner == null)
                Debug.LogError("Need to add at least one spawner in the field array of --Spawner--", transform);

            ActivateTheRightCharacter();

            if (!sceneTransition.activeInHierarchy)
            {
                sceneTransition.SetActive(true);
            }

            //Debug.Log("COUCOU", transform);
        }
    }
    #endregion

    public GameState GameState;

    [Header("WAVE")]
    [SerializeField] private int waveDone = 0;
    private bool waveCountHasBeenSet = false;

    [Header("SPAWNERS")]
    [SerializeField] private SpawnerSystem spawner;
    public SpawnerSystem Spawner { get => spawner; }
    public int WaveDone { get => waveDone; set => waveDone = value; }
    public bool WaveCountHasBeenSet { get => waveCountHasBeenSet; set => waveCountHasBeenSet = value; }
    public bool ItIsABossWave = false;
    public int RemainingMonstersValue = 0;
    private bool gameWasInPlayMod = false;

    public bool itsFinalWave = false;
    public bool tutorialsAreEnabled = false;
    public bool tutorielDisplayed = false;

    public int DalvaLifePoints = 0;
    public int damageAppliedByPlayerToTheTree = 0;
    public List<PlaceToDefend> placesToDefend;

    public GameObject MageCharacter;
    public GameObject WarriorCharacter;

    public GameObject victoryVirtualCameraObject;
    public GameObject defeatVirtualCameraObject;

    public GameObject sceneTransition;

    public Transform Player { get; set; }
    
    void Start()
    {
        PlayerHUDManager.Instance.CloseWindow(PlayerHUDManager.Instance.ShopWindow);

        if (!tutorialsAreEnabled)
        {
            SetGameToIntroMod();
        }
        else if (tutorialsAreEnabled)
        {
            SetGameToTutorialMod();
        }

        DalvaLifePoints = CalculateDalvaLifePoints();
        UIManager.Instance.UpdatePlaceToDefendHealth(DalvaLifePoints);

        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Player.GetComponent<EntityStats>().DamageAppliedToThePlaceToDefend = damageAppliedByPlayerToTheTree;
    }

    void Update()
    {
        if (UtilityClass.IsKeyPressed(KeyCode.Escape)
            && !tutorielDisplayed)
        {
            if (GameState == GameState.Intro) return;

            if(UIManager.Instance.aValidationPopupIsCurrentlyDisplayed)
            {
                UIManager.Instance.HideAllPopup();
                return;
            }

            if (GameState != GameState.Victory || GameState != GameState.Defeat)
            {
                if (PlayerHUDManager.Instance.isShopWindowOpen && GameIsInPlayMod())
                {
                    PlayerHUDManager.Instance.CloseWindow(PlayerHUDManager.Instance.ShopWindow);
                    return;
                }

                if (!GameIsInPause())
                {
                    Debug.Log("Pause Game Call");
                    PauseGame(true);
                }
                else if (GameIsInPause())
                {
                    Debug.Log("SetGameToProperMod");
                    SetGameToProperMod();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if(GameIsInPlayMod())
            UpdateSpawnersState();
    }

    private int CalculateDalvaLifePoints()
    {
        int value = 0;

        foreach (PlaceToDefend item in placesToDefend)
        {
            value += item.health;
        }

        //Set UI Infos
        UIManager.Instance.maxHealthAmount = value;

        UIMainHealthBar.Instance.currentAmountOfHealth = value;
        UIMainHealthBar.Instance.maxAmountOfLife = value;

        UIMainHealthBar.Instance.SetHealthBarParameters(value);

        return value;
    }

    private void ActivateTheRightCharacter()
    {
        if (GameParameters.Instance == null)
        {
            //MageCharacter.SetActive(true);
            WarriorCharacter.SetActive(true);
            Debug.Log("Default class chosen at Start !");
            return;
        }

        if (GameParameters.Instance.classIsMage)
        {
            MageCharacter.SetActive(true);
            Debug.Log("GameParameters Value : " + GameParameters.Instance.classIsMage + " Mage chosen at Start !");
        }
        else if (!GameParameters.Instance.classIsMage)
        {
            WarriorCharacter.SetActive(true);
            Debug.Log("GameParameters Value : " + GameParameters.Instance.classIsMage + " Warrior chosen at Start !");
        }
    }

    public void UpdateRemainingMonsterValue(int value)
    {
        Debug.Log("UpdateRemainingMonsterValue");

        RemainingMonstersValue += value;
        
        if (RemainingMonstersValue == 0)
        {
            if (itsFinalWave)
            {
                StartCoroutine(Victory(0.75f));
                return;
            }
        }
    }

    public void RecountRemainingMonster()
    {
        EntityDetection[] entitiesDetected = (EntityDetection[])FindObjectsOfType(typeof(EntityDetection));
        int value = 0;

        foreach (EntityDetection entity in entitiesDetected)
        {
            EntityStats entityStats = entity.gameObject.GetComponent<EntityStats>();

            if (entity.ThisTargetIsAMinion(entity) && !entityStats.IsDead)
            {
                value++;
            }  
        }

        RemainingMonstersValue = value;
        //Update UI;
    }

    public void UpdateWaveCount()
    {
        WaveDone++;
        UIManager.Instance.UpdateWaveCount(WaveDone);

        //Not needed anymore the 5th waves rotation doesn't matter
        //UpdateInternalCounter(); 
        WaveCountHasBeenSet = true;

    }

    public void UpdateSpawnersState()
    {
        if (spawner.waveState == WaveState.Standby && WaveCountHasBeenSet)
        {
            Debug.Log("Update Check", transform);
            spawner.IndexOfCurrentWave++;
            spawner.spawnEventEndedHasBeenHandled = true;
            StartCoroutine(spawner.UpdateElementsOnSpawnFinished());

            WaveCountHasBeenSet = false;
        }
    }
    
    public void ShopPhase()
    {
        if (!tutorielDisplayed)
            PlayerHUDManager.Instance.OpenWindow(PlayerHUDManager.Instance.ShopWindow);

        SetGameToStandbyMod();
        //SetTimeScale(0);
    }

    public void SetTimeScale(int value)
    {
        Time.timeScale = value;
    }

    #region Game Mods
    #region Pause
    public void PauseGame(bool displayPauseMenu = false)
    {
        //Save previous state here
        if (GameIsInPlayMod()) gameWasInPlayMod = true;
        else if (GameIsInStandBy()) gameWasInPlayMod = false;

        GameState = GameState.Pause;
        Time.timeScale = 0;

        if (displayPauseMenu)
            UIManager.Instance.DisplayPauseMenu();
    }

    public bool GameIsInPause()
    {
        return GameState == GameState.Pause;
    }
    #endregion

    #region PlayMod
    public void SetGameToProperMod()
    {
        if (!gameWasInPlayMod) SetGameToStandbyMod();
        else if (gameWasInPlayMod) SetGameToPlayMod();
    }

    public void SetGameToPlayMod()
    {
        GameState = GameState.PlayMod;
        Time.timeScale = 1;

        UIManager.Instance.HidePauseMenu();
    }

    public bool GameIsInPlayMod()
    {
        return GameState == GameState.PlayMod;
    }
    #endregion

    #region Standby
    public void SetGameToStandbyMod()
    {
        GameState = GameState.StandbyMod;

        Time.timeScale = 0;

        UIManager.Instance.HidePauseMenu();
    }

    public bool GameIsInStandBy()
    {
        return GameState == GameState.StandbyMod;
    }
    #endregion

    public void SetGameToIntroMod()
    {
        GameState = GameState.Intro;

        SetTimeScale(1);
    }

    public void SetGameToTutorialMod()
    {
        GameState = GameState.Tutorial;

        if (tutorialsAreEnabled && tutorielDisplayed)
            PlayerHUDManager.Instance.CloseWindow(PlayerHUDManager.Instance.ShopWindow);
    }

    public IEnumerator Victory(float delay)
    {
        Debug.Log("Victory");

        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();

        GameState = GameState.Victory;

        if (PlayerHUDManager.Instance.IsShopWindowOpen)
        {
            PlayerHUDManager.Instance.CloseWindow(PlayerHUDManager.Instance.ShopWindow);
        }

        if (Player.GetComponent<EntityStats>().deathHUD.activeInHierarchy)
        {
            Player.GetComponent<EntityStats>().deathHUD.SetActive(false);
        }

        yield return new WaitForSeconds(delay);

        //Time.timeScale = 1;
        //CAMERA TRAVEL HERE
        CameraTravel(true, false);
        //UIManager.Instance.DisplayVictory();

        Player.GetComponent<CursorLogic>().SetCursorToNormalAppearance();

        if (GameParameters.Instance != null && tutorialsAreEnabled)
            GameParameters.Instance.maxLevelDone++;

        //Represents 90% of the actual delay, to initiate the animation just before the blend is over
        yield return new WaitForSeconds((cinemachineBrain.m_DefaultBlend.BlendTime * 0.9f));

        //Call Victory animation here !

        Debug.Log("Brain blend is over !");
    }

    public IEnumerator Defeat(float delay)
    {
        Debug.Log("Defeat");

        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();

        GameState = GameState.Defeat;

        if (PlayerHUDManager.Instance.IsShopWindowOpen)
        {
            PlayerHUDManager.Instance.CloseWindow(PlayerHUDManager.Instance.ShopWindow);
        }

        if (Player.GetComponent<EntityStats>().deathHUD.activeInHierarchy)
        {
            Player.GetComponent<EntityStats>().deathHUD.SetActive(false);
        }

        yield return new WaitForSeconds(delay);

        //Time.timeScale = 1;

        //CAMERA TRAVEL HERE
        CameraTravel(false, true);

        //UIManager.Instance.DisplayDefeat();

        Player.GetComponent<CursorLogic>().SetCursorToNormalAppearance();

        //Represents 90% of the actual delay, to initiate the animation just before the blend is over
        yield return new WaitForSeconds((cinemachineBrain.m_DefaultBlend.BlendTime * 0.9f));

        //Call Victory animation here !
        Debug.Log("Brain blend is over !");
    }


    private void CameraTravel(bool victory = false, bool defeat = false)
    {
        Debug.Log("Camera Travel");

        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();

        UtilityClass.GetMainCamera().gameObject.SetActive(false);

        StartCoroutine(DisplayScreenTransitionWithDelay((cinemachineBrain.m_DefaultBlend.BlendTime * 0.85f)));

        if (victory)
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = victoryVirtualCameraObject.GetComponent<CinemachineVirtualCamera>();

            cinemachineVirtualCamera.Priority = 11;
        }
        else if (defeat)
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = defeatVirtualCameraObject.GetComponent<CinemachineVirtualCamera>();

            cinemachineVirtualCamera.Priority = 11;
        }
    }

    public void DisplayEndScreenWithDelay()
    {
        if (GameState == GameState.Victory)
        {
            UIManager.Instance.DisplayVictory();
        }
        else if (GameState == GameState.Defeat)
        {
            UIManager.Instance.DisplayDefeat();
        }
    }

    private IEnumerator DisplayScreenTransitionWithDelay(float delay)
    {
       yield return new WaitForSeconds(delay);

        InGameSceneTransitionManager.Instance.TriggerFadeInEndScreen();
    }
    #endregion
}
