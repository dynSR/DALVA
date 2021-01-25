using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class AutoAttackProjectileController : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject onHitEffect;
    [SerializeField] private Transform projectileSender;
    [SerializeField] private Transform target;

    [SerializeField] private Stats targetStats;
    Vector3 targetPosition;

    public Transform ProjectileSender { get => projectileSender; set => projectileSender = value; }
    public Transform Target { get => target; set => target = value; }
    public Stats ProjectileSenderCharacterStats => ProjectileSender.GetComponent<Stats>();
    public Stats TargetCharacterStats { get => targetStats; set => targetStats = value; }

    private Rigidbody Rb => GetComponent<Rigidbody>();

    void Start()
    {
        if (Target != null)
            TargetCharacterStats = Target.GetComponent<Stats>();

        if (Target == null)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        ProjectileMoveToTarget();
    }

    void ProjectileMoveToTarget()
    {
        if (Target == null) return;
            
        targetPosition = Target.position;
        
        Rb.MovePosition(Vector3.MoveTowards(transform.position,
            targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0),
            projectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
    }

    private void OnTriggerEnter(Collider other)
    {
        InstantiateHitEffect(onHitEffect);

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy touched !");

            Stats targetStats = other.gameObject.GetComponent<Stats>();

            if (targetStats != null)
            {
                Debug.Log("Projectile Applies Damage !");
                other.gameObject.GetComponent<Stats>().TakeDamage(
                    ProjectileSenderCharacterStats.CurrentAttackDamage,
                    ProjectileSenderCharacterStats.CurrentMagicDamage,
                    ProjectileSenderCharacterStats.CurrentCriticalStrikeChance,
                    ProjectileSenderCharacterStats.CurrentCriticalStrikeMultiplier,
                    ProjectileSenderCharacterStats.CurrentArmorPenetration,
                    ProjectileSenderCharacterStats.CurrentMagicResistancePenetration);
            }
        }

        Destroy(gameObject);
    }

    void InstantiateHitEffect(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }
}
