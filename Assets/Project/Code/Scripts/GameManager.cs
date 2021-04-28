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
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public static GameState GameState;

    [Header("WAVE")]
    [SerializeField] private int waveDone = 0;
    private bool waveCountHasBeenSet = false;

    [Header("SPAWNERS")]
    [SerializeField] private List<SpawnerSystem> spawners = new List<SpawnerSystem>();
    public List<SpawnerSystem> Spawners { get => spawners; }
    public int WaveDone { get => waveDone; set => waveDone = value; }
    public bool WaveCountHasBeenSet { get => waveCountHasBeenSet; set => waveCountHasBeenSet = value; }

    void Start()
    {
        
    }

    #region Game Mods
    public void PauseGame()
    {
        GameState = GameState.Pause;
    }

    public void SetGameToPlayMod()
    {
        GameState = GameState.PlayMod;
    }

    public void SetGameToStandbyMod()
    {
        GameState = GameState.StandbyMod;
    }

    public void Victory()
    {
        GameState = GameState.Victory;
    }

    public void Defeat()
    {
        GameState = GameState.Defeat;
    }
    #endregion
}
