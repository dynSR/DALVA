using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveStateDisplayer : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject content;
    [SerializeField] private SpawnerSystem spawner;
    public TextMeshProUGUI timerText;
    [SerializeField] private GameObject button;
    public Image waveIconImage;
    public GameObject crownIcon;

    [Header("NUMERIC VARIABLES")]
    public float timerAssigned;
    public float localTimer;
    
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

    private void Start ()
    {
        SetTextTimer(spawner.DelayBeforeSpawningFirstWave);
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

            SetTextTimer(localTimer);

            if (localTimer <= 1)
                timerText.SetText(localTimer.ToString("0.0"));
        }   
    }

    void SetTextTimer(float value)
    {
        //Update text timer here
        //Minutes + secondes
        string minutes = Mathf.Floor(value / 60).ToString("0");
        string seconds = Mathf.Floor(value % 60).ToString("00");

        timerText.SetText(minutes + " : " + seconds);
    }

    void ToggleContent(float boolValue)
    {
        if (boolValue == 0 && content.activeInHierarchy)
        {
            content.SetActive(false);
        }
        else if (boolValue == 1 && !content.activeInHierarchy)
        { 
            content.SetActive(true);

            if (spawner.ItIsABossWave())
            {
                crownIcon.SetActive(true);
            }
            else if (!spawner.ItIsABossWave())
            {
                crownIcon.SetActive(false);
            }

            GameManager.Instance.ItIsABossWave = spawner.ItIsABossWave();
        }
    }

    public void SpawnWaveOnButtonClick()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;
        
        spawner.CallSpawnEvent();
        ToggleContent(0);
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