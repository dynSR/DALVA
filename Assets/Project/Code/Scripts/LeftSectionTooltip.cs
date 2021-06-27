using UnityEngine;
using TMPro;

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

    [Header("TEXT COLORS")]
    public Color defaultColor;
    public Color cappedColor;

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
        attackValueText.SetText((stats.GetStat(StatType.PhysicalPower).Value + stats.GetStat(StatType.MagicalPower).Value).ToString("0"));

        attackSpeedValueText.SetText(stats.GetStat(StatType.AttackSpeed).Value.ToString("0.0"));

        resistances.SetText((stats.GetStat(StatType.PhysicalResistances).Value + stats.GetStat(StatType.MagicalResistances).Value).ToString("0"));

        //CC MAX - REACHED
        if (stats.GetStat(StatType.CriticalStrikeChance).Value >= stats.GetStat(StatType.CriticalStrikeChance).CapValue)
        {
            if (chanceOfCriticalStrikesValueText.color != cappedColor)
                chanceOfCriticalStrikesValueText.color = cappedColor;

            chanceOfCriticalStrikesValueText.SetText(stats.GetStat(StatType.CriticalStrikeChance).Value.ToString("0")
                + " / "
                + stats.GetStat(StatType.CriticalStrikeChance).CapValue.ToString("0"));
        }
        //CC MAX - NOT REACHED
        else if (stats.GetStat(StatType.CriticalStrikeChance).Value < stats.GetStat(StatType.CriticalStrikeChance).CapValue)
        {
            if (chanceOfCriticalStrikesValueText.color != defaultColor)
                chanceOfCriticalStrikesValueText.color = defaultColor;

            chanceOfCriticalStrikesValueText.SetText(stats.GetStat(StatType.CriticalStrikeChance).Value.ToString("0")
                + " / "
                + stats.GetStat(StatType.CriticalStrikeChance).CapValue.ToString("0"));
        }

        //CDR MAX - REACHED
        if (stats.GetStat(StatType.Cooldown_Reduction).Value >= stats.GetStat(StatType.Cooldown_Reduction).CapValue)
        {
            if(cooldownReductionValueText.color != cappedColor)
                cooldownReductionValueText.color = cappedColor;

            cooldownReductionValueText.SetText(stats.GetStat(StatType.Cooldown_Reduction).Value.ToString("0")
                + " / "
                + stats.GetStat(StatType.Cooldown_Reduction).CapValue.ToString("0"));
        }
        //CDR MAX - NOT REACHED
        else if(stats.GetStat(StatType.Cooldown_Reduction).Value < stats.GetStat(StatType.Cooldown_Reduction).CapValue)
        {
            if (cooldownReductionValueText.color != defaultColor)
                cooldownReductionValueText.color = defaultColor;

            cooldownReductionValueText.SetText(stats.GetStat(StatType.Cooldown_Reduction).Value.ToString("0")
                + " / "
                + stats.GetStat(StatType.Cooldown_Reduction).CapValue.ToString("0"));
        }

        movementSpeedValueText.SetText((stats.GetStat(StatType.MovementSpeed).Value * 10).ToString("0"));

        penetrationValueText.SetText((stats.GetStat(StatType.PhysicalPenetration).Value + stats.GetStat(StatType.MagicalPenetration).Value).ToString("0") + "%");

        lifeStealValueText.SetText((stats.GetStat(StatType.PhysicalLifesteal).Value + stats.GetStat(StatType.MagicalLifesteal).Value).ToString("0") + "%");

        //Trigger animation
        Animator attachedAnimator = GetComponent<Animator>();
        attachedAnimator.SetTrigger("TriggerFeedback");
    }
}