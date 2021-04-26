using UnityEngine;

public class DisplayLeftSection : MonoBehaviour
{
    [SerializeField] private PlayerHUDManager playerHUD;
    [SerializeField] private GameObject leftSectionGameObject;
    private bool LeftSectionOpen => leftSectionGameObject.activeInHierarchy;

    public void ToggleLeftSection()
    {
        playerHUD.ToggleAWindow(LeftSectionOpen, leftSectionGameObject);
    }
}
