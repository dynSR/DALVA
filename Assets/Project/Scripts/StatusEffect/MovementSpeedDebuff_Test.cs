using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedDebuff_Test : StatusEffect
{
    public List<CharacterController> currentCharacterControllers = new List<CharacterController>();

    public override void RemoveStatusEffect()
    {
        for (int i = currentCharacterControllers.Count - 1; i >= 0; i--)
        {
            if(DoStatusEffectResetTheValueAffectedToInitialValueBeforeApplying)
                currentCharacterControllers[i].CurrentSpeed = currentCharacterControllers[i].InitialSpeed;

            currentCharacterControllers.RemoveAt(i);
        }
    }

    public override void SetTargetAndApplyStatusEffectOnIt()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            StatusEffectHandler targetStatusEffectHandler = Targets[i].GetComponent<StatusEffectHandler>();

            if (targetStatusEffectHandler.IsDurationOfStatusEffectAlreadyApplied(this)) return;

            CharacterController targetCharacterController = Targets[i].GetComponent<CharacterController>();

            //currentCharacterControllers.Add(Targets[i].GetComponent<CharacterController>());

            base.CheckForExistingStatusEffect(targetStatusEffectHandler);

            targetCharacterController.CurrentSpeed /= 2;

            //currentCharacterControllers[i].CurrentSpeed /= 2;

            targetStatusEffectHandler.ApplyNewStatusEffectDuration(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in trigger");
            if (Targets.Count <= 0)
            {
                Targets.Add(other.transform);
            }
            else
            {
                for (int i = 0; i < Targets.Count; i++)
                {
                    if (other.transform != Targets[i])
                        Targets.Add(other.transform);
                }
            }
            
            SetTargetAndApplyStatusEffectOnIt();
        }
    }
}
