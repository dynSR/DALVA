using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveStateDisplayer : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject content;
    [SerializeField] private SpawnerSystem[] spawner;
    public float timerAssigned;
    public float localTimer;


    private void OnEnable()
    {
        for (int i = 0; i < spawner.Length; i++)
        {
            spawner[i].OnWaveStartinSoon += SetWaveDisplayerFillAmount;
            spawner[i].OnWavePossibilityToSpawnState += ToggleContent;
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < spawner.Length; i++)
        {
            spawner[i].OnWaveStartinSoon -= SetWaveDisplayerFillAmount;
            spawner[i].OnWavePossibilityToSpawnState -= ToggleContent;
        }
    }

    private void Update()
    {
        UpdateFillAmount();
    }

    public void SetWaveDisplayerFillAmount(float timer)
    {
        timerAssigned = timer;
        localTimer = timer;
        fillImage.fillAmount = localTimer / timerAssigned;
    }

    void UpdateFillAmount()
    {
        if (localTimer > 0)
        {
            localTimer -= Time.deltaTime;
            fillImage.fillAmount = localTimer / timerAssigned;
        }   
    }

    void ToggleContent(float boolValue)
    {
        if (boolValue == 0 && content.activeInHierarchy) content.SetActive(false);
        else if (boolValue == 1 && !content.activeInHierarchy) content.SetActive(true);
    }
}