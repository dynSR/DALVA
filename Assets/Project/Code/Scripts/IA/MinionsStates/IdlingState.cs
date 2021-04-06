
using UnityEngine;

public class IdlingState : IState
{
    private NPCController controller;

    void IState.Enter(NPCController controller)
    {
        this.controller = controller;

        controller.IsInIdleState = true;

        controller.transform.LookAt(controller.PositionToLookAt);
        controller.Stats.CanTakeDamage = true;
        controller.AggroStep = 8;
        controller.AggressionLimitsReached = false;

        if (controller.GetComponentInChildren<AggroRange>() != null) controller.GetComponentInChildren<AggroRange>().gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    void IState.Exit()
    {
        controller.NPCInteractions.StoppingDistance = controller.Stats.GetStat(StatType.AttackRange).Value;
        controller.IsInIdleState = false;
    }

    void IState.OnUpdate()
    {
        if (controller.isACampNPC && controller.NPCInteractions.HasATarget)
        {
            controller.ChangeState(new AttackingState());
        }
    }
}
