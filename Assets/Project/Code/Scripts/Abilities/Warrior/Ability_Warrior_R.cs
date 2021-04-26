using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingAbilityProjectile))]
public class Ability_Warrior_R : AbilityLogic
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
                PlayAbilityAnimation("UsesFourthAbility");
                Stats.Heal(transform, 450, Stats.GetStat(StatType.MagicalPower).Value);
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