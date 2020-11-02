using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedDebuff_Test : StatusEffect
{
    public List<CharacterController> currentCharacterControllers = new List<CharacterController>();

    public override void RemoveStatusEffect()
    {
        Debug.Log("Remove");
        for (int i = currentCharacterControllers.Count - 1; i >= 0; i--)
        {
            currentCharacterControllers[i].CurrentSpeed = currentCharacterControllers[i].InitialSpeed;
            currentCharacterControllers.RemoveAt(i);
        }
    }

    public override void SetTarget()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            currentCharacterControllers.Add(Targets[i].GetComponent<CharacterController>());
        }
    }

    protected override void ApplyStatusEffect()
    {
        Debug.Log("Apply");
        for (int i = 0; i < currentCharacterControllers.Count; i++)
        {
            currentCharacterControllers[i].CurrentSpeed /= 2;
            StatusEffectDurationHandler.ApplyStatusEffectDuration(this);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (StatusEffectDurationHandler.IsDurationOfStatusEffectApplied(this)) return;

            SetTarget();
            ApplyStatusEffect();
        }
    }
}
