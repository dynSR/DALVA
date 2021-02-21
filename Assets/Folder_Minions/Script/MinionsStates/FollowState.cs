using UnityEngine;

class FollowState : IState
{
    private Sc_Minions_Base parent;
    public void Enter(Sc_Minions_Base parent)
    {
        Debug.Log("follow");
        this.parent = parent;
        parent.agent.isStopped = false;
    }

    public void Exit()
    {
        parent.agent.SetDestination(Vector3.zero);
    }

    public void Update()
    {
        if (parent.Target != null)
        {
            parent.agent.SetDestination(parent.Target.position);
            float distance = Vector3.Distance(parent.Target.position, parent.transform.position);
            if(distance <= parent.MyAttackRange)
            {
                parent.ChangeState(new AttackState());
            }
        }
        else
        {
            parent.ChangeState(new WaypointState());
        }
    }
}
