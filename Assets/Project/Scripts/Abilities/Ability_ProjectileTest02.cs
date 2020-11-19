using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingProjectile))]
public class Ability_ProjectileTest02 : Ability
{
    private ThrowingProjectile ThrowingProjectile => GetComponent<ThrowingProjectile>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        StartCoroutine(ThrowingProjectile.LaunchAProjectile(AbilityPrefab, ThrowingProjectile.AimProjectileEmiterPos, ProjectileType.TravelsForward));
    }
}
