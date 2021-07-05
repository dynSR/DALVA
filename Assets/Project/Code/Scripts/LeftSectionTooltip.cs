using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class LeftSectionTooltip : MonoBehaviour
{
    [Header("CONCERNED CHARACTER")]
    [SerializeField] private EntityStats characterStats;

    [Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI attackValueText;
    [SerializeField] private TextMeshProUGUI attackSpeedValueText;
    [SerializeField] private TextMeshProUGUI resistances;
    [SerializeField] private TextMeshProUGUI chanceOfCriticalStrikesValueText;
    [SerializeField] private TextMeshProUGUI cooldownReductionValueText;
    [SerializeField] private TextMeshProUGUI movementSpeedValueText;
    [SerializeField] private TextMeshProUGUI penetrationValueText;
    [SerializeField] private TextMeshProUGUI lifeStealValueText;
    [SerializeField] private TextMeshProUGUI maxLifeValueText;

    [Header("TEXT COLORS")]
    public Color defaultColor;
    public Color cappedColor;

    [Header("FEEDBACK")]
    public GameObject highlightObject;

    public EntityStats CharacterStats { get => characterStats; }

    private void OnEnable()
    {
        CharacterStats.OnStatsValueChanged += SetLeftSectionInformations;
    }

    private void OnDisable()
    {
        CharacterStats.OnStatsValueChanged -= SetLeftSectionInformations;
    }

    void SetLeftSectionInformations(EntityStats stats)
    {
        #region Update text value feedback
        //Set parsed values here
        if (maxLifeValueText)
            UseTheRightAnimatorTrigger(maxLifeValueText, ParseAndTrimFloat(maxLifeValueText.text), stats.GetStat(StatType.Health).MaxValue);

        UseTheRightAnimatorTrigger(attackValueText, ParseAndTrimFloat(attackValueText.text), (stats.GetStat(StatType.PhysicalPower).Value + stats.GetStat(StatType.MagicalPower).Value));

        UseTheRightAnimatorTrigger(attackSpeedValueText, ParseAndTrimFloat(stats.GetStat(StatType.AttackSpeed).Value.ToString()), stats.GetStat(StatType.AttackSpeed).Value);

        UseTheRightAnimatorTrigger(resistances, ParseAndTrimFloat(resistances.text), (stats.GetStat(StatType.PhysicalResistances).Value + stats.GetStat(StatType.MagicalResistances).Value));

        UseTheRightAnimatorTrigger(chanceOfCriticalStrikesValueText, ParseAndTrimFloat(chanceOfCriticalStrikesValueText.text), stats.GetStat(StatType.CriticalStrikeChance).Value);

        UseTheRightAnimatorTrigger(cooldownReductionValueText, ParseAndTrimFloat(cooldownReductionValueText.text), stats.GetStat(StatType.Cooldown_Reduction).Value);

        UseTheRightAnimatorTrigger(movementSpeedValueText, ParseAndTrimFloat(movementSpeedValueText.text), (stats.GetStat(StatType.MovementSpeed).Value * 10));

        UseTheRightAnimatorTrigger(penetrationValueText, ParseAndTrimFloat(penetrationValueText.text), (stats.GetStat(StatType.PhysicalPenetration).Value + stats.GetStat(StatType.MagicalPenetration).Value));

        UseTheRightAnimatorTrigger(lifeStealValueText, ParseAndTrimFloat(lifeStealValueText.text), (stats.GetStat(StatType.PhysicalLifesteal).Value + stats.GetStat(StatType.MagicalLifesteal).Value));
        #endregion

        #region Set new text value
        if (maxLifeValueText)
            maxLifeValueText.SetText("Vie max - " + stats.GetStat(StatType.Health).MaxValue);

        attackValueText.SetText((stats.GetStat(StatType.PhysicalPower).Value + stats.GetStat(StatType.MagicalPower).Value).ToString("0"));

        attackSpeedValueText.SetText(stats.GetStat(StatType.AttackSpeed).Value.ToString("0.00"));

        resistances.SetText((stats.GetStat(StatType.PhysicalResistances).Value + stats.GetStat(StatType.MagicalResistances).Value).ToString("0"));

        //CC MAX - REACHED
        if (stats.GetStat(StatType.CriticalStrikeChance).Value >= stats.GetStat(StatType.CriticalStrikeChance).CapValue)
        {
            if (chanceOfCriticalStrikesValueText.color != cappedColor)
                chanceOfCriticalStrikesValueText.color = cappedColor;

            chanceOfCriticalStrikesValueText.SetText(stats.GetStat(StatType.CriticalStrikeChance).Value.ToString("0")
            + " /"
            + " "
            + stats.GetStat(StatType.CriticalStrikeChance).CapValue.ToString("0"));
        }
        //CC MAX - NOT REACHED
        else if (stats.GetStat(StatType.CriticalStrikeChance).Value < stats.GetStat(StatType.CriticalStrikeChance).CapValue)
        {
            if (chanceOfCriticalStrikesValueText.color != defaultColor)
                chanceOfCriticalStrikesValueText.color = defaultColor;

            chanceOfCriticalStrikesValueText.SetText(stats.GetStat(StatType.CriticalStrikeChance).Value.ToString("0")
            + " /"
            + " "
            + stats.GetStat(StatType.CriticalStrikeChance).CapValue.ToString("0"));
        }

        //CDR MAX - REACHED
        if (stats.GetStat(StatType.Cooldown_Reduction).Value >= stats.GetStat(StatType.Cooldown_Reduction).CapValue)
        {
            if(cooldownReductionValueText.color != cappedColor)
                cooldownReductionValueText.color = cappedColor;

            cooldownReductionValueText.SetText(stats.GetStat(StatType.Cooldown_Reduction).Value.ToString("0")
                + " /"
                + " "
                + stats.GetStat(StatType.Cooldown_Reduction).CapValue.ToString("0"));
        }
        //CDR MAX - NOT REACHED
        else if(stats.GetStat(StatType.Cooldown_Reduction).Value < stats.GetStat(StatType.Cooldown_Reduction).CapValue)
        {
            if (cooldownReductionValueText.color != defaultColor)
                cooldownReductionValueText.color = defaultColor;

            cooldownReductionValueText.SetText(stats.GetStat(StatType.Cooldown_Reduction).Value.ToString("0")
                + " / "
                + " "
                + stats.GetStat(StatType.Cooldown_Reduction).CapValue.ToString("0"));
        }

        movementSpeedValueText.SetText((stats.GetStat(StatType.MovementSpeed).Value * 10).ToString("0"));

        penetrationValueText.SetText((stats.GetStat(StatType.PhysicalPenetration).Value + stats.GetStat(StatType.MagicalPenetration).Value).ToString("0") + "%");

        lifeStealValueText.SetText((stats.GetStat(StatType.PhysicalLifesteal).Value + stats.GetStat(StatType.MagicalLifesteal).Value).ToString("0") + "%");
        #endregion

        //Trigger animation
        Animator attachedAnimator = GetComponent<Animator>();
        attachedAnimator.SetTrigger("TriggerFeedback");
    }

    
    public void DisplayHighlightObject()
    {
        if(highlightObject)
            highlightObject.SetActive(true);
    }

    public void HideHighlightObject ()
    {
        if (highlightObject)
            highlightObject.SetActive(false);
    }

    private void UseTheRightAnimatorTrigger (TextMeshProUGUI parsedString, double currentValue, float valueCompared)
    {
        if (valueCompared == currentValue) return;

        Animator animator = parsedString.transform.GetComponent<Animator>();

        if (valueCompared < currentValue)
        {
            Debug.Log("attack stat removed");

            if (animator)
            {
                animator.ResetTrigger("Remove");
                animator.SetTrigger("Remove");
            }
                
        }
        else if (valueCompared > currentValue)
        {
            Debug.Log("attack stat added");

            if (animator)
            {
                animator.ResetTrigger("Add");
                animator.SetTrigger("Add");
            }  
        }
    }

    private float ParseAndTrimFloat(string stringBuffer)
    {
        Debug.Log(stringBuffer);

        float floatValue;

        char [ ] charsToTrim = { '%', };
        string result = stringBuffer.Trim(charsToTrim);

        if (stringBuffer.ToLower().Contains('-'))
        {
            floatValue = float.Parse(result.Split('-').Last());
        }
        else
        {
            floatValue = float.Parse(result.Split('/').First());
        }

        return floatValue;
    }
}