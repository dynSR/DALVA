
public class Ability_Warrior_R : AbilityLogic
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        //if (Stats.GetStat(StatType.Health).Value == Stats.GetStat(StatType.Health).MaxValue)
        //{
        //    return;
        //}

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                PlayAbilityAnimation("UsesFourthAbility", true);
                Stats.Heal(transform, 75f + (75 * (Stats.GetStat(StatType.PhysicalResistances).Value * 0.075f)), Stats.GetStat(StatType.HealAndShieldEffectiveness).Value);
                break;
        }
    }
}