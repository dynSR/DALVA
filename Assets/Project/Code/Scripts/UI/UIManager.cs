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

    [Header("TREE LIFE COMPONENTS")]
    public GameObject treeLifeObject;

    [Header("WAVE")]
    public GameObject waveIndicationUI;
    [SerializeField] private GameObject waveDisplayerParent;

    [Header("TIMER")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI waveCountText;

    [Header("SCREENS")]
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public GameObject [ ] characterIcons;
    public GameObject [ ] endScreenComponents;
    public Animator[ ] endScreenAnimators;

    [Header("BASE DAMAGE LOSS")]
    public Animator damageLossAnimator;
    public TextMeshProUGUI damageLossText;
    public int damageLossValue;

    [Header("RESSOURCES")]
    public GameObject ressourcesLossFeedback;

    float timeValue = 0f;

    public GameObject pauseMenu;
    public GameObject[] popups;
    public GameObject waveBossPing;

    public bool aValidationPopupIsCurrentlyDisplayed = false;
    public bool debugClass = false;

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
        HideAllPopup();

        if (!popup.activeInHierarchy) popup.SetActive(true);

        aValidationPopupIsCurrentlyDisplayed = true;
    }

    public void HideValidationPopup(GameObject popup)
    {
        if (popup.activeInHierarchy) popup.SetActive(false);

        aValidationPopupIsCurrentlyDisplayed = false;
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

            HideAllPopup();
        }
    }

    public void HideAllPopup()
    {
        foreach (GameObject item in popups)
        {
            item.SetActive(false);
            aValidationPopupIsCurrentlyDisplayed = false;
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

        yield return new WaitForSeconds(0.2f);

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

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void DisplayVictory()
    {
        if (GameParameters.Instance && GameParameters.Instance.classIsMage || UIManager.Instance.debugClass)
        {
            characterIcons [ 0 ].SetActive(true);
        }
        else
        {
            characterIcons [ 1 ].SetActive(true);
        }

        victoryScreen.SetActive(true);

        foreach (Animator animator in endScreenAnimators)
        {
            animator.SetTrigger("Victory");
        }
    }

    public void DisplayDefeat()
    {
        if (GameParameters.Instance && GameParameters.Instance.classIsMage || UIManager.Instance.debugClass)
        {
            characterIcons [ 2 ].SetActive(true);
        }
        else
        {
            characterIcons [ 3 ].SetActive(true);
        }

        defeatScreen.SetActive(true);

        foreach (Animator animator in endScreenAnimators)
        {
            animator.SetTrigger("Defeat");
        }
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