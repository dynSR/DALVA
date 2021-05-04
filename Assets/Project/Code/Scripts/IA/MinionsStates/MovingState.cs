using UnityEngine;

class MovingState : IState
{
    private NPCController controller;

    public void Enter(NPCController controller)
    {
        this.controller = controller;

        if (!controller.isACampNPC)
            controller.waypointTarget = controller.waypoints[controller.WaypointIndex];
    }

    public void Exit()
    {
        controller.NPCInteractions.StoppingDistance = controller.Stats.GetStat(StatType.AttackRange).Value;
    }

    public void OnUpdate()
    {
        //Debug.Log("MOVING");

        if (controller.Stats.IsDead || controller.IsStunned || controller.IsRooted) return;

        if (controller.NPCInteractions.HasATarget) MoveTowardsTarget();
        
        if (!controller.NPCInteractions.HasATarget)
        {
            if (controller.isACampNPC) MoveTowardsStartingPosition();
            else MoveTowardsWaypoint();
        }
        
        if(!controller.isACampNPC)
            controller.CheckDistanceFromWaypoint(/*controller.waypoints[controller.WaypointIndex]*/ controller.waypointTarget);
        else controller.CompareCurrentPositionFromStartingPosition();
    }

    void MoveTowardsStartingPosition()
    {
        float distanceFromStartingPosition = Vector3.Distance(controller.transform.position, controller.StartingPosition.position);

        controller.Stats.RegenerateHealth(controller.transform, controller.Stats.GetStat(StatType.Health).Value * 0.15f);

        AggroRange aggroRange = controller.GetComponentInChildren<AggroRange>();

        if (aggroRange != null) aggroRange.gameObject.GetComponent<SphereCollider>().enabled = false;

        if (distanceFromStartingPosition > 0.1f) controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.StartingPosition, 0);
        else if (distanceFromStartingPosition <= 0.1f) controller.ChangeState(new IdlingState());
    }

    void MoveTowardsTarget()
    {
        Debug.Log("Move Towards Target");

        EntityStats targetStat = controller.NPCInteractions.Target.GetComponent<EntityStats>();

        if (targetStat.IsDead)
        {
            if (controller.Stats.SourceOfDamage == controller.NPCInteractions.Target) controller.Stats.SourceOfDamage = null;

            controller.NPCInteractions.Target = null;
            return;
        }

        //Has an enemy target
        if (!targetStat.IsDead)
        {
            controller.DistanceWithTarget = Vector3.Distance(controller.transform.position, controller.NPCInteractions.Target.position);

            //Distance is ok to interact - Change current state to attack state
            if (controller.DistanceWithTarget <= controller.Stats.GetStat(StatType.AttackRange).Value)
            {
                //Debug.Log("Can Interact");
                controller.ChangeState(new AttackingState());
            }
            //Distance is not ok to interact - Keep moving
            else if (controller.DistanceWithTarget > controller.Stats.GetStat(StatType.AttackRange).Value)
            {
                //Debug.Log("Too far to Interact");
                controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.NPCInteractions.Target, controller.Stats.GetStat(StatType.AttackRange).Value);
            }
        }
    }

    void MoveTowardsWaypoint()
    {
        //Distance is too high towards next waypoint - Keep moving towards it...
        Debug.Log("Moving towards waypoint");
        controller.NPCInteractions.MoveTowardsAnExistingTarget(controller.waypointTarget, controller.NPCInteractions.StoppingDistance);
    }
}