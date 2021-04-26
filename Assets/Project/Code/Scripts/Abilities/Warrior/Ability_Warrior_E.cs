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
        PlayAbilityAnimation("UsesThirdAbility", true, true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
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
        throw new System.NotImplementedException();
    }

    protected override void ResetAbilityAttributes()
    {

    }
}