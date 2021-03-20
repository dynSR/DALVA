using UnityEngine;

class MovingState : IState
{
    private NPCController controller;

    float distance;

    public void Enter(NPCController controller)
    {
        this.controller = controller;

        controller.waypointTarget = controller.waypoints[controller.waypointIndex];
    }

    public void Exit()
    {
        controller.Interactions.StoppingDistance = controller.Stats.GetStat(StatType.Attack_Range).Value;
    }

    public void OnUpdate()
    {
        Debug.Log("MOVING");

        //Has an enemy target
        if (controller.Interactions.HasATarget 
            && !controller.Interactions.Target.GetComponent<CharacterStat>().IsDead 
            && controller.Interactions.Target.GetComponent<VisibilityState>().IsVisible)
        {
            distance = Vector3.Distance(controller.transform.position, controller.Interactions.Target.position);

            //Distance is ok to interact - Change current state to attack state
            if (distance <= controller.Stats.GetStat(StatType.Attack_Range).Value)
                controller.ChangeState(new AttackingState());

            //Distance is not ok to interact - Keep moving
            else if(distance > controller.Stats.GetStat(StatType.Attack_Range).Value)
                controller.Interactions.MoveTowardsAnExistingTarget(controller.Interactions.Target, controller.Stats.GetStat(StatType.Attack_Range).Value);
        }
        //Has no enemy target - target is now a waypoint...
        else if (!controller.Interactions.HasATarget)
        {
            distance = Vector3.Distance(controller.transform.position, controller.waypointTarget.position);

            //Distance is too high towards next waypoint - Keep moving towards it...
            if (distance > controller.Agent.stoppingDistance)
                controller.Interactions.MoveTowardsAnExistingTarget(controller.waypointTarget, controller.Interactions.StoppingDistance);

            //Distance is ok - Update waypoint index and move towards next waypoint...
            if (distance <= 1f)
            {
                controller.waypointIndex++;
                controller.waypointTarget = controller.waypoints[controller.waypointIndex];
            }
        }
    }
}