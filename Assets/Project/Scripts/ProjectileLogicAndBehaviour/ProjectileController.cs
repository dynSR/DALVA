using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { TravelsForward, TravelsToAPosition, TravelsToATarget }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private GameObject onHitEffect;
    private bool canApplyDamage = false;

    private Stats projectileSenderCharacterStats;
    public Transform ProjectileSender { get; set; }
    public Transform Target { get; set; }

    private bool IsProjectileTravellingToATarget => ProjectileType == ProjectileType.TravelsToATarget;

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if(!IsProjectileTravellingToATarget)
            StartCoroutine(DestroyProjectileAfterTime());
    }

    void FixedUpdate()
    {
        switch (ProjectileType)
        {
            case ProjectileType.TravelsForward:
                ProjectileTravelsForward(ProjectileSender, projectileSenderCharacterStats);
                break;
            //case ProjectileType.TravelsToAPosition:
            //    break;
            case ProjectileType.TravelsToATarget:
                ProjectileTravelsToATarget(Target, ProjectileSender, projectileSenderCharacterStats);
                break;
            default:
                break;
        }
    }

    #region Projectile Behaviour
    void ProjectileTravelsForward(Transform sender, Stats projectileSenderStats)
    {
        ProjectileSender = sender;
        projectileSenderCharacterStats = projectileSenderStats;
        rb.MovePosition(transform.position += transform.forward * projectileSpeed);
    }

    void ProjectileTravelsToATarget(Transform target, Transform sender, Stats projectileSenderStats)
    {
        ProjectileSender = sender;
        projectileSenderCharacterStats = projectileSenderStats;
        rb.MovePosition(Vector3.Lerp(sender.position, target.transform.position, projectileSpeed * Time.deltaTime));
    }
    #endregion

    #region OnDestroy Projectile
    private void OnTriggerEnter(Collider other)
    {
        InstantiateHitEffect(onHitEffect);

        if (other.CompareTag("Enemy"))
        {
            if (ProjectileType == ProjectileType.TravelsToATarget && Target != null && other.transform == Target.transform)
            {
                canApplyDamage = true;
            }
            else if (ProjectileType == ProjectileType.TravelsForward || ProjectileType == ProjectileType.TravelsToAPosition)
            {
                canApplyDamage = true;
            }

            if (canApplyDamage)
            {
                Stats targetStats = GetComponent<Stats>();

                if (targetStats != null)
                {
                    targetStats.TakeDamage(
                        projectileSenderCharacterStats.CurrentAttackDamage,
                        projectileSenderCharacterStats.CurrentMagicDamage,
                        projectileSenderCharacterStats.CurrentCriticalStrikeChance,
                        projectileSenderCharacterStats.CurrentCriticalStrikeMultiplier,
                        projectileSenderCharacterStats.CurrentArmorPenetration,
                        projectileSenderCharacterStats.CurrentMagicResistancePenetration);
                }
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
