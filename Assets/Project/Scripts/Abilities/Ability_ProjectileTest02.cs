using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaunchProjectile))]
public class Ability_ProjectileTest02 : Ability
{
    private LaunchProjectile LaunchProjectile => GetComponent<LaunchProjectile>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        LaunchProjectile.TurnCharacterTowardsLaunchDirection();
        StartCoroutine(LaunchProjectile.LaunchAProjectile(AbilityPrefab, LaunchProjectile.EmmiterPosition, ProjectileType.TravelsForward));
    }
}
