using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    public GameObject padlockObject;

    private void Update()
    {
        if (padlockObject.activeInHierarchy && GameParameters.Instance.maxLevelDone >= 1)
        {
            padlockObject.SetActive(false);
        }
    }
}
