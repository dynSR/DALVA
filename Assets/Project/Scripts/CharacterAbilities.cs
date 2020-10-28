using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;

public class CharacterAbilities : MonoBehaviour
{
    private PlayerController playerController;
    private LaunchGameObject launchGameObject;

    [Header("CHARACTER ABILITIES")]
    [SerializeField] private Ability[] characterAbilities;
    public bool abilityOneCanBeUsed = true;
    public bool abilityOneHasBeenTrigger = false;
    private float cd1;

    [Header("ABILITIES INPUTS")]
    [SerializeField] private KeyCode[] abilitiesInputs;

    private void Start()
    {
        launchGameObject = GetComponent<LaunchGameObject>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        //A - Q
        if (Input.GetKeyDown(abilitiesInputs[0]))
        {
            ProcessAbilityInputs(characterAbilities[0], abilityOneCanBeUsed);
        }

        ApplyCooldownToAbilityOne(characterAbilities[0]);
    }

    void ProcessAbilityInputs(Ability abilityUsed, bool abilityCanBeUsed)
    {
        if (!abilityCanBeUsed) return;

        abilityOneHasBeenTrigger = true;
        playerController.NavMeshAgent.ResetPath();
        cd1 = abilityUsed.AbilityCooldown;
        UseAbility(abilityUsed);
    }

    void ApplyCooldownToAbilityOne(Ability abilityUsed)
    {
        if (abilityOneHasBeenTrigger)
        {
            abilityOneCanBeUsed = false;

            cd1 -= Time.deltaTime;

            if (cd1 <= 0)
            {
                abilityOneCanBeUsed = true;
                abilityOneHasBeenTrigger = false;
            }
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
                launchGameObject.TurnCharacterTowardsLaunchDirection();
                launchGameObject.LaunchProjectile(abilityUsed.AbilityPrefab, launchGameObject.EmmiterPosition);
                break;
            case AbilityType.CrowdControl:
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
