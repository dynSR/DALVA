using UnityEngine;
using UnityEngine.UI;

public class WaveStateDisplayer : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject content;
    [SerializeField] private SpawnerSystem spawner;
    public float timerAssigned;
    public float localTimer;
    [SerializeField] private GameObject button;

    private void OnEnable()
    {
        spawner.OnWaveStartinSoon += SetWaveDisplayerFillAmount;
        spawner.OnWavePossibilityToSpawnState += ToggleContent;
    }
    private void OnDisable()
    {
        spawner.OnWaveStartinSoon -= SetWaveDisplayerFillAmount;
        spawner.OnWavePossibilityToSpawnState -= ToggleContent;
    }

    private void Update()
    {
        ToggleButtonState();

        if (!GameManager.Instance.GameIsInPlayMod()
            || spawner.waveState == WaveState.IsSpawning) return;

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

    public void SpawnWaveOnButtonClick()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;
        
        spawner.CallSpawnEvent();
        ToggleContent(0);
        button.GetComponent<UIButtonWithTooltip>().Tooltip.SetActive(false);
    }

    private void ToggleButtonState()
    {
        if (!GameManager.Instance.GameIsInPlayMod() && button.activeInHierarchy)
        {
            button.SetActive(false);
            //Debug.Log("ToggleButtonState - OFF");
        }
        else if (GameManager.Instance.GameIsInPlayMod() && !button.activeInHierarchy)
        {
            button.SetActive(true);
            //Debug.Log("ToggleButtonState - ON");
        }
    }
}