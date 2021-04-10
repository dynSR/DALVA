using UnityEngine;

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
        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                PlayAbilityAnimation("UsesFirstAbility", true);
                Debug.Log("using first ability");
                AbilityGivesABuff(Stats, StatType.BonusPhysicalPower, 20 + (Stats.GetStat(StatType.PhysicalPower).Value * Ability.AbilityPhysicalRatio), this);
                ActivateVFX(AbilityVFXToActivate);
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
        RemoveBuffGivenByAnAbility(StatType.BonusPhysicalPower, this);
        DeactivateVFX(AbilityVFXToActivate);
    }
}