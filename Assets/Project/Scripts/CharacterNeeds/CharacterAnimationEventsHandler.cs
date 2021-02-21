using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEventsHandler : MonoBehaviour
{
    private CharacterCombatBehaviour attachedCombatBehaviour;

    private void Awake()
    {
        attachedCombatBehaviour = GetComponentInParent<CharacterCombatBehaviour>();
    }  

    public void RangedAttack_AnimationEvent()
    {
        attachedCombatBehaviour.RangedAttack();
    }

    public void MeleeAttack_AnimationEvent()
    {
        attachedCombatBehaviour.MeleeAttack();
    }
}
