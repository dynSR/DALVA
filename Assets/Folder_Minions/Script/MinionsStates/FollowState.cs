using UnityEngine;

class FollowState : IState
{
    private MinionBehaviour parent;
    public void Enter(MinionBehaviour parent)
    {
        Debug.Log("follow");
        this.parent = parent;

        parent.Agent.isStopped = false;
    }

    public void Exit()
    {
        parent.Agent.isStopped = true;
        parent.Agent.stoppingDistance = parent.MyAttackRange;
    }

    public void Update()
    {
        if (parent.Target != null)
        {
            UtilityClass.SetAgentDestination(parent.Target.position, parent.Agent);

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
