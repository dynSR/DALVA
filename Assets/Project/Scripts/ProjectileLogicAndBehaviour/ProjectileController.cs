using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ProjectileType { TravelsForward, TravelsToAPosition, TravelsToATarget }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;

    [SerializeField] private GameObject onHitEffect;
    [SerializeField] private Transform projectileSender;
    [SerializeField] private Transform target;

    [SerializeField] private Stats targetStats;

    public Transform ProjectileSender { get => projectileSender; set => projectileSender = value; }
    public Transform Target { get => target; set => target = value; }

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public Stats ProjectileSenderCharacterStats => ProjectileSender.GetComponent<Stats>();
    public Stats TargetCharacterStats { get => targetStats; set => targetStats = value; }

    private Rigidbody Rb => GetComponent<Rigidbody>();

    private void Awake()
    {
        if (TargetCharacterStats != null)
            TargetCharacterStats = Target.GetComponent<Stats>();
    }

    private void OnEnable()
    {
        if(projectileType == ProjectileType.TravelsForward || projectileType == ProjectileType.TravelsToAPosition)
            StartCoroutine(DestroyProjectileAfterTime());
    }

    void FixedUpdate()
    {
        if (projectileType == ProjectileType.TravelsToATarget)
        {
            Vector3 targetPosition = Target.position;
            Rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0), projectileSpeed * Time.fixedDeltaTime));
            transform.LookAt(Target);
        }
        else if (projectileType == ProjectileType.TravelsForward || projectileType == ProjectileType.TravelsToAPosition)
        {
            ProjectileTravelsForward(ProjectileSender);
        }
    }

    #region Projectile Behaviour
    void ProjectileTravelsForward(Transform sender)
    {
        ProjectileSender = sender;
        Rb.MovePosition(transform.position += transform.forward * (projectileSpeed * Time.fixedDeltaTime));
    }
    #endregion

    #region OnDestroy Projectile
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
        if(objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }

    IEnumerator DestroyProjectileAfterTime()
    {
        yield return new WaitForSeconds(projectileLifeTime);
        InstantiateHitEffect(onHitEffect);
        Destroy(gameObject);
    }
    #endregion
}
