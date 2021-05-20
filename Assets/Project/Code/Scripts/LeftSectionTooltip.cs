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
        chanceOfCriticalStrikesValueText.SetText(stats.GetStat(StatType.CriticalStrikeChance).Value.ToString("0") + "%");
        cooldownReductionValueText.SetText(stats.GetStat(StatType.Cooldown_Reduction).Value.ToString("0"));
        movementSpeedValueText.SetText((stats.GetStat(StatType.MovementSpeed).Value * 10).ToString("0"));
        penetrationValueText.SetText((stats.GetStat(StatType.PhysicalPenetration).Value + stats.GetStat(StatType.MagicalPenetration).Value).ToString("0") + "%");
        lifeStealValueText.SetText((stats.GetStat(StatType.PhysicalLifesteal).Value + stats.GetStat(StatType.MagicalLifesteal).Value).ToString("0") + "%");

        //Trigger animation
        Animator attachedAnimator = GetComponent<Animator>();
        attachedAnimator.SetTrigger("TriggerFeedback");
    }
}