public class Ability_Mage_A : AbilityLogic
{
    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesFirstAbility", true);

        //Si l'ennemi touché est marqué, le projectile rebondit sur les ennemis proches (un projectile pour chaque ennemi proche).
        Ability.AbilityDamageBonusOnMarkedTarget = 0.3f;
        StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityTimeToCast, ThrowingProjectile.AimProjectileEmiterPos, Ability, false, true));
    }
}