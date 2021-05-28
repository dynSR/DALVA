using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Mage_E : AbilityLogic
{
    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesThirdAbility", true);

        EntityStats abilityTargetStats = AbilityTarget.GetComponent<EntityStats>();
        Collider targetCollider = AbilityTarget.GetComponent<Collider>();

        StartCoroutine(ApplyShieldOnOneTarget(abilityTargetStats, Ability.AbilityTimeToCast));

        if (AbilityTarget.gameObject != transform.gameObject)
            StartCoroutine(ApplyDamageOnTargetWithDelay(Ability.AbilityTimeToCast  + 0.5f, targetCollider));
    }

    IEnumerator ApplyDamageOnTargetWithDelay(float delay, Collider target)
    {
        yield return new WaitForSeconds(delay);

        ApplyingDamageOnTarget(target);
    }
}