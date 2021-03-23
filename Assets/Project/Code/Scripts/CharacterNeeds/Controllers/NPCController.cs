using System.Collections.Generic;
using UnityEngine;

public class NPCController : CharacterController
{
    public int waypointIndex; //public to debug
    public Transform waypointTarget = null; //public to debug
    public List<Transform> waypoints; //public to debug

    public IState currentState; //set to private after tests

    #region Refs
    public CharacterStat Stats => GetComponent<CharacterStat>();
    public NPCInteractions Interactions => GetComponent<NPCInteractions>();
    public AggroRange AggroRange => GetComponentInChildren<AggroRange>();
    #endregion

    private void Start()
    {
        GetGlobalWaypoints();
        ChangeState(new MovingState());
    }

    protected override void Update()
    {
        base.Update();
        currentState.OnUpdate();
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    private void GetGlobalWaypoints()
    {
        foreach (var item in MinionWaypointsManager.Instance.MinionsGlobalWaypoints)
        {
            waypoints.Add(item);
        }
    }
}