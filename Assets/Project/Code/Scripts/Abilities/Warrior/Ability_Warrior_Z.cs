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
                break;
            case AbilityEffect.II:
                break;
            case AbilityEffect.III:
                break;
            case AbilityEffect.IV:
                break;
        }
    }

    public override void SetAbilityAfterAPurchase()
    {
        base.SetAbilityAfterAPurchase();
    }

    protected override void ResetAbilityAttributes()
    {

    }
}