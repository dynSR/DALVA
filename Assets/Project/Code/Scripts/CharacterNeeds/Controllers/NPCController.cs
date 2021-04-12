using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : CharacterController
{
    public delegate void IdleStateHandler();
    public event IdleStateHandler OnEnteringIdleState;
    public event IdleStateHandler OnExitingIdleState;

    public delegate void AggroStepHandler(float aggroStep);
    public event AggroStepHandler OnAggroValueChanged;

    public int WaypointIndex { get; set; }
    public Transform waypointTarget = null; //public to debug
    public List<Transform> waypoints; //public to debug
    public float DistanceWithTarget { get; set; }

    public IState currentState; //set to private after tests
    public string CurrentStateName; //for tests purpose

    [Header("NPC TYPE")]
    public bool isACampNPC = false;

    [Header("FOREST CAMP NPCS ATTRIBUTES")]
    [SerializeField] private Transform positionToLookAt;
    [SerializeField] private float aggressionLimitsValue = 2.5f;
    [SerializeField] private float delayBeforeDecreasingAggroSteps = .5f;
    [SerializeField] private int maxAggroStep = 8;
    [SerializeField] private int aggroStep = 8;
    private bool aggressionLimitsReached = false;
    private bool anAllyHasBeenAttacked = false;

    public Transform PositionToLookAt { get => positionToLookAt; }
    public int AggroStep { get => aggroStep; set => aggroStep = value; }
    public int MaxAgroStep { get => maxAggroStep; }
    public Transform StartingPosition { get; set; }
    public bool IsInIdleState { get; set; }
    public bool AggressionLimitsReached { get => aggressionLimitsReached; set => aggressionLimitsReached = value; }
    public bool AnAllyHasBeenAttacked { get => anAllyHasBeenAttacked; set => anAllyHasBeenAttacked = value; }

    #region Refs
    public NPCInteractions NPCInteractions => GetComponent<NPCInteractions>();
    public AggroRange AggroRange => GetComponentInChildren<AggroRange>();
    #endregion

    private void Start()
    {
        if (isACampNPC) ChangeState(new IdlingState());
        else ChangeState(new MovingState());
    }

    private void OnEnable()
    {
        Stats.OnDamageTaken += SetSourceOfDamageAsTarget;

        if (isACampNPC)
        {
            ChangeState(new IdlingState());
        }
        else ChangeState(new MovingState());
    }

    private void OnDisable()
    {
        Stats.OnDamageTaken -= SetSourceOfDamageAsTarget;
    }

    protected override void Update()
    {
        base.Update();
        currentState.OnUpdate();

        CurrentStateName = currentState.ToString(); // debugs
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);

        if(IsInIdleState)
        {
            //Debug.Log("Current state is IDLE");
            OnEnteringIdleState?.Invoke();
            OnAggroValueChanged?.Invoke(MaxAgroStep);
            AnAllyHasBeenAttacked = false;
        }
        else if (!IsInIdleState && !AnAllyHasBeenAttacked)
        {
            AnAllyHasBeenAttacked = true;
            OnExitingIdleState?.Invoke();
        }
    }

    private void Reset()
    {
        ChangeState(new MovingState());

        Stats.SourceOfDamage = null;
        Interactions.Target = null;

        Stats.CanTakeDamage = false;
    }

    #region Handle waypoints
    public virtual void CheckDistanceFromWaypoint(Transform waypoint)
    {
        if (Vector3.Distance(transform.position, waypoint.position) <= 1f)
        {
            if (WaypointIndex >= waypoints.Count) return;

            WaypointIndex++;
            waypointTarget = waypoints[WaypointIndex];
        }
    }
    #endregion

    #region Handle target and source of damage
    public void SetSourceOfDamageAsTarget()
    {
        //if an entity did damage to me it becomes my new current target...
        if (Stats.SourceOfDamage != null && Stats.SourceOfDamage.GetComponent<VisibilityState>().IsVisible && Stats.SourceOfDamage != NPCInteractions.Target)
        {
            if(isACampNPC)
            {
                if(!NPCInteractions.HasATarget)
                {
                    NPCInteractions.Target = Stats.SourceOfDamage;
                    ChangeState(new AttackingState());
                }
                else if(NPCInteractions.HasATarget)
                {
                    CompareTargetAndSourceOfDamagePositions();
                }
            }
        }
    }

    public void CompareTargetAndSourceOfDamagePositions()
    {
        EntityStats sourceOfDamageStats = Stats.SourceOfDamage.GetComponent<EntityStats>();
        VisibilityState sourceOfDamageVisibilityState = Stats.SourceOfDamage.GetComponent<VisibilityState>();

        //if an entity did damage to me and its different from my current target then...
        if (Stats.SourceOfDamage != null && Stats.SourceOfDamage != NPCInteractions.Target)
        {
            //first make sure that this entity (not my target) is not dead and still visible - We directly check the opposite and return if it is true.
            if (sourceOfDamageStats.IsDead || !sourceOfDamageVisibilityState.IsVisible)
            {
                Stats.SourceOfDamage = null;
                return;
            }

            float distanceWithCurrentTarget = Vector3.Distance(transform.position, NPCInteractions.Target.position);
            float distanceWithSourceOfDamage = Vector3.Distance(transform.position, Stats.SourceOfDamage.position);

            //else if the entity did damage to me and is nearer than my actual target, this entity becomes my new current target.
            if (distanceWithCurrentTarget <= distanceWithSourceOfDamage) return;

            if (distanceWithCurrentTarget > distanceWithSourceOfDamage)
            {
                //Debug.Log("Current target was too far, need to swap");
                NPCInteractions.Target = Stats.SourceOfDamage;
                AggroStep--;
                OnAggroValueChanged?.Invoke(AggroStep);

                if (AggroStep <= 0) Reset();
            }
        }
    }

    public void CompareCurrentPositionFromStartingPosition()
    {
        if (!isACampNPC) return;

        Vector3 currentPosition = transform.position;

        float distanceFromStartingDistance = DistanceBetweenAAndB(currentPosition, StartingPosition.position);

        if (distanceFromStartingDistance >= aggressionLimitsValue && !AggressionLimitsReached)
        {
            AggressionLimitsReached = true;
            StartCoroutine(DecreaseAggroStepOnReachingLimits(delayBeforeDecreasingAggroSteps));
            //Debug.Log("Distance from starting pos : " + distanceFromStartingDistance);
            //Debug.Log("2 meters away from starting position");
        }
    }

    IEnumerator DecreaseAggroStepOnReachingLimits(float tic)
    {
        do
        {
            yield return new WaitForSeconds(tic);

            //Debug.Log("Decreasing aggro steps");

            AggroStep--;
            OnAggroValueChanged?.Invoke(AggroStep);

            if (AggroStep <= 0) Reset();
        } while (AggroStep > 0);
    }

    private float DistanceBetweenAAndB(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    #endregion
}