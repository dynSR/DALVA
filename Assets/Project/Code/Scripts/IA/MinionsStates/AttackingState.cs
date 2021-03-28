using UnityEngine;

class AttackingState : IState
{
    private NPCController controller;

    public void Enter(NPCController controller)
    {
        this.controller = controller;
    }

    public void Exit()
    {
        controller.Agent.isStopped = false;
        controller.NPCInteractions.StoppingDistance = 0.2f;

        controller.NPCInteractions.ResetInteractionState();
    }

    public void OnUpdate()
    {
        Debug.Log("ATTACKING");

        //Has no enemy target
        if (!controller.NPCInteractions.HasATarget)
        {
            Debug.Log("Has no target");
            
            controller.ChangeState(new MovingState());
            return;
        }

        //Has an enemy target
        if (controller.NPCInteractions.HasATarget)
        {
            Debug.Log("Has a target");

            controller.DistanceWithTarget = Vector3.Distance(controller.NPCInteractions.Target.position, controller.transform.position);

            VisibilityState targetVisibilityState = controller.NPCInteractions.Target.GetComponent<VisibilityState>();

            //Enemy target is too far away
            if (controller.DistanceWithTarget <= controller.Stats.GetStat(StatType.AttackRange).Value && targetVisibilityState.IsVisible)
            {
                controller.NPCInteractions.Interact();
            }

            else if (controller.DistanceWithTarget > controller.Stats.GetStat(StatType.AttackRange).Value 
                && controller.NPCInteractions.CanPerformAttack
                || !targetVisibilityState.IsVisible)
            {
                //if (controller.isACampNPC) controller.ChangeState(new IdlingState());
                /*else */controller.ChangeState(new MovingState());
            }
        }
    }
}