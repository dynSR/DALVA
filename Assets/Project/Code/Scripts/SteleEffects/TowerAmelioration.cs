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
            case SteleLevel.EvolutionI: // 25% puissance, 25% as, proj *2
                Stats.GetStat(StatType.PhysicalPower).AddModifier(new StatModifier(0.25f, StatType.PhysicalPower, StatModType.PercentAdd, this));
                Stats.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(0.25f, StatType.AttackSpeed, StatModType.PercentAdd, this));
                Tower.AmountOfProjectileToSpawn *= 2;

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // 35% puissance, 35% as, proj *2
                Stats.GetStat(StatType.PhysicalPower).AddModifier(new StatModifier(0.35f, StatType.PhysicalPower, StatModType.PercentAdd, this));
                Stats.GetStat(StatType.AttackSpeed).AddModifier(new StatModifier(0.35f, StatType.AttackSpeed, StatModType.PercentAdd, this));
                Tower.AmountOfProjectileToSpawn *= 2;

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution:
                if (FinalEvolutionNumber == 2)
                {
                    Stats.GetStat(StatType.PhysicalPower).AddModifier(new StatModifier(0.45f, StatType.PhysicalPower, StatModType.PercentAdd, this));
                }
                else towerRange.gameObject.SetActive(false);

                Debug.Log("UPGRADE 3");
                break;
        }
    }
}