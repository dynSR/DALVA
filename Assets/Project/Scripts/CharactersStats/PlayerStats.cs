using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
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

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    //Debug.Log("Take Damage");
        //    TakeDamage(200, 150);
        //    Debug.Log(CurrentHealth);
        //}
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void TakeDamage(float attackDamageTaken, float magicDamageTaken, float criticalStrikeChance, float criticalStrikeMultiplier, float armorPenetration, float magicResistancePenetration)
    {
        base.TakeDamage(attackDamageTaken, magicDamageTaken, criticalStrikeChance, criticalStrikeMultiplier, armorPenetration, magicResistancePenetration);
    }
}
