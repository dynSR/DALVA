using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAmelioration : SteleAmelioration
{
    Tower Tower => GetComponent<Tower>();
    EntityStats Stats => GetComponent<EntityStats>();

    [SerializeField] private TowerRange towerRange;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // 15% puissance, 15% as
                Stats.GetStat(StatType.PhysicalPower).AddModifier(new StatModifier(0.15f, StatType.PhysicalPower, StatModType.PercentAdd, this));
                Stats.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(0.15f, StatType.AttackSpeed, StatModType.PercentAdd, this));

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // 20% puissance, 20% as
                Stats.GetStat(StatType.PhysicalPower).AddModifier(new StatModifier(0.2f, StatType.PhysicalPower, StatModType.PercentAdd, this));
                Stats.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(0.2f, StatType.AttackSpeed, StatModType.PercentAdd, this));

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // 35% puissance, 35% as
                Stats.GetStat(StatType.PhysicalPower).AddModifier(new StatModifier(0.35f, StatType.PhysicalPower, StatModType.PercentAdd, this));
                Stats.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(0.45f, StatType.AttackSpeed, StatModType.PercentAdd, this));

                Debug.Log("UPGRADE 3");
                break;
        }
    }
}