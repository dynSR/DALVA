using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Mage_E : AbilityLogic
{
    [Header("Z EFFECTS")]
    [SerializeField] private StatusEffect slowEffect;
    [SerializeField] private StatusEffect shieldEffect;

    [Header("Z MARK EFFECTS")]
    [SerializeField] private StatusEffect stunEffect;
    [SerializeField] private StatusEffect rootEffect;

    [Header("SHIELD PROPAGATION AREA OF EFFECT")]
    [SerializeField] private GameObject shieldZone;
    [SerializeField] private float shieldZoneSize;

    float ShieldValue => Ability.AbilityShieldValue + (Stats.GetStat(StatType.MagicalPower).Value * Ability.ShieldMagicalRatio);

    protected override void Awake() => base.Awake();
    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesThirdAbility", true, true);

        EntityStats abilityTargetStats = AbilityTarget.GetComponent<EntityStats>();
        Collider targetCollider = AbilityTarget.GetComponent<Collider>();

        ApplyShieldOnOneTarget(abilityTargetStats);

        if (AbilityTarget.gameObject != transform.gameObject)
            StartCoroutine(ApplyDamageOnTargetWithDelay(Ability.AbilityTimeToCast  + 0.5f, targetCollider));

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                //"Sort ciblé : 
                //- sur ennemi: inflige(40 + 40 % PM) dégâts et ralentit de 15 VD pendant 2s, si la cible est marqué, immobilise au lieu de ralentir
                //- sur allié: donne un bouclier de(50 + 150 % PM) points de vie(PV) pendant 2s."
                Debug.Log("Applying damage");
                //ApplyingDamageOnTarget(AbilityTarget.GetComponent<Collider>());
                Ability.EffectAppliedOnMarkedEnemy = rootEffect;
                Ability.EffectAppliedOnMarkedAlly = null;
                Ability.AbilityCanConsumeMark = false;
                break;
            case AbilityEffect.II:
                //Étourdit la cible au lieu de l'immobiliser si elle est marquée.
                Ability.EffectAppliedOnMarkedEnemy = stunEffect;
                Ability.EffectAppliedOnMarkedAlly = null;
                Ability.AbilityCanConsumeMark = false;
                break;
            case AbilityEffect.III:
                //"Le Z marque aussi les alliés.
                //Si l'allié est marqué, soigne de (6% PV max) points de vie (PV) en plus du bouclier."
                Ability.EffectAppliedOnMarkedEnemy = rootEffect; //en trop
                Ability.EffectAppliedOnMarkedAlly = null;
                Ability.AbilityCanConsumeMark = false;
                if (abilityTargetStats.EntityTeam == Stats.EntityTeam && abilityTargetStats.EntityIsMarked) 
                    abilityTargetStats.Heal(AbilityTarget, abilityTargetStats.GetStat(StatType.Health).MaxValue * 0.06f, Stats.GetStat(StatType.HealAndShieldEffectiveness).Value);
                break;
            case AbilityEffect.IV:
                //"Le Z marque aussi les alliés.
                //Propage le bouclier aux alliés proches marqués."
                Ability.EffectAppliedOnMarkedEnemy = rootEffect; //en trop
                Ability.EffectAppliedOnMarkedAlly = shieldEffect;
                Ability.AbilityCanConsumeMark = true;

                if(AbilityTarget != transform || Stats.EntityIsMarked)
                    ApplyShieldOnOtherAllies();
                break;
        }
    }

    IEnumerator ApplyDamageOnTargetWithDelay(float delay, Collider target)
    {
        yield return new WaitForSeconds(delay);

        ApplyingDamageOnTarget(target);
    }

    void ApplyShieldOnOneTarget(EntityStats abilityTargetStats)
    {
        float shieldValue = shieldEffect.StatModifiers[0].Value;
        float shieldEffectiveness = Stats.GetStat(StatType.HealAndShieldEffectiveness).Value;

        if (abilityTargetStats.EntityTeam == Stats.EntityTeam)
        {
            shieldEffect.StatModifiers[0].Value = shieldValue;

            shieldEffect.ApplyEffect(abilityTargetStats.transform);
            abilityTargetStats.ApplyShieldOnTarget(abilityTargetStats.transform, 0, shieldEffectiveness);
        }
    }

    void ApplyShieldOnOtherAllies()
    {
        if (AbilityTarget.GetComponent<EntityStats>().EntityTeam != Stats.EntityTeam) return;

        if (AbilityTarget != transform)
            ApplyShieldOnOneTarget(AbilityTarget.GetComponent<EntityStats>());

        GameObject shieldZoneInstance = Instantiate(shieldZone, AbilityTarget.position, Quaternion.identity);
        ShieldZone _shieldZone = shieldZoneInstance.GetComponent<ShieldZone>();

        _shieldZone.SetShieldZone(shieldZoneSize, this, shieldEffect, shieldEffect.StatusEffectDuration, ShieldValue, 0f, false, true);
    }

    public override void SetAbilityAfterAPurchase()
    {
        throw new System.NotImplementedException();
    }

    protected override void ResetAbilityAttributes()
    {

    }
}