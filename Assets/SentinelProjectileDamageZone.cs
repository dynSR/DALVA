using UnityEngine;

public class SentinelProjectileDamageZone : StatusEffectZoneCore
{
    [SerializeField] private float actualDamage = 35f;
    public ProjectileLogic projectile;
    public Transform projectileTarget;

    protected override void ApplyAffect(EntityStats target)
    {
        if(target.transform != projectileTarget)
        {
            target.GetStat(StatType.Health).Value -= actualDamage;

            if (target.CanTakeDamage)
                StartCoroutine(target.CreateDamagePopUpWithDelay(0.1f, actualDamage, StatType.PhysicalPower, null));
            else if (!target.CanTakeDamage)
            {
                StartCoroutine(target.CreateDamagePopUpWithDelay(0.1f, actualDamage, StatType.PhysicalPower, null, true));
            }
        }
            //target.TakeDamage(null, 0, 0, 0, actualDamage, 0, 0, 0, 0);
    }

    protected override void RemoveEffect(EntityStats target)
    {
        //NOTHING
    }
}
