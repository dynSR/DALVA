using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("DEBUG SECTION")]
    [SerializeField] private TextMeshProUGUI debugCameraStateText;
    [SerializeField] private GameObject debugSectionWindow;
    bool IsDebugSectionWindowOpen = true;

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

    private void Update()
    {
        switch (UtilityClass.GetMainCamera().GetComponent<CameraController>().CameraLockState)
        {
            case CameraLockState.Locked:
                debugCameraStateText.text = "Vérouillée";
                break;
            case CameraLockState.Unlocked:
                debugCameraStateText.text = "Libre";
                break;
            default:
                break;
        }
    }

    public void OpenCloseDebugSectionWindow()
    {
        if (IsDebugSectionWindowOpen)
        {
            debugSectionWindow.SetActive(false);
            IsDebugSectionWindowOpen = false;
        }
        else if (!IsDebugSectionWindowOpen)
        {
            debugSectionWindow.SetActive(true);
            IsDebugSectionWindowOpen = true;
        }
    }
}