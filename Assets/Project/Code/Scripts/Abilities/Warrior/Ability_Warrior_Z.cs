using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Warrior_Z : AbilityLogic
{
    [SerializeField] private float delay;
    [SerializeField] private GameObject damageZone;
    private float shieldValue;

    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesSecondAbility", true);

        StartCoroutine(ApplyShieldOnOneTarget(Stats, Ability.AbilityTimeToCast));

        StartCoroutine(ActivateDamageZone());
    }

    private IEnumerator ActivateDamageZone()
    {
        yield return new WaitForSeconds(delay);

        damageZone.SetActive(true);
        damageZone.GetComponent<Warrior_Z_DamageZone>().SetDamage(CalculateShieldValue());
    }

    private float CalculateShieldValue()
    {
        return shieldValue = (Ability.AbilityShieldValue + (Stats.GetStat(StatType.Health).MaxValue * Ability.ShieldHealthRatio)) * 0.5f;
    }
}