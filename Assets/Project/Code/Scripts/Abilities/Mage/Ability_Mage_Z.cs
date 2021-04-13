using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Mage_Z : AbilityLogic
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesSecondAbility", true, true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                //Sort de zone.Après 0,5s, inflige(60 + 50 % PM) dégâts magiques et marque les ennemis pendant 3 secondes.
                break;
            case AbilityEffect.II:
                //Augmente la durée de la marque de 2 secondes
                break;
            case AbilityEffect.III:
                //Si l'ennemi touché est marqué, le projectile rebondit sur les ennemis proches (un projectile pour chaque ennemi proche).
                break;
            case AbilityEffect.IV:
                //Les ennemis marqués infligent 10 % dégâts en moins.
                break;
        }
    }
}