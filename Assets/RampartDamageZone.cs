using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampartDamageZone : StatusEffectZoneCore
{
    [SerializeField] private float damagePerSecond = 4f;
    [SerializeField] private float interval = 0.4f;
    public GameObject rotatingProjectiles;

    public float DamagePerSecond { get => damagePerSecond; set => damagePerSecond = value; }
    public float Interval { get => interval; set => interval = value; }

    private void Start()
    {
        InvokeRepeating("ApplyDamageOverTime", 1, Interval);
    }

    protected override void ApplyAffect(EntityStats target)
    {
        target.Controller.ActivateBurnVFX();
        //target.TakeDamage(null, 0, 0 /*statsOfEntitiesInTrigger[i].GetStat(StatType.MagicalResistances).Value*/, 0, damagePerSecond, 0, 0, 0, 0);
    }

    void ApplyDamageOverTime()
    {
        for (int i = 0; i < statsOfEntitiesInTrigger.Count; i++)
        {
            Debug.Log("0000");

            if(statsOfEntitiesInTrigger[i].IsDead)
            {
                statsOfEntitiesInTrigger.Remove(statsOfEntitiesInTrigger[i]);
                return;
            }

            statsOfEntitiesInTrigger[i].TakeDamage(null, 0, 0 /*statsOfEntitiesInTrigger[i].GetStat(StatType.MagicalResistances).Value*/, 0, damagePerSecond, 0, 0, 0, 0);
        }
    }

    protected override void RemoveEffect(EntityStats target)
    {
        target.Controller.DeactivateBurnVFX();
    }
}
