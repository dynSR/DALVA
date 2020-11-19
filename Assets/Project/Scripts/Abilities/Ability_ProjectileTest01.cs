using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingProjectile))]
public class Ability_ProjectileTest01 : Ability
{
    private ThrowingProjectile ThrowingProjectile => GetComponent<ThrowingProjectile>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        Debug.Log("Cast in Ability01");
        StartCoroutine(ThrowingProjectile.LaunchAProjectile(AbilityPrefab, ThrowingProjectile.AimProjectileEmiterPos, ProjectileType.TravelsForward));
    }
}