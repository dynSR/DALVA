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

        controller.CompareCurrentPositionFromStartingPosition();

        if (controller.NPCInteractions.HasATarget)
            MoveTowardsTarget();
        //OR
        else if (!controller.NPCInteractions.HasATarget)
        {
            if (controller.isACampNPC) MoveTowardsStartPosition();
            else MoveTowardsWaypoint();
        }
           
        controller.CheckDistanceFromWaypoint(controller.waypoints[controller.WaypointIndex]);
    }

    void MoveTowardsStartPosition()
    {
        float distanceFromStartingPosition = Vector3.Distance(controller.transform.position, controller.StartingPosition.position);

        if (distanceFromStartingPosition > 0.1f)
            controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.StartingPosition, 0);
        else if (distanceFromStartingPosition <= 0.1f) controller.ChangeState(new IdlingState());
    }

    void MoveTowardsTarget()
    {
        CharacterStat targetStat = controller.NPCInteractions.Target.GetComponent<CharacterStat>();
        VisibilityState targetVisibilityState = controller.NPCInteractions.Target.GetComponent<VisibilityState>();

        if (targetStat.IsDead || !targetVisibilityState.IsVisible)
        {
            if (controller.Stats.SourceOfDamage == controller.NPCInteractions.Target) controller.Stats.SourceOfDamage = null;

            controller.NPCInteractions.Target = null;
            return;
        }

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

        //Distance is too high towards next waypoint - Keep moving towards it...
        controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.waypointTarget, controller.NPCInteractions.StoppingDistance);

        //if while moving an entity does damage to us then its our new target
        //controller.SetSourceOfDamageAsTarget();
    }
}