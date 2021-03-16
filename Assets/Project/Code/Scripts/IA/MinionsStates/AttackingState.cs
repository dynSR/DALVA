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
        if (controller.Interactions.Target != null)
        {
            float distance = Vector3.Distance(controller.Interactions.Target.position, controller.transform.position);

            //Enemy target is too far away
            if (distance > controller.Stats.CurrentAttackRange)
            {
                controller.ChangeState(new MovingState());
                controller.Interactions.ResetInteractionState();
            }
            else
                controller.Interactions.Interact();
        }
        //Has no enemy target
        else
            controller.ChangeState(new MovingState());
    }
}