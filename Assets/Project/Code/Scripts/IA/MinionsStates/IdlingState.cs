﻿
using UnityEngine;

public class IdlingState : IState
{
    private NPCController controller;

    void IState.Enter(NPCController controller)
    {
        this.controller = controller;

        controller.IsInIdleState = true;
        ResetParametersWhenInIdle();
    }

    void IState.Exit()
    {
        controller.NPCInteractions.StoppingDistance = controller.Stats.GetStat(StatType.AttackRange).Value;
        controller.IsInIdleState = false;
    }

    void IState.OnUpdate()
    {
        if (controller.IsACampNPC && controller.NPCInteractions.HasATarget)
        {
            controller.ChangeState(new AttackingState());
        }
    }

    void ResetParametersWhenInIdle()
    {
        //Stats
        if (!controller.isAGuardian)
            controller.Stats.Heal(controller.transform, controller.Stats.GetStat(StatType.Health).MaxValue, 0);

        controller.Stats.CanTakeDamage = true;

        if (controller.StartingPosition != null)
            controller.transform.position = controller.StartingPosition.position;

        //Rotation
        controller.transform.LookAt(controller.PositionToLookAt);
        
        //Aggression attributes
        controller.AggroStep = 8;
        controller.AggressionLimitsReached = false;

        AggroRange aggroRange = controller.GetComponentInChildren<AggroRange>();

        if (aggroRange != null)
            aggroRange.gameObject.GetComponent<SphereCollider>().enabled = true;
    }
}
