using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;using UnityEngine.EventSystems;


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

    [Header("FEEDBACK")]
    [SerializeField] private GameObject usingAbilityFeedbackObject;
    public GameObject UsingAbilityFeedbackObject { get => usingAbilityFeedbackObject; }

    [Header("TIMER TEXT FORMAT")]
    [SerializeField] private bool onlySeconds = false;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltip;

    [Header("ABILITY APPLICATION TYPE ICONS")]
    [SerializeField] private GameObject iconParent;
    [SerializeField] private Sprite aimedType;
    [SerializeField] private Sprite targetedType;
    [SerializeField] private Sprite zoneType;
    [SerializeField] private Sprite buffType;
    [SerializeField] private Sprite healType;

    float storedCooldown = 0f;

    public KeyCode AbilityKey { get => abilityKey; set => abilityKey = value; }
    public AbilityLogic ContainedAbility { get => containedAbility; set => containedAbility = value; }
    public GetCharacterAbilities Parent { get; set; }
   
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
        yield return new WaitUntil(() => GameManager.Instance.GameIsInPlayMod());

        SetAbilityCooldown();

        float cooldownCountdown = storedCooldown;

        do
        {
            cooldownCountdown -= Time.deltaTime;

            //Mettre à jour le timer text
            if (onlySeconds)
            {
                cooldownText.SetText(cooldownCountdown.ToString("0"));
            }
            else if (!onlySeconds)
            {
                //Calcul minutes + secondes
                string minutes = Mathf.Floor(cooldownCountdown / 60).ToString("0");
                string seconds = Mathf.Floor(cooldownCountdown % 60).ToString("00");

                cooldownText.SetText(minutes + ":" + seconds);
            }

            if (cooldownCountdown <= 1)
                cooldownText.SetText(cooldownCountdown.ToString("0.0"));

            //Update l'image filled
            cooldownFiller.fillAmount = cooldownCountdown / containedAbility.Ability.AbilityCooldown;

            yield return new WaitForEndOfFrame();
        } while (cooldownCountdown > 0.1f);

        //Désactiver le timer text et l'image filled
        cooldownContainerGameObject.SetActive(false);

        animator.SetTrigger("EndOfCD");
    }

    public void DisplayAbilityInUseFeedback()
    {
        UsingAbilityFeedbackObject.SetActive(true);
    }

    public void HideAbilityInUseFeedback()
    {
        UsingAbilityFeedbackObject.SetActive(false);
    }

    #region Event System behaviours
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

        TextMeshProUGUI textToSet = iconParent.GetComponentInChildren<TextMeshProUGUI>();
        Sprite spriteToSend = null;

        //ACTIVATE OR DEACTIVATE ICON'S PARENT
        if(ContainedAbility.Ability.ZoneType || ContainedAbility.Ability.AimedType || ContainedAbility.Ability.TargetedType)
        {
            if (!iconParent.activeInHierarchy) iconParent.SetActive(true);
        }
        else if (ContainedAbility.Ability.NoneType) iconParent.SetActive(false);

        //ZONE
        if (ContainedAbility.Ability.ZoneType)
        {
            spriteToSend = zoneType;
            textToSet.SetText("Sort de zone");
        }
        //AIMED
        else if (ContainedAbility.Ability.AimedType)
        {
            spriteToSend = aimedType;
            textToSet.SetText("Sort Visé");
        }
        //TARGETED
        else if (ContainedAbility.Ability.TargetedType)
        {
            spriteToSend = targetedType;
            textToSet.SetText("Sort Ciblé");
        }
        //BUFF
        else if (ContainedAbility.Ability.IsABuff)
        {
                spriteToSend = buffType;
                textToSet.SetText("Buff");
        }
        //Heal
        else if (ContainedAbility.Ability.AbilityCanHealTarget)
        {
            spriteToSend = healType;
            textToSet.SetText("Soin");
        }

        SetAbilityCooldown();

        //SETTING THE TOOLTIP WITH THE WANTED INFORMATIONS
        tooltipSetter.SetTooltip(
            ContainedAbility.Ability.AbilityName,
            ContainedAbility.Ability.AbilityDescription,
            storedCooldown.ToString("0" + "s"),
            spriteToSend);
    }
    #endregion

    void SetAbilityCooldown()
    {
        storedCooldown = ContainedAbility.Ability.AbilityCooldown;

        if (ContainedAbility.Stats.GetStat(StatType.Cooldown_Reduction).Value > 0)
            storedCooldown -= ContainedAbility.Ability.AbilityCooldown * (ContainedAbility.Stats.GetStat(StatType.Cooldown_Reduction).Value / 100);

        else if (ContainedAbility.Stats.GetStat(StatType.Cooldown_Reduction).Value == 0)
            storedCooldown = ContainedAbility.Ability.AbilityCooldown;
    }
}