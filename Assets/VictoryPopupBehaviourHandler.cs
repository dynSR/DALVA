using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPopupBehaviourHandler : MonoBehaviour
{
    public string sceneNameToLoad;

    private void OnDisable ()
    {
        sceneNameToLoad = string.Empty;
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
}