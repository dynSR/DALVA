using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Mage_R : AbilityLogic
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesFirstAbility", true, true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                //Lance un projectile qui inflige(50 + 50 % PM) dégâts magiques. Fait(30 % PM) dégâts supplémentaires si l'ennemi est marqué (consomme la marque).
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityDuration, ThrowingProjectile.AimProjectileEmiterPos, Ability));
                break;
            case AbilityEffect.II:
                //Traverse les unités
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityDuration, ThrowingProjectile.AimProjectileEmiterPos, Ability, true));
                break;
            case AbilityEffect.III:
                //Si l'ennemi touché est marqué, le projectile rebondit sur les ennemis proches (un projectile pour chaque ennemi proche).
                StartCoroutine(ThrowingProjectile.ThrowProjectile(Ability.AbilityEffectObject, Ability.AbilityDuration, ThrowingProjectile.AimProjectileEmiterPos, Ability, false, true));
                break;
            case AbilityEffect.IV:
                //Le Z marque aussi les alliés.
                //Peut toucher les alliés marqués pour les soigner de(25 + 50 % PM) points de vie(PV), se propage aux alliés proches.
                break;
        }
    }
}