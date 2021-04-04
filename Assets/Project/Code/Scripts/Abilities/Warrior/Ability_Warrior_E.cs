using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Warrior_E : AbilityLogic
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
                PlayAbilityAnimation("UsesThirdAbility", true, true);
                ApplyAbilityAtLocation(CastLocation, Ability.AbilityEffectObject);
                ResetAbilityAnimation("UsesThirdAbility");
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