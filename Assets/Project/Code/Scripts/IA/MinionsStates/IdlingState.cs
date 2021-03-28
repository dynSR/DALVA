using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlingState : IState
{
    private NPCController controller;

    void IState.Enter(NPCController controller)
    {
        this.controller = controller;

        if(controller.transform.position != controller.startingPosition.position) 
            controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.startingPosition, 0);
    }

    void IState.Exit()
    {
        controller.NPCInteractions.StoppingDistance = controller.Stats.GetStat(StatType.AttackRange).Value;
    }

    void IState.OnUpdate()
    {
        if (controller.Stats.sourceOfDamage != null)
        {
            controller.NPCInteractions.Target = controller.Stats.sourceOfDamage;
            controller.ChangeState(new AttackingState()); 
        }
    }
}
