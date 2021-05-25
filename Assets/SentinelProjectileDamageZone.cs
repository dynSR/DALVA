using UnityEngine;

public class SentinelProjectileDamageZone : StatusEffectZoneCore
{
    [SerializeField] private float actualDamage = 35f;
    public ProjectileLogic projectile;

    protected override void ApplyAffect(EntityStats target)
    {
        target.TakeDamage(null, 0, 0, 0, actualDamage, 0, 0, 0, 0);
    }

    protected override void RemoveEffect(EntityStats target)
    {
        //NOTHING
    }
}
