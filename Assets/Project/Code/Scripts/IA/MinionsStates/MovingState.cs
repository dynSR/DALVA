using UnityEngine;

class MovingState : IState
{
    private NPCController controller;

    public void Enter(NPCController controller)
    {
        this.controller = controller;

        controller.waypointTarget = controller.waypoints[controller.WaypointIndex];
    }

    public void Exit()
    {
        controller.NPCInteractions.StoppingDistance = controller.Stats.GetStat(StatType.AttackRange).Value;
    }

    public void OnUpdate()
    {
        Debug.Log("MOVING");

        if (controller.GetComponent<CharacterStat>().IsDead || !controller.CanMove) return;

        if (controller.NPCInteractions.HasATarget)
            MoveTowardsTarget();
        //OR
        else if (!controller.NPCInteractions.HasATarget)
        {
            if (controller.isACampNPC) controller.ChangeState(new IdlingState());
            else MoveTowardsWaypoint();
        }
           
        controller.CheckDistanceFromWaypoint(controller.waypoints[controller.WaypointIndex]);
    }

    void MoveTowardsTarget()
    {
        CharacterStat targetStat = controller.NPCInteractions.Target.GetComponent<CharacterStat>();
        VisibilityState targetVisibilityState = controller.NPCInteractions.Target.GetComponent<VisibilityState>();

        if (targetStat.IsDead || !targetVisibilityState.IsVisible) controller.NPCInteractions.Target = null;

        //Has an enemy target
        if (!targetStat.IsDead && targetVisibilityState.IsVisible)
        {
            controller.DistanceWithTarget = Vector3.Distance(controller.transform.position, controller.NPCInteractions.Target.position);

            //Distance is ok to interact - Change current state to attack state
            if (controller.DistanceWithTarget <= controller.Stats.GetStat(StatType.AttackRange).Value)
            {
                Debug.Log("Can Interact");
                controller.ChangeState(new AttackingState());
            }
            //Distance is not ok to interact - Keep moving
            else if (controller.DistanceWithTarget > controller.Stats.GetStat(StatType.AttackRange).Value)
            {
                Debug.Log("Too far to Interact");
                controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.NPCInteractions.Target, controller.Stats.GetStat(StatType.AttackRange).Value);
            }
        }
    }

    void MoveTowardsWaypoint()
    {
        //Has no enemy target - target is now a waypoint...
        Debug.Log("Need to move towards next waypoint");

        //controller.DistanceWithTarget = Vector3.Distance(controller.transform.position, controller.waypointTarget.position);

        //Distance is too high towards next waypoint - Keep moving towards it...
        controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.waypointTarget, controller.NPCInteractions.StoppingDistance);

        //Distance is ok - Update waypoint index and move towards next waypoint...
        //controller.CheckDistanceFromWaypoint(controller.waypoints[controller.WaypointIndex]);

        if (controller.Stats.sourceOfDamage != null)
        {
            controller.NPCInteractions.Target = controller.Stats.sourceOfDamage;
            controller.ChangeState(new AttackingState());
        }
    }
}