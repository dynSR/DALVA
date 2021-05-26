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
    protected override void Update() => base.Update();

    protected override void Cast()
    {
        //if (!Stats.EntityIsAscended) return;

        //Joue toujours l'animation sachant que l'animation controller changera en fonction de l'ascension choisie OU ajouter un bool correspondant au check de la classe 
        Controller.CharacterAnimator.SetBool("IsSorcerer", true);
        PlayAbilityAnimation("UsesFourthAbility", true);

        //En fontion de la classe finale faire des choses en dessous / rapport à l'index (comportement normal)

        if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
        {
            //Displayers
            sorcererRangeDisplayer.SetActive(true);

            Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer;
            Ability.AbilityDuration = 0f;
            Ability.AbilityCooldown = cooldownAtStartForSorcerer;
            Ability.InstantCasting = false;

            StartCoroutine(InstantiateCorrectEffect(damageZone, 0.55f));
        }

        switch (UsedEffectIndex)
        {
            case AbilityEffect.III:  //Consomme la marque pour améliorer l'effet de la compétence.
                                     //SORCERER - +30 % PM dégâts magiques supplémentaires
                                     //PRIEST - soin supplémentaire de(12 % PV max) points de vie(PV))"

                Ability.AbilityCanMark = false;
                Ability.AbilityCanConsumeMark = true;

                Ability.AbilityMagicalDamage = magicalDamageAtStart;
                Ability.AbilityDamageBonusOnMarkedTarget = magicalDamageAtStart * 0.3f;

                break;
        }
    }

    private IEnumerator InstantiateCorrectEffect(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
        {
            GameObject gameObject = Instantiate(effect, transform.position, transform.rotation);

            gameObject.GetComponentInChildren<Mage_R_Damage_Zone>().SetZone(
               Stats,
               this,
               Ability.AbilityCanMark);
        }

        yield return new WaitForEndOfFrame();
        Controller.IsCasting = false;
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
    }
}