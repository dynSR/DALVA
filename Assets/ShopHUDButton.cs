using UnityEngine;

public class ShopHUDButton : MonoBehaviour
{
    public GameObject feedbackObject;
    public GameObject disponibleDrawObject;
    private CharacterRessources characterRessources;


    void Start()
    {
        characterRessources = PlayerHUDManager.Instance.Player.GetComponent<CharacterRessources>();
    }

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

        if (characterRessources.CurrentAmountOfPlayerRessources >= 150 && !disponibleDrawObject.activeInHierarchy)
        {
            disponibleDrawObject.SetActive(true);
        }
        else if (characterRessources.CurrentAmountOfPlayerRessources < 150 && disponibleDrawObject.activeInHierarchy)
        {
            disponibleDrawObject.SetActive(false);
        }
    }
}