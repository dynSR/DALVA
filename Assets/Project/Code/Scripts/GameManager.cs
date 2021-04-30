﻿using System.Collections;
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

    [SerializeField] private ShopManager shop;

    [Header("WAVE")]
    [SerializeField] private int waveDone = 0;
    private int InternalCounter = 0;
    private bool waveCountHasBeenSet = false;

    [Header("SPAWNERS")]
    [SerializeField] private List<SpawnerSystem> spawners = new List<SpawnerSystem>();
    public List<SpawnerSystem> Spawners { get => spawners; }
    public int WaveDone { get => waveDone; set => waveDone = value; }
    public bool WaveCountHasBeenSet { get => waveCountHasBeenSet; set => waveCountHasBeenSet = value; }
    

    void Start()
    {
        
    }

    public void FithWaveTracker()
    {
        InternalCounter++;

        if (InternalCounter == 6)
        {
            InternalCounter = 0;
            SetGameToStandbyMod();
            PlayerHUDManager.Instance.OpenWindow(PlayerHUDManager.Instance.ShopWindow);
            
        }
    }

    #region Game Mods
    public void PauseGame()
    {
        GameState = GameState.Pause;
        Time.timeScale = 0;
    }

    public void SetGameToPlayMod()
    {
        GameState = GameState.PlayMod;
        Time.timeScale = 1;
    }

    public void SetGameToStandbyMod()
    {
        GameState = GameState.StandbyMod;
        Time.timeScale = 0;
    }

    public void Victory()
    {
        GameState = GameState.Victory;
        Time.timeScale = 0;
    }

    public void Defeat()
    {
        GameState = GameState.Defeat;
        Time.timeScale = 0;
    }
    #endregion
}