using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DashLogic))]
public class Ability_Warrior_E : AbilityLogic
{
    private DashLogic DashLogic => GetComponent<DashLogic>();

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        //Dash
    }
}
