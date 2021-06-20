using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetShopButton : UIButtonWithTooltip
{
    [SerializeField] private ShopManager shop;
    [SerializeField] private TextMeshProUGUI resetDrawCostText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private GameObject cantAffordFeedback;
    public GameObject undisponibilityObject;
    Color initialResetDrawCostTextColor;

    private CharacterRessources PlayerRessources => shop.Player.GetComponent<CharacterRessources>();

    protected override void OnEnable()
    {
        base.OnEnable();
        shop.OnShopDrawSetResetCost += SetResetDrawCostText;
        PlayerRessources.OnCharacterRessourcesChanged += ToggleButtonStatus;
    }

    private void OnDisable()
    {
        shop.OnShopDrawSetResetCost -= SetResetDrawCostText;
        PlayerRessources.OnCharacterRessourcesChanged -= ToggleButtonStatus;
    }

    void Awake()
    {
        initialResetDrawCostTextColor = resetDrawCostText.color;
        GetComponent<UIButtonHighlight>().enabled = false;
    }

    private void Update()
    {
        if (shop.firstDrawDone == true && undisponibilityObject.activeInHierarchy)
        {
            GetComponent<UIButtonHighlight>().enabled = true;
            undisponibilityObject.SetActive(false);
        }
    }

    void SetResetDrawCostText(int value)
    {
        resetDrawCostText.SetText(value.ToString("0"));
    }

    void ToggleButtonStatus()
    {
        UIButtonSound uiButtonSoundScript = GetComponent<UIButtonSound>();

        if (PlayerRessources.CurrentAmountOfPlayerRessources >= shop.ResetDrawCost)
        {
            resetDrawCostText.color = initialResetDrawCostTextColor;
            backgroundImage.color = enabledColor;
            uiButtonSoundScript.enabled = true;
            cantAffordFeedback.SetActive(false);
        }
        else if (PlayerRessources.CurrentAmountOfPlayerRessources < shop.ResetDrawCost)
        {
            resetDrawCostText.color = Color.red;
            backgroundImage.color = disabledColor;
            uiButtonSoundScript.enabled = false;
            cantAffordFeedback.SetActive(true);
        }
    }
}