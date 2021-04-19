using UnityEngine;

public class Ability_Mage_E : AbilityLogic
{
    [Header("Z EFFECTS")]
    [SerializeField] private StatusEffect slowEffect;
    [SerializeField] private StatusEffect shieldEffect;

    protected override void Awake() => base.Awake();
    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesThirdAbility", true, true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                //"Sort ciblé : 
                //- sur ennemi: inflige(40 + 40 % PM) dégâts et ralentit de 15 VD pendant 2s, si la cible est marqué, immobilise au lieu de ralentir
                //- sur allié: donne un bouclier de(50 + 150 % PM) points de vie(PV) pendant 2s."
                if (AbilityTarget.GetComponent<EntityStats>().EntityTeam == Stats.EntityTeam)
                {
                    Ability.AbilityStatusEffect = shieldEffect;

                    shieldEffect.StatModifiers.Clear();
                    shieldEffect.StatModifiers.Add(
                        new StatModifier(Ability.AbilityShieldValue + (Stats.GetStat(StatType.MagicalPower).Value * Ability.ShieldMagicalRatio),
                        StatType.Shield,
                        StatModType.Flat,
                        shieldEffect));

                    AbilityTarget.GetComponent<EntityStats>().ApplyShieldOnTarget(AbilityTarget, shieldEffect.StatModifiers[0].Value, 0f);
                }
                else if (AbilityTarget.GetComponent<EntityStats>().EntityTeam != Stats.EntityTeam) Ability.AbilityStatusEffect = slowEffect;
                    break;
            case AbilityEffect.II:
                //Étourdit la cible au lieu de l'immobiliser si elle est marquée.
                break;
            case AbilityEffect.III:
                //"Le Z marque aussi les alliés.
                //Si l'allié est marqué, soigne de (6% PV max) points de vie (PV) en plus du bouclier."
                break;
            case AbilityEffect.IV:
                //"Le Z marque aussi les alliés.
                //Propage le bouclier aux alliés proches marqués."
                break;
        }
    }

    protected override void SetAbilityAfterAPurchase()
    {
        throw new System.NotImplementedException();
    }
}