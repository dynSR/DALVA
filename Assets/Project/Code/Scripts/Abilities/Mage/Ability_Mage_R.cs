using UnityEngine;

public class Ability_Mage_R : AbilityLogic
{
    [Header("GENERAL PROPERTIES")]
    [SerializeField] private float cooldownAtStart = 0f;
    [SerializeField] private float activationDelay = 0f;

    [Header("CORE PROPERTIES")]
    [SerializeField] private float healAtStart = 0f;
    [SerializeField] private float durationAtStart = 0f;
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
        if (!Stats.EntityIsAscended) return;

        //Joue toujours l'animation sachant que l'animation controller changera en fonction de l'ascension choisie OU ajouter un bool correspondant au check de la classe 
        PlayAbilityAnimation("UsesFourthAbility", true, true);

        //En fontion de la classe finale faire des choses en dessous / rapport à l'index (comportement normal)
        if (Stats.EntityIsAscended)
        {
            if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
            {               
                //Displayers
                sorcererRangeDisplayer.SetActive(true);
                priestRangeDisplayer.SetActive(false);

                Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer;
                Ability.AbilityDuration = durationAtStart;

                InstantiateCorrectEffect(damageZone);
            }
            else if (Stats.BaseUsedEntity.EntityType != EntityType.Priest)
            {
                //Displayers
                sorcererRangeDisplayer.SetActive(false);
                priestRangeDisplayer.SetActive(true);

                Ability.AbilityMagicalRatio = magicalRatioAtStartForPriest;
                Ability.AbilityDuration = 3f;

                InstantiateCorrectEffect(healZone);
            }
        }

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I: // SORCERER - Rayon de flux magique, après 0,5s, inflige (150 + 150% PM) dégats magiques.	
                                  // PRIEST - Crée une zone autour de soi qui soigne de(25 + 75 % PM) points de vie(PV) toutes les 0,5 secondes, pendant 3 secondes.

                Ability.AbilityCanMark = false;
                Ability.AbilityCooldown = cooldownAtStart;
                Ability.AbilityCanConsumeMark = false;

                if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer) Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer;
                else if (Stats.BaseUsedEntity.EntityType != EntityType.Priest) Ability.AbilityMagicalRatio = magicalRatioAtStartForPriest;

                break;
            case AbilityEffect.II: //Marque les personnages touchés par la compétence.

                Ability.AbilityCanMark = true;
                Ability.AbilityCooldown = cooldownAtStart;
                Ability.AbilityCanConsumeMark = false;

                if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer) Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer;
                else if (Stats.BaseUsedEntity.EntityType != EntityType.Priest) Ability.AbilityMagicalRatio = magicalRatioAtStartForPriest;

                break;
            case AbilityEffect.III:  //Consomme la marque pour améliorer l'effet de la compétence.
                                     //SORCERER - +30 % PM dégâts magiques supplémentaires
                                     //PRIEST - soin supplémentaire de(12 % PV max) points de vie(PV))"

                Ability.AbilityCanMark = false;
                Ability.AbilityCooldown = cooldownAtStart;
                Ability.AbilityCanConsumeMark = true;
               
                if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer) Ability.AbilityMagicalRatio = magicalRatioAtStartForSorcerer + (30  / 100);
                else if (Stats.BaseUsedEntity.EntityType != EntityType.Priest) Ability.AbilityMagicalRatio = magicalRatioAtStartForPriest;

                break;
            case AbilityEffect.IV: //Nouveau: Réduction fixe de cooldown / Efficacité réduite"

                Ability.AbilityCanMark = false;
                Ability.AbilityCooldown = Ability.AbilityCooldown * (cooldownReduction / 100);
                Ability.AbilityMagicalRatio = Ability.AbilityMagicalRatio * (effectivenessReduction / 100);
                Ability.AbilityCanConsumeMark = false;

                break;
        }
    }

    private void InstantiateCorrectEffect(GameObject effect)
    {
        float maxTargetHealthScalingFactorValue;

        if (Ability.AbilityCanConsumeMark) maxTargetHealthScalingFactorValue = 12f;
        else maxTargetHealthScalingFactorValue = 0f;


        if (Stats.BaseUsedEntity.EntityType == EntityType.Sorcerer)
        {
            GameObject gameObject = Instantiate(effect, transform.position, transform.rotation);
        }
        else if (Stats.BaseUsedEntity.EntityType == EntityType.Priest)
        {
            GameObject gameObject = Instantiate(effect, transform.position, Quaternion.identity);
            gameObject.transform.SetParent(transform);

            gameObject.GetComponent<HealZone>().SetZone(
                this, 
                activationDelay,
                Ability.AbilityHealValue,
                Ability.AbilityMagicalRatio,
                maxTargetHealthScalingFactorValue, 
                Ability.AbilityCanConsumeMark, 
                false, 
                true);
        }
    }

    protected override void SetAbilityAfterAPurchase()
    {
        throw new System.NotImplementedException();
    }

    protected override void ResetAbilityAttributes()
    {

    }
}