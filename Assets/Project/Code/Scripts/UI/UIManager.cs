using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private SpawnerSystem spawner;

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI placeToDefendHealtAmountText;

    [Header("WAVE")]
    [SerializeField] private GameObject waveIndicationUI;
    [SerializeField] private GameObject waveDisplayerParent;

    [Header("TIMER")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI waveCountText;

    float timeValue = 0f;

    public GameObject pauseMenu;

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
        waveCountText.SetText(amnt.ToString("0"));
    }

    public void UpdatePlaceToDefendHealth(int amnt)
    {
        placeToDefendHealtAmountText.SetText(amnt.ToString("0"));
    }

    #region Popup
    public void DisplayValidationPopup(GameObject popup)
    {
        if (!popup.activeInHierarchy) popup.SetActive(true);
    }

    public void HideValidationPopup(GameObject popup)
    {
        if (!popup.activeInHierarchy) popup.SetActive(false);
    }
    #endregion

    #region PauseMenu
    public void DisplayPauseMenu()
    {
        if (!pauseMenu.activeInHierarchy) pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseMenu.activeInHierarchy) pauseMenu.SetActive(false);
    }

    public void GetBackToMainMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString(), LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}