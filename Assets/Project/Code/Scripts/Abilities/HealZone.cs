using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    EntityStats UserStats { get; set; }
    public bool IsAttachedToPlayer { get; set; }

    private SphereCollider TriggerZone => GetComponent<SphereCollider>();
    private Lifetime lifeTime;

    public List<EntityStats> targetsStats = new List<EntityStats>();

    public AbilityLogic AbilityUsed { get ; set; }
    public float ActivationDelay { get; set; }

    public float HealValue { get; set; }
    public bool CanMark { get; set; }
    public bool CanApplyOnMarkedTarget { get; set; }

    public float MagicalPowerBonusRatio { get; set; }
    public bool ScaledOnMagicalPower { get; set; }

    public float MaxTargetBonusRatio { get; set; }
    public bool ScaledOnMaxTargetHealth { get; set; }

    void OnEnable()
    {
        lifeTime = GetComponent<Lifetime>();
        InvokeRepeating(nameof(EnableTrigger), ActivationDelay, 1f);
    }

    private void OnDisable()
    {
        UserStats.DeactiveHealVFX();
    }

    public void SetZone(
        EntityStats userStats,
        AbilityLogic ability,
        float activationDelay,
        float baseHealValue,
        float magicalPowerBonusRatio,
        float maxTargetBonusRatio,
        bool canMark = false,
        bool canApplyOnMarkedTarget = false, 
        bool scaledOnMagicalPower = false,
        bool scaledOnMaxTargetHealth = false)
    {
        UserStats = userStats;

        if (IsAttachedToPlayer) targetsStats.Add(userStats);

        lifeTime.LifetimeValue = ability.Ability.AbilityDuration;
        lifeTime.DestroyAfterTime = true;
        StartCoroutine(lifeTime.DestroyAfterATime(lifeTime.LifetimeValue));

        //Set mandatories values
        AbilityUsed = ability;
        ActivationDelay = activationDelay;

        HealValue = baseHealValue;
        MagicalPowerBonusRatio = magicalPowerBonusRatio;
        MaxTargetBonusRatio = maxTargetBonusRatio;

        CanMark = canMark;
        CanApplyOnMarkedTarget = canApplyOnMarkedTarget;
        ScaledOnMagicalPower = scaledOnMagicalPower;
        ScaledOnMaxTargetHealth = scaledOnMaxTargetHealth;
    }

    private void EnableTrigger()
    {
        TriggerZone.enabled = true;

        for (int i = 0; i < targetsStats.Count; i++)
        {
            EntityStats entityStats = targetsStats[i];
            Debug.Log(targetsStats[i].name);

            if (entityStats.EntityTeam == UserStats.EntityTeam)
            {
                ApplyHealInTheZone(
                    entityStats,
                    HealValue,
                    MagicalPowerBonusRatio,
                    MaxTargetBonusRatio,
                    CanMark,
                    CanApplyOnMarkedTarget,
                    ScaledOnMagicalPower,
                    ScaledOnMaxTargetHealth);
            }
        }
    }

    private void ApplyHealInTheZone(
        EntityStats targetStats,
        float healValue,
        float magicalPowerBonusRatio,
        float maxHealthTargetBonusRatio,
        bool canMark,
        bool canApplyOnMarkedTarget,
        bool scaledOnMagicalPower, 
        bool scaledOnMaxTargetHealth)
    {
        Debug.Log("Heal");

        float healRealValue = healValue + (healValue * AbilityUsed.Ability.HealMagicalRatio);

        //Si peut marquer et la cible est marquée
        if (canApplyOnMarkedTarget)
        {
            if (targetStats.EntityIsMarked)
            {
                if (scaledOnMagicalPower)
                {
                    healRealValue += healValue * (magicalPowerBonusRatio / 100);
                }
                else if (scaledOnMaxTargetHealth)
                {
                    healRealValue += (targetStats.GetStat(StatType.Health).MaxValue * (maxHealthTargetBonusRatio / 100));
                }

                targetStats.EntityIsMarked = false;
            }
        }

        Debug.Log("Real Heal Value " + healRealValue);

        if (IsAttachedToPlayer) UserStats.Heal(targetStats.transform, healRealValue, UserStats.GetStat(StatType.HealAndShieldEffectiveness).Value);

        targetStats.Heal(targetStats.transform, healRealValue, UserStats.GetStat(StatType.HealAndShieldEffectiveness).Value);

        if (canMark) targetStats.EntityIsMarked = true;

        TriggerZone.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats targetStats = other.GetComponent<EntityStats>();

        if (targetStats != null && !targetStats.IsDead)
        {
            targetsStats.Add(targetStats);
        }
    }
}