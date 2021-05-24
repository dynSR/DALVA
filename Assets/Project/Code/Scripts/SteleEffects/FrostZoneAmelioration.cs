using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostZoneAmelioration : SteleAmelioration
{
    [SerializeField] private FrostZone frostZone;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // reductions +5%
                frostZone.MovementSpeedReduction = 0.2f;
                frostZone.AttackSpeedReduction = 0.2f;

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // reductions +10%
                frostZone.MovementSpeedReduction = 0.3f;
                frostZone.AttackSpeedReduction = 0.3f;

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // reductions +15%
                frostZone.MovementSpeedReduction = 0.45f;

                Debug.Log("UPGRADE 3");
                break;
        }
    }
}