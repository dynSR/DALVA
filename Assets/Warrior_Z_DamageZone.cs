using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior_Z_DamageZone : MonoBehaviour
{
    [SerializeField] private EntityStats characterStats;
    private float damage;

    private void OnTriggerEnter(Collider other)
    {
        ApplyAffect(other.GetComponent<EntityStats>());
    }

    private void ApplyAffect(EntityStats target)
    {
        if (target != null
            && !target.IsDead
            && (target.EntityTeam == EntityTeam.HULRYCK || target.EntityTeam == EntityTeam.NEUTRAL))
        {
            target.TakeDamage(characterStats.transform, target.GetStat(StatType.PhysicalResistances).Value, 0, damage, 0, 0, 0, characterStats.GetStat(StatType.PhysicalPenetration).Value, 0);
        }
    }

    public float SetDamage(float value)
    {
        return damage = value;
    }
}