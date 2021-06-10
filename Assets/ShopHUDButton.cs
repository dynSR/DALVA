using UnityEngine;

public class ShopHUDButton : MonoBehaviour
{
    public GameObject feedbackObject;

    void LateUpdate()
    {
        if (PlayerHUDManager.Instance.IsShopWindowOpen && !feedbackObject.activeInHierarchy)
        {
            feedbackObject.SetActive(true);
        }
        else if (!PlayerHUDManager.Instance.IsShopWindowOpen && feedbackObject.activeInHierarchy)
        {
            feedbackObject.SetActive(false);
        }
    }
}
