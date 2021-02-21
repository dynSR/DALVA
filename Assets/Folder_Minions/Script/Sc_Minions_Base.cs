using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sc_Minions_Base : MonoBehaviour
{
    [Header("Attribute")]
    public float minSpeed = 3.5f;
    public float maxSpeed = 6;
    public float turnSpeed = 10f;
    public float attackRange = 2;
    public float MyAttackRange { get; set; }

    [Header("Unity Setup Field")]
    public string enemyTag = "Enemy";
    public string enemyLayer = "Enemy";

    public Transform partToRotate;
    public NavMeshAgent agent;

    [SerializeField]
    private Transform target;
    private float rangeToDetectOther;
    public Transform[] waypointsToReach;
    private int wavePointIndex = 0;
    private IState currentState;
    public Transform Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }
    public Transform[] WaypointsToReach
    {
        get
        {
            return waypointsToReach;
        }

        set
        {
            waypointsToReach = value;
        }
    }

    public int WavePointIndex
    {
        get
        {
            return wavePointIndex;
        }

        set
        {
            wavePointIndex = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rangeToDetectOther = GetComponentInChildren<SphereCollider>().radius;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = minSpeed;
        MyAttackRange = attackRange;
        ChangeState(new WaypointState());
        //InvokeRepeating("SelectedTarget", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();

        if (Target == null)
        {
            //UpdateTarget();
            return;
        }
        Vector3 dir = Target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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

    /*void UpdateTarget()
    {
        Debug.Log("Enter in UpdateTarget");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= rangeToDetectOther)
        {
            target = nearestEnemy.transform;
            Debug.Log(target.name);
            ChangeState(new FollowState());
        }
        else
        {
            target = null;
        }
    }*/
}
