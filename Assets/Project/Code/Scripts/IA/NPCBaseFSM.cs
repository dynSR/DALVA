using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBaseFSM : StateMachineBehaviour
{
    protected GameObject owner;
    protected GameObject target;

    #region Refs
    protected CharacterStats Stats => owner.GetComponent<CharacterStats>();
    protected NPCController Controller => owner.GetComponent<NPCController>();
    protected NPCInteractions Interactions => owner.GetComponent<NPCInteractions>();
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.gameObject;

        if (owner.GetComponent<NPCInteractions>().Target.gameObject)
            target = owner.GetComponent<NPCInteractions>().Target.gameObject;
    }
}
