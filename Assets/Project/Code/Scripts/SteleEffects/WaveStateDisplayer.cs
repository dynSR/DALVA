using UnityEngine;
using UnityEngine.UI;

public class WaveStateDisplayer : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject content;
    private SpawnerSystem spawner;
    public float timerAssigned;
    public float localTimer;

    private void Awake()
    {
        spawner = GameManager.Instance.Spawner;
    }

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
        if (!GameManager.Instance.GameIsInPlayMod()) return;

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