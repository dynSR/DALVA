using UnityEngine;

public class SentinelProjectileDamageZone : StatusEffectZoneCore
{
    [SerializeField] private float actualDamage = 35f;
    public ProjectileLogic projectile;
    public Transform projectileTarget;

    protected override void ApplyAffect(EntityStats target)
    {
        if(target.transform != projectileTarget)
            target.TakeDamage(null, 0, 0, 0, actualDamage, 0, 0, 0, 0);
    }

    protected override void RemoveEffect(EntityStats target)
    {
        //NOTHING
    }
}
