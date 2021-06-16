using UnityEngine;
using UnityEngine.UI;

public class WaveStateDisplayer : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject content;
    [SerializeField] private SpawnerSystem spawner;
    [SerializeField] private GameObject breathingButtonVFX;
    public float timerAssigned;
    public float localTimer;
    [SerializeField] private GameObject button;
    public Image waveIconImage;
    //public Sprite[] waveIcons;
    public GameObject crownIcon;

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

            if(!breathingButtonVFX.activeInHierarchy && GameManager.Instance.GameIsInPlayMod())
            {
                breathingButtonVFX.SetActive(true);
            }
        }   
    }

    void ToggleContent(float boolValue)
    {
        if (boolValue == 0 && content.activeInHierarchy)
        {
            content.SetActive(false);

            UIButtonWithTooltip buttonTooltip = button.GetComponent<UIButtonWithTooltip>();
            buttonTooltip.HideTooltip(buttonTooltip.Tooltip);

            breathingButtonVFX.SetActive(false);
        }
        else if (boolValue == 1 && !content.activeInHierarchy)
        { 
            content.SetActive(true);

            if (spawner.ItIsABossWave())
            {
                crownIcon.SetActive(true);
                //waveIconImage.sprite = waveIcons[1];
            }
            else if (!spawner.ItIsABossWave())
            {
                crownIcon.SetActive(false);
                //waveIconImage.sprite = waveIcons[0];
            }

            GameManager.Instance.ItIsABossWave = spawner.ItIsABossWave();
        }
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