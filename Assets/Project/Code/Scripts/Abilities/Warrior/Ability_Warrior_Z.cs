using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingAbilityProjectile))]
public class Ability_Warrior_Z : AbilityLogic
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        Ability.AbilityEffects[0].ApplyEffect(transform);
    }
}
