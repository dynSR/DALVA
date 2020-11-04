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
            CooldownHandler targetCooldownHandler = Targets[i].GetComponent<CooldownHandler>();

            if (targetCooldownHandler.IsDurationOfStatusEffectAlreadyApplied(this)) return;

            currentCharacterControllers.Add(Targets[i].GetComponent<CharacterController>());
            base.CheckForExistingStatusEffect(targetCooldownHandler);
            currentCharacterControllers[i].CurrentSpeed /= 2;
            targetCooldownHandler.ApplyNewStatusEffectDuration(this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetTargetAndApplyStatusEffectOnIt();
        }
    }
}
