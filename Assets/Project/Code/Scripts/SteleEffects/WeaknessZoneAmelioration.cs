using UnityEngine;

public class WeaknessZoneAmelioration : SteleAmelioration
{
    [SerializeField] private WeaknessZone weaknessZone;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // reductions +5%
                weaknessZone.PhysicalResistancesReduction = 0.25f;
                weaknessZone.MagicalResistancesReduction = 0.25f;

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // reductions +15%
                weaknessZone.PhysicalResistancesReduction = 0.40f;
                weaknessZone.MagicalResistancesReduction = 0.40f;

                weaknessZone.AugmentZoneRange(1.5f);

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // increased damage taken * 15%
                weaknessZone.CanApplyIncreasedDamageTaken = true;

                weaknessZone.AugmentZoneRange(2f);

                Debug.Log("UPGRADE 3");
                break;
        }

        weaknessZone.ResetTrigger();
    }
}
