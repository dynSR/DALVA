using UnityEngine;

public class Ability_Mage_Z : AbilityLogic
{
    [SerializeField] private float augmentedMarkDuration = 0f;
    [Range(0,1)] [SerializeField] private float healthThresholdBonusDamage = 0f;
    private float markDuration = 0f;

    [SerializeField] private StatusEffect statusEffectToAttribute;

    protected void Awake()
    {
        markDuration = Ability.AbilityMarkDuration;
        Ability.AbilityAddedDamageOnTargetHealthThreshold = 0f;
    }

    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesSecondAbility", true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                SetAbilityMarkDuration(markDuration);
                Ability.DefaultEffectAppliedOnEnemy = null;
                Ability.AbilityAddedDamageOnTargetHealthThreshold = 0f;
                //SetRange(normalAbilityRange, normalAreaOfEffect, normalRange, augmentedRange, true);
                break;
            case AbilityEffect.II:
                //Augmente la durée de la marque de 2 secondes
                SetAbilityMarkDuration(augmentedMarkDuration);
                Ability.DefaultEffectAppliedOnEnemy = null;
                Ability.AbilityAddedDamageOnTargetHealthThreshold = 0f;
                //SetRange(normalAbilityRange, normalAreaOfEffect, normalRange, augmentedRange, true);
                break;
            case AbilityEffect.III:
                //Augmente la portée //Nouveau: Inflige 50 % dégâts supplémentaires aux ennemis qui ont moins de 25 % de leurs PV
                SetAbilityMarkDuration(markDuration);
                Ability.DefaultEffectAppliedOnEnemy = null;
                Ability.AbilityAddedDamageOnTargetHealthThreshold = healthThresholdBonusDamage;
                //SetRange(augmentedAbilityRange, augmentedAbilityAreaOfEffect, normalRange, augmentedRange);
                break;
            case AbilityEffect.IV:
                //Les ennemis marqués infligent 10 % dégâts en moins.
                SetAbilityMarkDuration(markDuration);
                Ability.DefaultEffectAppliedOnEnemy = statusEffectToAttribute;
                Ability.AbilityAddedDamageOnTargetHealthThreshold = 0f;
                //SetRange(normalAbilityRange, normalAreaOfEffect, normalRange, augmentedRange, true);
                break;
        }
    }
}