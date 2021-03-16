using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : NPCBaseFSM
{
    private int waypointIndex ;
    private Transform waypointTarget = null;
    private readonly List<Transform> waypoints;


    private void Awake()
    {
        GetGlobalWaypoints();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        waypointTarget = waypoints[waypointIndex];
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Stats.IsDead) return;

        if (Vector3.Distance(owner.transform.position, waypointTarget.position) < 3.0f)
        {
            Debug.Log("Near enought to waypoint");
            waypointIndex++;
            waypointTarget = waypoints[waypointIndex];
        }

        Controller.HandleCharacterRotation(owner.transform, target.transform.position, Controller.RotateVelocity, Controller.RotationSpeed);
        Controller.HandleMotionAnimation(Controller.Agent, animator, "MoveSpeed", Controller.MotionSmoothTime);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }

    void GetGlobalWaypoints()
    {
        foreach (var item in MinionWaypointsManager.Instance.MinionsGlobalWaypoints)
        {
            waypoints.Add(item);
        }
    }
}
