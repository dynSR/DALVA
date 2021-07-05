using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    public GameObject padlockObject;
    private Button ButtonComponent => GetComponent<Button>();
    private UIButtonSound UIButtonSound => GetComponent<UIButtonSound>();

    private void Start()
    {
        if (GameParameters.Instance.maxLevelDone >= 1)
        {
            UIButtonSound.enabled = true;
        }
        else if(GameParameters.Instance.maxLevelDone == 0)
        {
            UIButtonSound.enabled = false;
        }
    }

    private void Update()
    {
        if (padlockObject.activeInHierarchy && GameParameters.Instance.maxLevelDone >= 1)
        {
            padlockObject.SetActive(false);
            ButtonComponent.interactable = true;
            UIButtonSound.enabled = true;
        }
    }
}
