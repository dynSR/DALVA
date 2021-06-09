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
                AbilityGivesABuff(Stats, StatType.BonusPhysicalPower, 20 + (Stats.GetStat(StatType.MagicalPower).Value * Ability.AbilityPhysicalRatio), this);
                ActivateVFX(AbilityVFXToActivate);
                break;
        }
    }

    void RemoveEffect()
    {
        Debug.Log("RemoveEffect");
        RemoveBuffGivenByAnAbility(StatType.BonusPhysicalPower, this);
        DeactivateVFX(AbilityVFXToActivate);
    }
}