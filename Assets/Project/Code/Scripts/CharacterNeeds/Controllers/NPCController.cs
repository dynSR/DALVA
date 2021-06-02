using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : CharacterController
{
    public delegate void AgressionStateHandler();
    public event AgressionStateHandler OnEnteringIdleState;
    public event AgressionStateHandler OnExitingIdleState;
    public event AgressionStateHandler OnMovingToStartPosition;

    public delegate void AggroStepHandler(float aggroStep);
    public event AggroStepHandler OnAggroValueChanged;

    public int WaypointIndex { get; set; }
    public Transform waypointTarget = null; //public to debug
    public List<Transform> waypoints; //public to debug
    public float DistanceWithTarget { get; set; }

    public IState currentState; //set to private after tests
    public string CurrentStateName; //for tests purpose

    [Header("NPC TYPE")]
    [SerializeField] private bool isACampNPC = false;
    [SerializeField] private bool isABoss = false;

    [Header("FOREST CAMP NPCS ATTRIBUTES")]
    [SerializeField] private Transform startingPosition;
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
    public Transform StartingPosition { get => startingPosition; set => startingPosition = value; }
    public bool IsInIdleState { get; set; }
    public bool AggressionLimitsReached { get => aggressionLimitsReached; set => aggressionLimitsReached = value; }
    public bool AnAllyHasBeenAttacked { get => anAllyHasBeenAttacked; set => anAllyHasBeenAttacked = value; }
    public bool IsABoss { get => isABoss; set => isABoss = value; }
    public bool IsACampNPC { get => isACampNPC; set => isACampNPC = value; }
    public bool IsABossWaveMember { get; set; }

    public float AggressionLimitsValue { get => aggressionLimitsValue; set => aggressionLimitsValue = value; }

    #region Refs
    public NPCInteractions NPCInteractions => GetComponent<NPCInteractions>();
    public AggroRange AggroRange => GetComponentInChildren<AggroRange>();
    #endregion

    private void Start()
    {
        if (IsACampNPC) ChangeState(new IdlingState());
        else ChangeState(new MovingState());
    }

    private void OnEnable()
    {
        Stats.OnDamageTaken += SetSourceOfDamageAsTarget;
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

        if (IsACampNPC)
        {
            if (IsInIdleState)
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
    }

    private void Reset()
    {
        OnMovingToStartPosition?.Invoke();

        Debug.Log("Reset");

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
            if (WaypointIndex > waypoints.Count) return;

            WaypointIndex++;
            waypointTarget = waypoints[WaypointIndex];
        }
    }
    #endregion

    #region Handle target and source of damage
    public void SetSourceOfDamageAsTarget()
    {
        //if an entity did damage to me it becomes my new current target...
        if (Stats.SourceOfDamage != null && Stats.SourceOfDamage != NPCInteractions.Target)
        {
            EntityDetection sourceOfDamageED = Stats.SourceOfDamage.GetComponent<EntityDetection>();

            if (!NPCInteractions.HasATarget && !sourceOfDamageED.ThisTargetIsASteleEffect(sourceOfDamageED))
            {
                NPCInteractions.Target = Stats.SourceOfDamage;
                ChangeState(new AttackingState());
            }
        }
    }

    public void CompareCurrentPositionFromStartingPosition()
    {
        if (!IsACampNPC) return;

        Vector3 currentPosition = transform.position;

        float distanceFromStartingDistance = DistanceBetweenAAndB(currentPosition, StartingPosition.position);

        if (distanceFromStartingDistance >= AggressionLimitsValue && !AggressionLimitsReached)
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