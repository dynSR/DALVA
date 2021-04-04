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
        UsedEffectIndex = AbilityEffect.I; //debug

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                PlayAbilityAnimation("UsesSecondAbility", true, true);
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, ThrowingProjectile.AimProjectileEmiterPos, Ability));
                ResetAbilityAnimation("UsesSecondAbility");
                break;
            case AbilityEffect.II:
                break;
            case AbilityEffect.III:
                break;
            case AbilityEffect.IV:
                break;
        }
    }
}