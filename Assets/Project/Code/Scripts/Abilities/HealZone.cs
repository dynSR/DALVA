using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    private SphereCollider TriggerZone => GetComponent<SphereCollider>();
    private Lifetime LifeTime => GetComponent<Lifetime>();

    public AbilityLogic AbilityUsed { get ; set; }
    public float ActivationDelay { get; set; }

    public float HealValue { get; set; }
    public bool CanApplyOnMarkedTarget { get; set; }

    public float MagicalPowerBonusRatio { get; set; }
    public bool ScaledOnMagicalPower { get; set; }

    public float MaxTargetBonusRatio { get; set; }
    public bool ScaledOnMaxTargetHealth { get; set; }

    void OnEnable()
    {
        LifeTime.LifetimeValue = AbilityUsed.Ability.AbilityDuration;
        InvokeRepeating("EnableTrigger", ActivationDelay, 1f);
    }

    public void SetZone(
        AbilityLogic ability, 
        float activationDelay, 
        float baseHealValue,
        float magicalPowerBonusRatio,
        float maxTargetBonusRatio,
        bool canApplyOnMarkedTarget = false, 
        bool scaledOnMagicalPower = false,
        bool scaledOnMaxTargetHealth = false)
    {
        //Set mandatories values
        AbilityUsed = ability;
        ActivationDelay = activationDelay;

        HealValue = baseHealValue;
        MagicalPowerBonusRatio = magicalPowerBonusRatio;
        MaxTargetBonusRatio = maxTargetBonusRatio;

        CanApplyOnMarkedTarget = canApplyOnMarkedTarget;
        ScaledOnMagicalPower = scaledOnMagicalPower;
        ScaledOnMaxTargetHealth = scaledOnMaxTargetHealth;
    }

    private void EnableTrigger()
    {
        TriggerZone.enabled = true;
    }

    private void ApplyHealInTheZone(
        Collider target,
        float healValue,
        float magicalPowerBonusRatio,
        float maxTargetBonusRatio,
        bool canApplyOnMarkedTarget,
        bool scaledOnMagicalPower, 
        bool scaledOnMaxTargetHealth)
    {
        EntityStats entityStats = target.GetComponent<EntityStats>();

        //Si peut marquer et la cible est marquée
        if (canApplyOnMarkedTarget)
        {
            if (entityStats.EntityIsMarked)
            {
                if (scaledOnMagicalPower)
                {
                    healValue = healValue + healValue * (magicalPowerBonusRatio / 100);
                }
                else if (scaledOnMaxTargetHealth)
                {
                    healValue = healValue + healValue * (maxTargetBonusRatio / 100);
                }

                entityStats.EntityIsMarked = false;
            }
        }

        TriggerZone.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && !entityStats.IsDead)
        {
            ApplyHealInTheZone(
                other,
                HealValue,
                MagicalPowerBonusRatio,
                MaxTargetBonusRatio,
                CanApplyOnMarkedTarget,
                ScaledOnMagicalPower,
                ScaledOnMaxTargetHealth);
        }
    }
}