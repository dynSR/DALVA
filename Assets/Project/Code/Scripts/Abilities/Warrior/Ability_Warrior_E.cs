using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingAbilityProjectile))]
public class Ability_Warrior_E : AbilityLogic
{
    private ThrowingAbilityProjectile ThrowingProjectile => GetComponent<ThrowingAbilityProjectile>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityProjectilePrefab, ThrowingProjectile.AimProjectileEmiterPos, Ability));
    }
}
