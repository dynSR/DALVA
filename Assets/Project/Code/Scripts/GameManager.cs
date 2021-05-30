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

            //Debug.Log("COUCOU", transform);
        }
    }
    #endregion

    public static GameState GameState;

    [Header("WAVE")]
    [SerializeField] private int waveDone = 0;
    private int InternalCounter = 0;
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
    public bool mapIsEasy = false;
    public bool tutorielDisplayed = false;

    public int DalvaLifePoints = 0;
    public List<PlaceToDefend> placesToDefend;

    public GameObject MageCharacter;
    public GameObject WarriorCharacter;

    public Transform Player { get; set; }
    
    void Start()
    {
        if (!mapIsEasy) ShopPhase();
        else SetGameToTutorialMod();

        DalvaLifePoints = CalculateDalvaLifePoints();
        UIManager.Instance.UpdatePlaceToDefendHealth(DalvaLifePoints);

        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (UtilityClass.IsKeyPressed(KeyCode.Escape))
        {
            if (!GameIsInPause())
            {
                PauseGame();
            }
            else if (GameIsInPause())
            {
                SetGameToProperMod();
                //SetGameToPlayMod();
            }
        }
    }

    private void LateUpdate()
    {
        if(GameIsInPlayMod() && !ItIsABossWave)
            UpdateSpawnersState();
    }

    private int CalculateDalvaLifePoints()
    {
        int value = 0;

        foreach (PlaceToDefend item in placesToDefend)
        {
            value += item.health;
        }

        return value;
    }

    private void ActivateTheRightCharacter()
    {
        if (GameParameters.classIsMage)
        {
            MageCharacter.SetActive(true);
        }
        else if (!GameParameters.classIsMage)
        {
            WarriorCharacter.SetActive(true);
        }
        else MageCharacter.SetActive(true);
    }

    private void UpdateInternalCounter()
    {
        InternalCounter++;

        if (InternalCounter == 6)
        {
            InternalCounter = 0;
            ShopPhase();
        }
    }

    public void UpdateRemainingMonsterValue(int value)
    {
        Debug.Log("UpdateRemainingMonsterValue");

        RemainingMonstersValue += value;
        //Display & Update UI

        if (RemainingMonstersValue == 0 && ItIsABossWave)
        {
            if (itsFinalWave)
            {
                Victory();
                return;
            }

            ResetWhenBossWaveIsDone();
            //Hide UI
        }
    }

    public void ResetWhenBossWaveIsDone()
    {
        ItIsABossWave = false;
        UpdateWaveCount();
        UpdateSpawnersState();
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
        UpdateInternalCounter(); 
        WaveCountHasBeenSet = true;
    }

    public void UpdateSpawnersState()
    {
        if (spawner.waveState == WaveState.Standby && WaveCountHasBeenSet)
        {
            Debug.Log("Update Check", transform);
            spawner.IndexOfCurrentWave++;
            spawner.spawnEventEndedHasBeenHandled = true;
            spawner.UpdateElementsOnSpawnFinished();

            WaveCountHasBeenSet = false;
        }
    }
    
    public void ShopPhase()
    {
        if (!tutorielDisplayed)
            PlayerHUDManager.Instance.OpenWindow(PlayerHUDManager.Instance.ShopWindow);

        SetGameToStandbyMod();
    }

    #region Game Mods
    #region Pause
    public void PauseGame()
    {
        //Save previous state here
        if (GameIsInPlayMod()) gameWasInPlayMod = true;
        else if (GameIsInStandBy()) gameWasInPlayMod = false;

        GameState = GameState.Pause;
        Time.timeScale = 0;

        UIManager.Instance.DisplayPauseMenu();
    }

    private bool GameIsInPause()
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

    public void SetGameToTutorialMod()
    {
        GameState = GameState.Tutorial;

        if (mapIsEasy && tutorielDisplayed)
            PlayerHUDManager.Instance.CloseWindow(PlayerHUDManager.Instance.ShopWindow);
    }

    public void Victory()
    {
        GameState = GameState.Victory;
        UIManager.Instance.DisplayVictory();

        Time.timeScale = 0;
    }

    public void Defeat()
    {
        GameState = GameState.Defeat;
        UIManager.Instance.DisplayDefeat();

        Time.timeScale = 0;
    }
    #endregion
}
