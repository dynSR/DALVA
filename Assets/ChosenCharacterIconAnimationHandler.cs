using UnityEngine;

public class ChosenCharacterIconAnimationHandler : MonoBehaviour
{
    public void DisplayUIManagerComponent()
    {
        foreach (GameObject gO in UIManager.Instance.endScreenComponents)
        {
            gO.SetActive(true);
        }
    }
}