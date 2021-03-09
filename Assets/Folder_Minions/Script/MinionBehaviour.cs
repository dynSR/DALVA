using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionBehaviour : MonoBehaviour
{
    [Header("Attribute")]
    [SerializeField] private float movementSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 0.1f;

    public float MyAttackRange /*{ get; set; }*/;

    public Transform _renderer;

    public List<Transform> waypoints;
    public IState currentState; //set to private after tests

    public Transform Target /*{ get; set; }*/;
    public List<Transform> Waypoints { get => waypoints; }
    public int WavePointIndex /*{ get; set; }*/;

    public NavMeshAgent Agent => GetComponent<NavMeshAgent>();
    public CharacterStats Stats => GetComponent<CharacterStats>();

    private void Awake()
    {
        GetGlobalWaypoints();
    }

    void Start()
    {
        //to set with stats -!-
        Agent.speed = movementSpeed;
        MyAttackRange = Stats.UsedCharacter.BaseAttackRange;
        //

        ChangeState(new WaypointState());
    }

    void Update()
    {
        currentState.Update();

        if (Target != null)
            UtilityClass.HandleCharacterRotation(transform, Target.position, 0, rotationSpeed);
    }

    public void ChangeState(IState newState)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    void GetGlobalWaypoints()
    {
        foreach (var item in MinionWaypointsManager.Instance.MinionsGlobalWaypoints)
        {
            waypoints.Add(item);
        }
    }
}