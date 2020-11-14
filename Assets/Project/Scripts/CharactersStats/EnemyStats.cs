using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void TakeDamage(float attackDamageTaken, float magicDamageTaken, float criticalStrikeChance, float criticalStrikeMultiplier, float armorPenetration, float magicResistancePenetration)
    {
        base.TakeDamage(attackDamageTaken, magicDamageTaken, criticalStrikeChance, criticalStrikeMultiplier, armorPenetration, magicResistancePenetration);
        Debug.Log("Enemy took damage");
    }
}
