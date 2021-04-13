using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Mage_Z : AbilityLogic
{
    [SerializeField] private float augmentedMarkDuration = 0f;
    private float markDuration = 0f;

    [SerializeField] private StatusEffect statusEffectToAttribute;

    [SerializeField] private GameObject normalRange;
    [SerializeField] private GameObject augmentedRange;
    [SerializeField] private float augmentedAbilityRange;
    [SerializeField] private float augmentedAbilityAreaOfEffect;
    private float normalAbilityRange = 0f;
    private float normalAreaOfEffect = 0f;

    private void Awake()
    {
        markDuration = Ability.AbilityMarkDuration;
        normalAbilityRange = Ability.AbilityRange;
        normalAreaOfEffect = Ability.AbilityAreaOfEffect;
    }

    protected override void Update() => base.Update();

    protected override void Cast()
    {
        PlayAbilityAnimation("UsesSecondAbility", true, true);

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                Ability.AbilityMarkDuration = markDuration;
                SetAbilityStatusEffect(null);
                SetRange(normalAbilityRange, normalAreaOfEffect, normalRange, augmentedRange, true);
                break;
            case AbilityEffect.II:
                //Augmente la durée de la marque de 2 secondes
                SetAbilityMarkDuration(augmentedMarkDuration);
                SetAbilityStatusEffect(null);
                SetRange(normalAbilityRange, normalAreaOfEffect, normalRange, augmentedRange, true);
                break;
            case AbilityEffect.III:
                //Augmente la portée
                SetAbilityMarkDuration(markDuration);
                SetAbilityStatusEffect(null);
                SetRange(augmentedAbilityRange, augmentedAbilityAreaOfEffect, normalRange, augmentedRange);
                break;
            case AbilityEffect.IV:
                //Les ennemis marqués infligent 10 % dégâts en moins.
                SetAbilityMarkDuration(markDuration);
                SetAbilityStatusEffect(statusEffectToAttribute);
                SetRange(normalAbilityRange, normalAreaOfEffect, normalRange, augmentedRange, true);
                break;
        }
    }

    protected override void SetAbilityAfterAPurchase()
    {
        throw new System.NotImplementedException();
    }
}