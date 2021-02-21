using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class WaypointState : IState
{
    private Sc_Minions_Base parent;
    public void Enter(Sc_Minions_Base parent)
    {
        Debug.Log("WaypointState");
        this.parent = parent;
        parent.agent.isStopped = false;
        if (parent.WavePointIndex == 0)
        {
            parent.agent.SetDestination(parent.WaypointsToReach[0].position);
            Debug.Log("waypoint "+ parent.WaypointsToReach[0].name);
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
            parent.agent.SetDestination(parent.WaypointsToReach[parent.WavePointIndex].position);
            if (Vector3.Distance(parent.transform.position, parent.WaypointsToReach[parent.WavePointIndex].position) <= 1.2f)
            {
                GetNextWaipoint();
            }
        }
    }

    void GetNextWaipoint()
    {
        if (parent.WavePointIndex >= parent.WaypointsToReach.Length - 1)
        {
            GameObject.Destroy(parent.gameObject);
            Debug.Log("lastWaypoint");
            return;
        }
        parent.WavePointIndex++;
        Debug.Log("next Waypoint"+ parent.WaypointsToReach[parent.WavePointIndex].name);
    }
}