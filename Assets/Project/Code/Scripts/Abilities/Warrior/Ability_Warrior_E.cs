using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Warrior_E : AbilityLogic
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesThirdAbility", true);


    }
}