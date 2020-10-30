using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTest : Ability
{
    private LaunchProjectile launchProjectile;

    protected override void Start()
    {
        base.Start();
        launchProjectile = GetComponent<LaunchProjectile>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        launchProjectile.LaunchAProjectile(AbilityPrefab, launchProjectile.EmmiterPosition);
    }
}
