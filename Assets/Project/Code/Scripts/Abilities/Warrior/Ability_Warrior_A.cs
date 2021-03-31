using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingAbilityProjectile))]
public class Ability_Warrior_A : AbilityLogic
{
    private void OnEnable()
    {
        Interactions.OnAttacking += RemoveEffect;
    }

    private void OnDisable()
    {
        Interactions.OnAttacking -= RemoveEffect;
    }


    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        UsedEffectIndex = 0;
        Ability.UsedAbilityEffect = Ability.AbilityEffects[(int)UsedEffectIndex /*int value qui change /rapport au shop*/ ];
        Interactions.ResetInteractionState();
        Controller.CharacterAnimator.SetBool("UsesFirstAbility", true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                AbilityBuff(Stats, StatType.BonusPhysicalPower, 20 + (Stats.GetStat(StatType.PhysicalPower).Value * Ability.AbilityPhysicalRatio), this);
                ActivateVFX(AbilityEffectsToActivate);
                break;
            case AbilityEffect.II:
                break;
            case AbilityEffect.III:
                break;
            case AbilityEffect.IV:
                break;
        }

        Controller.CharacterAnimator.SetBool("UsesFirstAbility", false);
    }

    void RemoveEffect()
    {
        RemoveAbilityBuff(StatType.BonusPhysicalPower, this);
        DeactivateVFX(AbilityEffectsToActivate);
    }
}