using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Warrior_Z : AbilityLogic
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, ThrowingProjectile.AimProjectileEmiterPos, Ability));
    }
}
