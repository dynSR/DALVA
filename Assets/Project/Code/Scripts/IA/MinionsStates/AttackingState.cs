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

            CharacterStat targetStat = controller.NPCInteractions.Target.GetComponent<CharacterStat>();
            VisibilityState targetVisibilityState = controller.NPCInteractions.Target.GetComponent<VisibilityState>();

            if (targetStat.IsDead || !targetVisibilityState.IsVisible)
            {
                if (controller.Stats.SourceOfDamage == controller.NPCInteractions.Target) controller.Stats.SourceOfDamage = null;

                controller.NPCInteractions.Target = null;
                return;
            }

            //Enemy target is close enough to interact...
            if (controller.DistanceWithTarget <= controller.Stats.GetStat(StatType.AttackRange).Value && targetVisibilityState.IsVisible)
            {
                //Interact
                controller.NPCInteractions.Interact();
            }
            else if (controller.DistanceWithTarget > controller.Stats.GetStat(StatType.AttackRange).Value && controller.NPCInteractions.CanPerformAttack || !targetVisibilityState.IsVisible)
            {
                controller.ChangeState(new MovingState());
            }
        }
    }
}