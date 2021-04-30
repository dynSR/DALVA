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
        //Debug.Log("ATTACKING");

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
            //Debug.Log("Has a target");

            controller.DistanceWithTarget = Vector3.Distance(controller.NPCInteractions.Target.position, controller.transform.position);

            EntityStats targetStat = controller.NPCInteractions.Target.GetComponent<EntityStats>();
            VisibilityState targetVisibilityState = controller.NPCInteractions.Target.GetComponent<VisibilityState>();

            if (targetStat.IsDead /*|| !targetVisibilityState.IsVisible*/)
            {
                if (controller.Stats.SourceOfDamage == controller.NPCInteractions.Target) controller.Stats.SourceOfDamage = null;

                controller.NPCInteractions.Target = null;

                if (controller.AggroRange != null) controller.AggroRange.CheckForNewTarget();

                return;
            }

            //Enemy target is close enough to interact...
            if (controller.DistanceWithTarget <= controller.Stats.GetStat(StatType.AttackRange).Value /*&& targetVisibilityState.IsVisible*/)
            {
                //Interact
                controller.NPCInteractions.Interact();
            }
            else if (controller.DistanceWithTarget > controller.Stats.GetStat(StatType.AttackRange).Value
                && controller.NPCInteractions.CanPerformAttack
                /*|| !targetVisibilityState.IsVisible*/)
            {
                if (controller.Stats.GetStat(StatType.MovementSpeed).Value <= 0)
                {
                    controller.ChangeState(new IdlingState());
                }
                else if (controller.Stats.GetStat(StatType.MovementSpeed).Value > 0)
                    controller.ChangeState(new MovingState());
            }
        }
    }
}