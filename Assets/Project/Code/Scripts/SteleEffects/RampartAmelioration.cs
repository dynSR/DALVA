using UnityEngine;
using UnityEngine.AI;

public class RampartAmelioration : SteleAmelioration
{
    [SerializeField] private RampartDamageZone rampartDamageZone;

    public override void UpgradeEffect()
    {
        switch (Stele.SteleLevel)
        {
            case SteleLevel.EvolutionI: // reductions +10%
                rampartDamageZone.DamagePerSecond *= 1.5f;

                Debug.Log("UPGRADE 1");
                break;
            case SteleLevel.EvolutionII: // reductions +7.5% + 1 range
                rampartDamageZone.DamagePerSecond *= 2f;

                Debug.Log("UPGRADE 2");
                break;
            case SteleLevel.FinalEvolution: // rotating projectiles / 20% réduction délai d'application des dégâts
                rampartDamageZone.rotatingProjectiles.SetActive(true);
                rampartDamageZone.Interval = 0.35f;

                Debug.Log("UPGRADE 3");
                break;
        }

        rampartDamageZone.ResetTrigger();
    }
}