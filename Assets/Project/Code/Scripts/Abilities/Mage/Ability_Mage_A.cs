public class Ability_Mage_A : AbilityLogic
{
    protected override void Awake() => base.Awake();

    protected override void OnEnable() => base.OnEnable();

    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesFirstAbility", true, true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                //Lance un projectile qui inflige(50 + 50 % PM) dégâts magiques. Fait(30 % PM) dégâts supplémentaires si l'ennemi est marqué (consomme la marque).
                //Calcul des dégâts bonus
                Ability.AbilityDamageBonusOnMarkedTarget = 0.3f;
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityTimeToCast, ThrowingProjectile.AimProjectileEmiterPos, Ability));
                break;
            case AbilityEffect.II:
                //Traverse les unités
                Ability.AbilityDamageBonusOnMarkedTarget = 0;
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityTimeToCast, ThrowingProjectile.AimProjectileEmiterPos, Ability, true));
                break;
            case AbilityEffect.III:
                //Si l'ennemi touché est marqué, le projectile rebondit sur les ennemis proches (un projectile pour chaque ennemi proche).
                Ability.AbilityDamageBonusOnMarkedTarget = 0.3f;
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityTimeToCast, ThrowingProjectile.AimProjectileEmiterPos, Ability, false, true));
                break;
            case AbilityEffect.IV:
                //Le Z marque aussi les alliés.
                //Peut toucher les alliés marqués pour les soigner de(25 + 50 % PM) points de vie(PV), se propage aux alliés proches.
                Ability.AbilityDamageBonusOnMarkedTarget = 0;
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityTimeToCast, ThrowingProjectile.AimProjectileEmiterPos, Ability, false, true, true));
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