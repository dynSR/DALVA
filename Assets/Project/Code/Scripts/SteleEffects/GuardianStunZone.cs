using UnityEngine;

public class GuardianStunZone : StatusEffectZoneCore
{
    [SerializeField] private StatusEffect guardianStatusEffect;
    [SerializeField] private float delayBeforeApplyingEffect = 3f;

    void Start()
    {
        InvokeRepeating("ApplyStunEffectOverTime", 1, delayBeforeApplyingEffect);
    }

    void ApplyStunEffectOverTime()
    {
        for (int i = 0; i < statsOfEntitiesInTrigger.Count; i++)
        {
            ApplyAffect(statsOfEntitiesInTrigger[i]);
        }
    }

    protected override void ApplyAffect(EntityStats target)
    {
        guardianStatusEffect.ApplyEffect(target.transform);
    }

    protected override void RemoveEffect(EntityStats target)
    {
        //NOTHING
    }
}