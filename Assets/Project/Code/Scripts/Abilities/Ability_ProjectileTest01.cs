using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingProjectile))]
public class Ability_ProjectileTest01 : AbilityLogic
{
    private ThrowingProjectile ThrowingProjectile => GetComponent<ThrowingProjectile>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        Debug.Log("Cast in Ability01");
        StartCoroutine(ThrowingProjectile.LaunchAProjectile(Ability.AbilityProjectilePrefab, ThrowingProjectile.AimProjectileEmiterPos, Ability));
    }
}