using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AttackState : IState
{
    private Sc_Minions_Base parent;
    public void Enter(Sc_Minions_Base parent)
    {
        Debug.Log("attack");
        this.parent = parent;
        parent.agent.isStopped = false;
    }

    public void Exit()
    {

    }

    public void Update()
    {
        if(parent.Target != null)
        {
            Debug.Log("Target != null");
            //besoin de  check la range et attaquer
            float distance = Vector3.Distance(parent.Target.position, parent.transform.position);
            parent.agent.isStopped = true;

            if (distance > parent.MyAttackRange)
            {
                Debug.Log("switchToFollow");
                parent.agent.isStopped = false;
                parent.ChangeState(new FollowState());
            }
        }
        else //(parent.Target == null)
        {
            Debug.Log("else or parent.Target == null");
            parent.Target = parent.GetComponentInChildren<Range>().GetNearestTarget();
            if (parent.GetComponentInChildren<Range>().GetNearestTarget() != null)
            {
                Debug.Log("NearestTarget != null");
                float distance = Vector3.Distance(parent.Target.position, parent.transform.position);
                if (distance > parent.MyAttackRange)
                {
                    Debug.Log("switchToFollow");
                    parent.agent.isStopped = false;
                    parent.ChangeState(new FollowState());
                }
            }
            else
            {
                Debug.Log("switchToWaypoint");
                parent.agent.isStopped = false;
                parent.ChangeState(new WaypointState());
            }
        }
    }
}