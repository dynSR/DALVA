using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAmelioration : SteleAmelioration
{
    [SerializeField] private EntityStats guardianStats;
    [SerializeField] private GameObject stunZone;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // Résistances + 10%
                guardianStats.GetStat(StatType.PhysicalResistances).AddModifier(new StatModifier(0.1f, StatType.PhysicalResistances, StatModType.PercentAdd, this));
                guardianStats.GetStat(StatType.MagicalResistances).AddModifier(new StatModifier(0.1f, StatType.MagicalResistances, StatModType.PercentAdd, this));

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // 20% puissance, 20% as
                guardianStats.GetStat(StatType.PhysicalResistances).AddModifier(new StatModifier(0.2f, StatType.PhysicalResistances, StatModType.PercentAdd, this));
                guardianStats.GetStat(StatType.MagicalResistances).AddModifier(new StatModifier(0.2f, StatType.MagicalResistances, StatModType.PercentAdd, this));

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // Stun zone added
                Instantiate(stunZone, guardianStats.transform);

                Debug.Log("UPGRADE 3");
                break;
        }
    }
}
