using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldZone : MonoBehaviour
{
    public SphereCollider AttachedSphereCollider => GetComponent<SphereCollider>();

    public AbilityLogic UsedAbility { get; set; }
    public StatusEffect ShieldStatusEffectToApply { get; set; }
    public float BuffDuration { get; set; }
    public float ShieldValue { get; set; }
    public float ShieldEffectivenessValue { get; set; }
    public bool ScaleWithShieldEffectiveness { get; set; }
    public bool OnlyApplyOnMarkedTarget { get; set; }

    private void OnEnable()
    {
        AttachedSphereCollider.enabled = true;
    }

    private void GiveShieldToNearestTargets(Collider target,
        StatusEffect shieldStatusEffectToApply,
        float buffDuration,
        float shieldValue,
        float shieldEffectiveness,
        bool scaleWithShieldEffectiveness = false,
        bool onlyApplyOnMarkedTarget = false)
    {
        Debug.Log("Gives Shield");

        EntityStats entityStats = target.GetComponent<EntityStats>();

        Debug.Log("Enter Function");

        if (onlyApplyOnMarkedTarget && !entityStats.EntityIsMarked) return;

        shieldStatusEffectToApply.StatModifiers[0].Value = shieldValue;
        shieldStatusEffectToApply.StatusEffectDuration = buffDuration;

        if (scaleWithShieldEffectiveness) shieldEffectiveness = ShieldEffectivenessValue;


        shieldStatusEffectToApply.ApplyEffect(entityStats.transform);
        entityStats.ApplyShieldOnTarget(entityStats.transform, 0, shieldEffectiveness);

        if (onlyApplyOnMarkedTarget && entityStats.EntityIsMarked) entityStats.EntityIsMarked = false;
    }

    public void SetShieldZone(
        float triggerSize,
        AbilityLogic usedAbility,
        StatusEffect shieldStatusEffectToApply,
        float buffDuration,
        float shieldValue,
        float shieldEffectiveness,
        bool scaleWithShieldEffectiveness,
        bool onlyApplyOnMarkedTarget)
    {
        AttachedSphereCollider.radius = triggerSize;
        UsedAbility = usedAbility;
        ShieldStatusEffectToApply = shieldStatusEffectToApply;
        BuffDuration = buffDuration;
        ShieldValue = shieldValue;
        ShieldEffectivenessValue = shieldEffectiveness;
        ScaleWithShieldEffectiveness = scaleWithShieldEffectiveness;
        OnlyApplyOnMarkedTarget = onlyApplyOnMarkedTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && !entityStats.IsDead && UsedAbility.AbilityTarget.gameObject != other.gameObject)
        {
            GiveShieldToNearestTargets(other, ShieldStatusEffectToApply, BuffDuration, ShieldValue, ShieldEffectivenessValue, ScaleWithShieldEffectiveness, OnlyApplyOnMarkedTarget);
        }
    }
}