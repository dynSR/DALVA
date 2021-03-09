using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class WaypointState : IState
{
    private MinionBehaviour parent;
    public void Enter(MinionBehaviour parent)
    {
        Debug.Log("WaypointState");

        this.parent = parent;
        parent.Agent.isStopped = false;

        if (parent.WavePointIndex == 0)
        {
            parent.Agent.SetDestination(parent.Waypoints[0].position);
            Debug.Log("waypoint "+ parent.Waypoints[0].name);
        }
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        //Changer en FollowState si un joueur ou un ennemi est proche
        if (parent.Target != null)
        {
            parent.ChangeState(new FollowState());
        }
        else
        {
            //parent.agent.isStopped = true;
            parent.Agent.SetDestination(parent.Waypoints[parent.WavePointIndex].position);

            if (Vector3.Distance(parent.transform.position, parent.Waypoints[parent.WavePointIndex].position) <= 1.2f)
            {
                GetNextWaypoint();
            }
        }
    }

    void GetNextWaypoint()
    {
        //if (parent.WavePointIndex >= parent.Waypoints.Count - 1)
        //{
        //    UnityEngine.Object.Destroy(parent.gameObject);
        //    Debug.Log("lastWaypoint");
        //    return;
        //}

        parent.WavePointIndex++;

        Debug.Log("next Waypoint"+ parent.Waypoints[parent.WavePointIndex].name);
    }
}