using System.Collections;
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

        StartCoroutine(ApplyDamage(target));
    }

    protected override void RemoveEffect(EntityStats target)
    {
        //NOTHING
    }

    IEnumerator ApplyDamage(EntityStats target)
    {
        yield return new WaitForSeconds(0.1f);

        if (!target.IsDead)
            target.TakeDamage(null, 0, 0, 0, actualDamage, 0, 0, 0, 0);
    }
}
