using System.Collections;
using UnityEngine;

public class Ability_Mage_R : AbilityLogic
{
    [Header("GENERAL PROPERTIES")]
    [SerializeField] private float cooldownAtStartForSorcerer = 0f;
    [SerializeField] private float cooldownAtStartForPriest = 0f;
    [SerializeField] private float healActivationDelay = 0f;

    [Header("CORE PROPERTIES")]
    [SerializeField] private float magicalDamageAtStart = 0f;
    [SerializeField] private float healAtStart = 0f;
    [SerializeField] private float magicalRatioAtStartForSorcerer = 0f;
    [SerializeField] private float magicalRatioAtStartForPriest = 0f;

    [Header("REDUCTION VALUES")]
    [SerializeField] private float cooldownReduction = 0f;
    [SerializeField] private float effectivenessReduction = 0f;

    [Header("DISPLAYERS")]
    [SerializeField] private GameObject sorcererRangeDisplayer;
    [SerializeField] private GameObject priestRangeDisplayer;

    [Header("ZONES")]
    [SerializeField] private GameObject damageZone;
    [SerializeField] private GameObject healZone;

    protected override void Awake() => base.Awake();

    protected override void OnEnable() => base.OnEnable();

    protected override void Update() => base.Update();

    protected override void Cast()
    {
        //if (!Stats.EntityIsAscended) return;

        //Joue toujours l'animation sachant que l'animation controller changera en fonction de l'ascension choisie OU ajouter un bool correspondant au check de la classe 
        PlayAbilityAnimation("UsesFourthAbility", true, true);

        //En fontion de la classe finale faire des choses en dessous / rapport à l'index (comportement normal)

        if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
        {
            //Displayers
            sorcererRangeDisplayer.SetActive(true);
            priestRangeDisplayer.SetActive(false);

            Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer;
            Ability.AbilityDuration = 0f;
            Ability.AbilityCooldown = cooldownAtStartForSorcerer;
            Ability.InstantCasting = false;

            StartCoroutine(InstantiateCorrectEffect(damageZone, 0.75f));
        }

        //if (Stats.EntityIsAscended)
        //{
        //    else if (Stats.BaseUsedEntity.EntityType == EntityType.Priest)
        //    {
        //        //Displayers
        //        sorcererRangeDisplayer.SetActive(false);
        //        priestRangeDisplayer.SetActive(true);

        //        Ability.AbilityMagicalRatio = magicalRatioAtStartForPriest;
        //        Ability.AbilityDuration = 3f;
        //        Ability.AbilityCooldown = cooldownAtStartForPriest;
        //        Ability.InstantCasting = true;

        //        StartCoroutine(InstantiateCorrectEffect(healZone, 0.15f));
        //    }
        //}

        switch (UsedEffectIndex)
        {
            //case AbilityEffect.I: // SORCERER - Rayon de flux magique, après 0,5s, inflige (150 + 150% PM) dégats magiques.	
            //                      // PRIEST - Crée une zone autour de soi qui soigne de(25 + 75 % PM) points de vie(PV) toutes les 0,5 secondes, pendant 3 secondes.

            //    Ability.AbilityCanMark = false;
            //    Ability.AbilityCanConsumeMark = false;

            //    Ability.AbilityMagicalDamage = magicalDamageAtStart;

            //    Ability.AbilityDamageBonusOnMarkedTarget = 0;

            //    break;

            //case AbilityEffect.II: //Marque les personnages touchés par la compétence.

            //    Ability.AbilityCanMark = true;
            //    Ability.AbilityCanConsumeMark = false;

            //    Ability.AbilityMagicalDamage = magicalDamageAtStart;

            //    Ability.AbilityDamageBonusOnMarkedTarget = 0;

            //    break;

            case AbilityEffect.III:  //Consomme la marque pour améliorer l'effet de la compétence.
                                     //SORCERER - +30 % PM dégâts magiques supplémentaires
                                     //PRIEST - soin supplémentaire de(12 % PV max) points de vie(PV))"

                Ability.AbilityCanMark = false;
                Ability.AbilityCanConsumeMark = true;

                Ability.AbilityMagicalDamage = magicalDamageAtStart;
                Ability.AbilityDamageBonusOnMarkedTarget = magicalDamageAtStart * 0.3f;

                break;

                //case AbilityEffect.IV: //Nouveau: Réduction fixe de cooldown / Efficacité réduite"

                //    Ability.AbilityCanMark = false;
                //    Ability.AbilityCanConsumeMark = false;

                //    #region Effectiveness/CD Reduction
                //    Ability.AbilityMagicalDamage = Ability.AbilityMagicalDamage - (Ability.AbilityMagicalDamage * (effectivenessReduction / 100));
                //    Ability.AbilityCooldown = Ability.AbilityCooldown - (Ability.AbilityCooldown * (cooldownReduction / 100));
                //    #endregion

                //    Ability.AbilityDamageBonusOnMarkedTarget = 0;

                //    break;
        }
    }

    private IEnumerator InstantiateCorrectEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);

        //float maxTargetHealthScalingFactorValue;

        //if (Ability.AbilityCanConsumeMark) maxTargetHealthScalingFactorValue = 12f;
        //else maxTargetHealthScalingFactorValue = 0f;


        if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
        {
            GameObject gameObject = Instantiate(effect, transform.position, transform.rotation);

            gameObject.GetComponentInChildren<Mage_R_Damage_Zone>().SetZone(
               Stats,
               this,
               Ability.AbilityCanMark);
        }
        //else if (Stats.BaseUsedEntity.EntityType == EntityType.Priest)
        //{
        //    GameObject gameObject = Instantiate(effect, transform.position, effect.transform.rotation);
        //    gameObject.transform.SetParent(transform);

        //    gameObject.GetComponent<HealZone>().IsAttachedToPlayer = true;

        //    gameObject.GetComponent<HealZone>().SetZone(
        //        Stats,
        //        this, 
        //        healActivationDelay,
        //        Ability.AbilityHealValue,
        //        Ability.AbilityMagicalRatio,
        //        maxTargetHealthScalingFactorValue, 
        //        Ability.AbilityCanMark, 
        //        Ability.AbilityCanConsumeMark,
        //        false, 
        //        true);
        //}
    }

    public override void SetAbilityAfterAPurchase()
    {
        base.SetAbilityAfterAPurchase();
    }

    protected override void ResetAbilityAttributes()
    {
        Ability.AbilityCooldown = 0;

        if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
        {
            Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer;
        }
        //else if (Stats.BaseUsedEntity.EntityType != EntityType.Priest)
        //{
        //    Ability.AbilityMagicalRatio = magicalRatioAtStartForPriest;
        //}
    }
}