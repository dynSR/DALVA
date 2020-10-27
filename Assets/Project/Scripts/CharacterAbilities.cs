using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbilities : MonoBehaviour
{
    private PlayerController player;

    [Header("CHARACTER SPELLS")]
    [SerializeField] private Ability[] characterAbilities;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        ProcessAbilityInputs(KeyCode.A, characterAbilities[0]);
    }

    void ProcessAbilityInputs(KeyCode keyPressed, Ability abilityUsed)
    {
        if (Input.GetKeyDown(keyPressed))
        {
            Debug.Log("Using An Ability");
            UseAbility(abilityUsed);
        }
    }

    void UseAbility(Ability abilityUsed)
    {
        Debug.Log("Applying CD");
        switch (abilityUsed.AbilityType)
        {
            case AbilityType.Buff:
                break;
            case AbilityType.Heal:
                break;
            case AbilityType.Debuff:
                break;
            case AbilityType.Projectile:
                abilityUsed.ThrowProjectile(abilityUsed.AbilityPrefab, player.ProjectileEmiterLocation);
                break;
            case AbilityType.CC:
                break;
            case AbilityType.Movement:
                break;
            case AbilityType.Shield:
                break;
            default:
                break;
        }
    }
}
