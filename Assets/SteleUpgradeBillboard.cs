using UnityEngine;

public class SteleUpgradeBillboard : MonoBehaviour
{
    private Canvas Canvas => GetComponent<Canvas>();

    private void Start ()
    {
        Canvas.worldCamera = UtilityClass.GetMainCamera();
        gameObject.SetActive(false);
    }
}