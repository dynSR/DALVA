using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingProjectile))]
public class Ability_ProjectileTest04 : AbilityLogic
{
    private ThrowingProjectile ThrowingProjectile => GetComponent<ThrowingProjectile>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        StartCoroutine(ThrowingProjectile.LaunchAProjectile(Ability.AbilityProjectilePrefab, ThrowingProjectile.AimProjectileEmiterPos, Ability));
    }
}
