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
        UsedEffectIndex = AbilityEffect.I; //debug

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                PlayAbilityAnimation("UsesFirstAbility", true);
                AbilityBuff(Stats, StatType.BonusPhysicalPower, 20 + (Stats.GetStat(StatType.PhysicalPower).Value * Ability.AbilityPhysicalRatio), this);
                ActivateVFX(AbilityVFXToActivate);
                ResetAbilityAnimation("UsesFirstAbility");
                break;
            case AbilityEffect.II:
                break;
            case AbilityEffect.III:
                break;
            case AbilityEffect.IV:
                break;
        }
    }

    void RemoveEffect()
    {
        RemoveAbilityBuff(StatType.BonusPhysicalPower, this);
        DeactivateVFX(AbilityVFXToActivate);
    }
}