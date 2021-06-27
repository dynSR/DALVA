using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private SpawnerSystem spawner;

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI placeToDefendHealtAmountText;
    public int maxHealthAmount;

    [Header("WAVE")]
    [SerializeField] private GameObject waveIndicationUI;
    [SerializeField] private GameObject waveDisplayerParent;

    [Header("TIMER")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI waveCountText;

    [Header("SCREENS")]
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    [Header("BASE DAMAGE LOSS")]
    public Animator damageLossAnimator;
    public TextMeshProUGUI damageLossText;
    public int damageLossValue;

    [Header("RESSOURCES")]
    public GameObject ressourcesLossFeedback;

    float timeValue = 0f;

    public GameObject pauseMenu;
    public GameObject popupsParent;
    public GameObject waveBossPing;

    #region Singleton
    public static UIManager Instance;

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

    private void OnEnable()
    {
        PlaceToDefend.OnHealthValueChanged += UpdatePlaceToDefendHealth;
    }

    private void OnDisable()
    {
        PlaceToDefend.OnHealthValueChanged -= UpdatePlaceToDefendHealth;
    }

    private void Start()
    {
        timerText.SetText("00 : 00");
        PopulateSpawnersList();

        UpdateWaveCount(spawner.IndexOfCurrentWave);

        UpdapteGameTimer();
    }

    private void LateUpdate() => UpdapteGameTimer();

    SpawnerSystem PopulateSpawnersList()
    {
        return this.spawner = GameManager.Instance.Spawner;
    }

    void UpdapteGameTimer()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

        timeValue += Time.deltaTime;

        string minutes = Mathf.Floor(timeValue / 60).ToString("0");
        string seconds = Mathf.Floor(timeValue % 60).ToString("00");

        timerText.SetText(minutes + " : " + seconds);
    }

    public void UpdateWaveCount(int amnt)
    {
        waveCountText.SetText("Vague : " + amnt.ToString("0") + " / " + spawner.Waves.Count.ToString("0"));
    }

    public void SetDamageLoss(int value)
    {
        damageLossValue = value;
    }

    public void UpdatePlaceToDefendHealth(int amnt)
    {
        placeToDefendHealtAmountText.SetText(amnt.ToString("0") +  " / " + maxHealthAmount.ToString("0"));

        if (damageLossValue > 0)
        {
            damageLossText.SetText("- " + damageLossValue.ToString("0"));
            damageLossAnimator.SetTrigger("DamageFeedback");
        }
    }

    #region Popup
    public void DisplayValidationPopup(GameObject popup)
    {
        if (!popup.activeInHierarchy) popup.SetActive(true);
    }

    public void HideValidationPopup(GameObject popup)
    {
        if (popup.activeInHierarchy) popup.SetActive(false);
    }
    #endregion

    #region PauseMenu
    public void DisplayPauseMenu()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            GameManager.Instance.Player.GetComponent<CursorLogic>().SetCursorToNormalAppearance();
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            GameManager.Instance.SetGameToProperMod();

            foreach (Transform item in popupsParent.transform)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void GetBackToMainMenu()
    {
        StartCoroutine(GetBackToMainMenuCoroutine());
    }

    private IEnumerator GetBackToMainMenuCoroutine()
    {
        InGameSceneTransitionManager.Instance.TriggerFadeIn();
        Time.timeScale = 1;

        yield return new WaitForSeconds(1.25f);

        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void RestartLevel ()
    {
        StartCoroutine(RestartLevelCoroutine());
    }

    private IEnumerator RestartLevelCoroutine()
    {
        InGameSceneTransitionManager.Instance.TriggerFadeIn();
        Time.timeScale = 1;

        yield return new WaitForSeconds(1.25f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void DisplayVictory()
    {
        victoryScreen.SetActive(true);
    }

    public void DisplayDefeat()
    {
        defeatScreen.SetActive(true);
    }
    #endregion

    public void DisplayBossWavePing()
    {
        waveBossPing.SetActive(true);
    }

    public void HideBossWavePing()
    {
        waveBossPing.SetActive(false);
    }
}