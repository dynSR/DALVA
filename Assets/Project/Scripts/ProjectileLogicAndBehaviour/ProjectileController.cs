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

    //private bool IsProjectileTravellingToATarget => ProjectileType == ProjectileType.TravelsToATarget;

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public Stats ProjectileSenderCharacterStats { get; set; }
    public Stats TargetCharacterStats { get => targetStats; set => targetStats = value; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        TargetCharacterStats = Target.GetComponent<Stats>();
    }

    private void OnEnable()
    {
        //if(!IsProjectileTravellingToATarget)
        //    StartCoroutine(DestroyProjectileAfterTime());
    }

    void FixedUpdate()
    {
        if (projectileType == ProjectileType.TravelsToATarget)
        {
            Vector3 targetPosition = Target.position;
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0), projectileSpeed * Time.fixedDeltaTime));
            transform.LookAt(Target);
        }
        else if (projectileType == ProjectileType.TravelsForward || projectileType == ProjectileType.TravelsToAPosition)
        {
            ProjectileTravelsForward(ProjectileSender, ProjectileSenderCharacterStats);
        }
    }

    #region Projectile Behaviour
    void ProjectileTravelsForward(Transform sender, Stats projectileSenderStats)
    {
        ProjectileSender = sender;
        ProjectileSenderCharacterStats = projectileSenderStats;
        rb.MovePosition(transform.position += transform.forward * projectileSpeed);
    }

    //void ProjectileTravelsToATarget(Transform target, Transform sender, Stats projectileSenderStats)
    //{
    //    target = Target;
    //    sender = ProjectileSender;
    //    projectileSenderCharacterStats = projectileSenderStats;
    //    rb.MovePosition(Vector3.Lerp(sender.position, target.transform.position, projectileSpeed * Time.deltaTime));
    //}
    #endregion

    #region OnDestroy Projectile
    private void OnTriggerEnter(Collider other)
    {
        InstantiateHitEffect(onHitEffect);

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy touched !");
            //    if (ProjectileType == ProjectileType.TravelsToATarget && Target != null && other.transform == Target.transform)
            //    {
            //        canApplyDamage = true;
            //    }
            //    else if (ProjectileType == ProjectileType.TravelsForward || ProjectileType == ProjectileType.TravelsToAPosition)
            //    {
            //        canApplyDamage = true;
            //    }


            TargetCharacterStats.TakeDamage(15, 0, 0, 175, 0, 0);

            //Stats targetStats = other.gameObject.GetComponent<Stats>();

            //if (targetStats != null)
            //{
            //    Debug.Log("Projectile Applies Damage !");
            //    other.gameObject.GetComponent<Stats>().TakeDamage(
            //        ProjectileSenderCharacterStats.CurrentAttackDamage,
            //        ProjectileSenderCharacterStats.CurrentMagicDamage,
            //        ProjectileSenderCharacterStats.CurrentCriticalStrikeChance,
            //        ProjectileSenderCharacterStats.CurrentCriticalStrikeMultiplier,
            //        ProjectileSenderCharacterStats.CurrentArmorPenetration,
            //        ProjectileSenderCharacterStats.CurrentMagicResistancePenetration);
            //}
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
