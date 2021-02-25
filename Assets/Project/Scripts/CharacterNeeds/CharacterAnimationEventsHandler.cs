using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterAnimationEventsHandler : MonoBehaviour
{
    public Animator MyAnimator => GetComponent<Animator>();
    private CharacterInteractionsHandler AttachedCombatBehaviour => GetComponentInParent<CharacterInteractionsHandler>();
    private CharacterStats AttachedStats => GetComponentInParent<CharacterStats>();

    [Header("CHARACTER ANIMATION CONTROLLERS")]
    [SerializeField] private RuntimeAnimatorController[] animatorsControllers;

    private void Awake()
    {
        switch (AttachedStats.CharacterClass)
        {
            case CharacterClass.Archer:
                MyAnimator.runtimeAnimatorController = animatorsControllers[0];
                break;
            //case CharacterClass.Berzerk:
            //    MyAnimator.runtimeAnimatorController = animatorsControllers[1];
            //    break;
            case CharacterClass.Coloss:
                MyAnimator.runtimeAnimatorController = animatorsControllers[1];
                break;
            case CharacterClass.DaggerMaster:
                MyAnimator.runtimeAnimatorController = animatorsControllers[2];
                break;
            case CharacterClass.Mage:
                MyAnimator.runtimeAnimatorController = animatorsControllers[3];
                break;
            //case CharacterClass.Priest:
            //    break;
            default:
                break;
        }
    }

    public void RangedAttack_AnimationEvent()
    {
        AttachedCombatBehaviour.RangedAttack();
    }

    public void MeleeAttack_AnimationEvent()
    {
        AttachedCombatBehaviour.MeleeAttack();
    }
}
