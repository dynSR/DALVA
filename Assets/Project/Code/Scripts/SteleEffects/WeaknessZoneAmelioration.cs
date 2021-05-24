using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessZoneAmelioration : SteleAmelioration
{
    [SerializeField] private WeaknessZone weaknessZone;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // reductions +5%
                weaknessZone.PhysicalResistancesReduction = 0.2f;
                weaknessZone.MagicalResistancesReduction = 0.2f;

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // reductions +10%
                weaknessZone.PhysicalResistancesReduction = 0.3f;
                weaknessZone.MagicalResistancesReduction = 0.3f;

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // reductions +25%
                weaknessZone.PhysicalResistancesReduction = 0.55f;
                weaknessZone.MagicalResistancesReduction = 0.55f;

                Debug.Log("UPGRADE 3");
                break;
        }
    }
}
