using UnityEngine;
using TMPro;

public class LeftSectionTooltip : MonoBehaviour
{
    [Header("CONCERNED CHARACTER")]
    [SerializeField] private EntityStats characterStats;

    [Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI attackValueText;
    [SerializeField] private TextMeshProUGUI physicalResistancesValueText;
    [SerializeField] private TextMeshProUGUI magicalResistancesValueText;
    [SerializeField] private TextMeshProUGUI chanceOfCriticalStrikesValueText;
    [SerializeField] private TextMeshProUGUI cooldownReductionValueText;
    [SerializeField] private TextMeshProUGUI movementSpeedValueText;

    private void OnEnable()
    {
        characterStats.OnStatsValueChanged += SetLeftSectionInformations;
    }

    private void OnDisable()
    {
        characterStats.OnStatsValueChanged -= SetLeftSectionInformations;
    }

    void SetLeftSectionInformations(EntityStats stats)
    {
        attackValueText.SetText((stats.GetStat(StatType.PhysicalPower).Value + stats.GetStat(StatType.MagicalPower).Value).ToString("0"));
        physicalResistancesValueText.SetText(stats.GetStat(StatType.PhysicalResistances).Value.ToString("0"));
        magicalResistancesValueText.SetText(stats.GetStat(StatType.MagicalResistances).Value.ToString("0"));
        chanceOfCriticalStrikesValueText.SetText(stats.GetStat(StatType.CriticalStrikeChance).Value.ToString("0"));
        cooldownReductionValueText.SetText(stats.GetStat(StatType.Cooldown_Reduction).Value.ToString("0"));
        movementSpeedValueText.SetText((stats.GetStat(StatType.MovementSpeed).Value * 100).ToString("0"));
    }
}