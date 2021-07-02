using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPopupBehaviourHandler : MonoBehaviour
{
    public string sceneNameToLoad;
    public VictoryScreen_ClassChoiceButtonHandler [ ] victoryScreen_ClassChoiceButtonHandler;

    private void OnDisable ()
    {
        sceneNameToLoad = string.Empty;
        ResetAllClassChoiseButtonSelection();
    }

    public void SetSceneToLoadAsString (string sceneName)
    {
        sceneNameToLoad = sceneName;
    }

    public void LoadStoredScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }

    public void SetChosenClass(bool value)
    {
        GameParameters.Instance.classIsMage = value;
    }

    public void ResetAllClassChoiseButtonSelection()
    {
        foreach (VictoryScreen_ClassChoiceButtonHandler item in victoryScreen_ClassChoiceButtonHandler)
        {
            item.isSelected = false;
            item.highlightObject.SetActive(false);
        }
    }
}