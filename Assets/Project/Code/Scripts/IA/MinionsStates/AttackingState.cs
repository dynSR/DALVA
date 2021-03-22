using UnityEngine;

class AttackingState : IState
{
    private NPCController controller;

    public void Enter(NPCController controller)
    {
        this.controller = controller;
        controller.CharacterAnimator.SetFloat("MoveSpeed", 0.0f);
    }

    public void Exit()
    {
        controller.Agent.isStopped = false;
        controller.Interactions.StoppingDistance = 0.2f;
    }

    public void OnUpdate()
    {
        Debug.Log("ATTACKING");

        //Has an enemy target
        if (controller.Interactions.HasATarget)
        {
            float distance = Vector3.Distance(controller.Interactions.Target.position, controller.transform.position);

            //Enemy target is too far away
            if (distance <= controller.Stats.GetStat(StatType.Attack_Range).Value
                && !controller.Interactions.Target.GetComponent<CharacterStat>().IsDead 
                && controller.Interactions.Target.GetComponent<VisibilityState>().IsVisible)
            {
                controller.Interactions.Interact();
            }
            else if (controller.Interactions.Target.GetComponent<CharacterStat>().IsDead
                || !controller.Interactions.Target.GetComponent<VisibilityState>().IsVisible)
            {
                controller.Interactions.ResetInteractionState();
                controller.Interactions.Target = null;
                controller.AggroRange.CheckForNewTarget();
            }
        }
        //Has no enemy target
        else
        {
            controller.Interactions.ResetInteractionState();
            controller.ChangeState(new MovingState());
        }
    }
}