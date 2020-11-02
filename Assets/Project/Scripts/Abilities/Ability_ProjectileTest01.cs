using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaunchProjectile))]
public class Ability_ProjectileTest01 : Ability
{
    private LaunchProjectile LaunchProjectile => GetComponent<LaunchProjectile>();


    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        LaunchProjectile.LaunchAProjectile(AbilityPrefab, LaunchProjectile.EmmiterPosition);
    }
}
