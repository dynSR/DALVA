using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeedBuff_Test : StatusEffect
{
    public List<CharacterController> currentCharacterControllers = new List<CharacterController>();

    protected override void ApplyStatusEffect()
    {
        for (int i = 0; i < currentCharacterControllers.Count; i++)
        {
            currentCharacterControllers[i].CurrentSpeed = 10f;
            StatusEffectDurationHandler.ApplyStatusEffectDuration(this);
        }
    }

    public override void RemoveStatusEffect()
    {
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (StatusEffectDurationHandler.IsDurationOfStatusEffectApplied(this)) return;

            if (StatusEffectDurationHandler.CheckForSimilarExistingStatusEffect(this))
                base.CheckForExistingStatusEffect();

            SetTarget();
            ApplyStatusEffect();
            
        }
    }
}
