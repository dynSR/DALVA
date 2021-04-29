using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityContainerLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IButtonTooltip
{
    [Header("CONTAINED ABILITY")]
    [SerializeField] private AbilityLogic containedAbility;
    [SerializeField] private Animator animator;
    [SerializeField] private Image containedAbilityIcon;

    [Header("KEY")]
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private TextMeshProUGUI keyText;

    [Header("COOLDOWN")]
    [SerializeField] private GameObject cooldownContainerGameObject;
    [SerializeField] private Image cooldownFiller;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Header("TIMER TEXT FORMAT")]
    [SerializeField] private bool onlySeconds = false;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltip;

    public KeyCode AbilityKey { get => abilityKey; set => abilityKey = value; }
    public AbilityLogic ContainedAbility { get => containedAbility; set => containedAbility = value; }

    private void OnEnable()
    {
        AbilitiesCooldownHandler.OnAbitilityUsed += UpdateAbilityCooldownUI;
    }

    private void OnDisable()
    {
        AbilitiesCooldownHandler.OnAbitilityUsed -= UpdateAbilityCooldownUI;
    }

    void Start() => SetTheAbilityUI(this.ContainedAbility);

    protected void SetTheAbilityUI(AbilityLogic containedAbility)
    {
        //Set ability key + Input UI
        abilityKey = containedAbility.Ability.AbilityKey;
        keyText.SetText(abilityKey.ToString());

            if (containedAbility.Ability.AbilityIcon != null)
                containedAbilityIcon.sprite = containedAbility.Ability.AbilityIcon;
    }

    private void UpdateAbilityCooldownUI(AbilityLogic containedAbility)
    {
        if (containedAbility == ContainedAbility)
        {
            //Activer le timer text et l'image filled
            cooldownContainerGameObject.SetActive(true);

            //Update le texte et l'image filled
            StartCoroutine(UpdateCooldownUIComponents(containedAbility));
        }
    }

    private IEnumerator UpdateCooldownUIComponents(AbilityLogic containedAbility)
    {
        float storedCooldown = containedAbility.Ability.AbilityCooldown;

        do
        {
            storedCooldown -= Time.deltaTime;

            //Mettre à jour le timer text
            if (onlySeconds)
            {
                cooldownText.SetText(storedCooldown.ToString("0"));
            }
            else if (!onlySeconds)
            {
                //Calcul minutes + secondes
                string minutes = Mathf.Floor(storedCooldown / 60).ToString("0");
                string seconds = Mathf.Floor(storedCooldown % 60).ToString("00");

                cooldownText.SetText(minutes + ":" + seconds);
            }

            if (storedCooldown <= 1)
                cooldownText.SetText(storedCooldown.ToString("0.0"));

            //Update l'image filled
            cooldownFiller.fillAmount = storedCooldown / containedAbility.Ability.AbilityCooldown;

            yield return new WaitForEndOfFrame();
        } while (storedCooldown > 0.1f);

        //Désactiver le timer text et l'image filled
        cooldownContainerGameObject.SetActive(false);

        animator.SetTrigger("EndOfCD");
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayTooltip(tooltip);
        UpdateContainerTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(tooltip);
    }

    public void DisplayTooltip(GameObject tooltip)
    {
        if (!tooltip.activeInHierarchy)
            tooltip.SetActive(true);
    }

    public void HideTooltip(GameObject tooltip)
    {
        if (tooltip.activeInHierarchy)
            tooltip.SetActive(false);
    }

    public void UpdateContainerTooltip()
    {
        TooltipSetter tooltipSetter = tooltip.GetComponent<TooltipSetter>();

        tooltipSetter.SetTooltip(
            ContainedAbility.Ability.AbilityName,
            ContainedAbility.Ability.AbilityDescription,
            ContainedAbility.Ability.AbilityCooldown.ToString("0" + " s"));
    }
}
