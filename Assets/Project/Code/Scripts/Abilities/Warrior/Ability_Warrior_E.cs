﻿using DarkTonic.MasterAudio;
using System.Collections;
using UnityEngine;

public class Ability_Warrior_E : AbilityLogic
{
    [SerializeField] private float delay;
    [SerializeField] private GameObject damageZone;
    private float damageValue;

    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesThirdAbility", true);

        StartCoroutine(ActivateDamageZone());
    }

    private IEnumerator ActivateDamageZone()
    {
        yield return new WaitForSeconds(delay);

        damageZone.SetActive(true);
        damageZone.GetComponent<Warrior_Z_DamageZone>().SetDamage(CalculateDamageValue());
    }

    private float CalculateDamageValue()
    {
        return damageValue = (Ability.AbilityPhysicalDamage + (Stats.GetStat(StatType.MagicalPower).Value * Ability.AbilityPhysicalRatio));
    }
}