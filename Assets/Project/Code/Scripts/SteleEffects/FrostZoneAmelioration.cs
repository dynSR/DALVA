using UnityEngine;

public class FrostZoneAmelioration : SteleAmelioration
{
    [SerializeField] private FrostZone frostZone;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // reductions +10%
                frostZone.MovementSpeedReduction = 0.25f;
                frostZone.AttackSpeedReduction = 0.25f;

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // reductions +7.5% + 1 range
                frostZone.MovementSpeedReduction = 0.325f;
                frostZone.AttackSpeedReduction = 0.325f;

                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // damage reduction + 1 range
                frostZone.CanReduceDamage = true;

                transform.localScale = new Vector3(2f, 2f, 2f);

                Debug.Log("UPGRADE 3");
                break;
        }

        frostZone.ResetTrigger();
    }
}