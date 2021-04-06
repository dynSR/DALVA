using UnityEngine;
using UnityEngine.AI;

public enum ProjectileType { None, TravelsForward, TravelsToAPosition }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject onHitEffect;

    [SerializeField] private Transform projectileSender; //debug

    [SerializeField] private Transform target; //debug
    [SerializeField] private EntityStats targetStats; //debug

    public Ability Ability { get; set; }

    Vector3 targetPosition;

    public float TotalPhysicalDamage { get; set; }
    public float TotalMagicalDamage { get; set; }

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public float ProjectileSpeed { get => projectileSpeed; }
    public GameObject OnHitEffect { get => onHitEffect; }
    
    public Transform Target { get => target; set => target = value; }

    public Transform ProjectileSender { get => projectileSender; set => projectileSender = value; }
    public EntityStats ProjectileSenderCharacterStats => ProjectileSender.GetComponent<EntityStats>();
  
    private Rigidbody Rb => GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjectileType.None:
                break;
            case ProjectileType.TravelsForward:
                ProjectileTravelsToPosition(ProjectileSender);
                break;
            case ProjectileType.TravelsToAPosition:
                ProjectileMoveToATarget();
                break;
            default:
                break;
        }
    }

    #region Projectile Behaviours
    void ProjectileMoveToATarget()
    {
        if (Target.GetComponent<NavMeshAgent>() != null)
        {
            targetPosition = Target.position;

            Rb.MovePosition(Vector3.MoveTowards(
                transform.position,
                targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0),
                ProjectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
        }
        else if (Target.GetComponent<NavMeshAgent>() == null)
        {
            targetPosition = Target.position;

            Rb.MovePosition(Vector3.MoveTowards(
                transform.position,
                targetPosition + new Vector3(0, 0.5f, 0),
                ProjectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ProjectileTravelsToPosition(Transform sender)
    {
        ProjectileSender = sender;
        //Rb.MovePosition(Vector3.MoveTowards(
        //    transform.position,
        //    Target.position,
        //    ProjectileSpeed * Time.fixedDeltaTime));
        Rb.MovePosition(transform.position += transform.forward * (ProjectileSpeed * Time.fixedDeltaTime));
    }
    #endregion

    #region On hit behaviour
    private void OnTriggerEnter(Collider other)
    {
        ApplyDamageOnTargetHit(other);
    }

    protected void ApplyDamageOnTargetHit(Collider targetCollider)
    {
        if (targetCollider.gameObject.GetComponent<EntityStats>() != null)
        {
            Debug.Log("Enemy touched !");

            EntityDetection entityFound = targetCollider.gameObject.GetComponent<EntityDetection>();
            EntityStats targetStat = targetCollider.GetComponent<EntityStats>();

            //Ability projectile damage appplication
            if (entityFound.TypeOfEntity == TypeOfEntity.EnemyPlayer
                || entityFound.TypeOfEntity == TypeOfEntity.EnemyMinion
                || entityFound.TypeOfEntity == TypeOfEntity.EnemyStele
                || entityFound.TypeOfEntity == TypeOfEntity.Monster)
            {
                Debug.Log("Projectile Applies Damage !");
                
                //Maybe it needs to add something else for the ratio added to projectile damage +% ????
                if (Ability != null)
                {
                    Debug.Log("Has an ability");
                    targetStat.TakeDamage(
                    ProjectileSender,
                    targetStat.GetStat(StatType.PhysicalResistances).Value,
                    targetStat.GetStat(StatType.MagicalResistances).Value,
                    Ability.AbilityPhysicalDamage + (ProjectileSenderCharacterStats.GetStat(StatType.PhysicalPower).Value * Ability.AbilityPhysicalRatio),
                    Ability.AbilityMagicalDamage + (ProjectileSenderCharacterStats.GetStat(StatType.MagicalPower).Value * Ability.AbilityMagicalRatio),
                    ProjectileSenderCharacterStats.GetStat(StatType.CriticalStrikeChance).Value,
                    175f,
                    ProjectileSenderCharacterStats.GetStat(StatType.PhysicalPenetration).Value,
                    ProjectileSenderCharacterStats.GetStat(StatType.MagicalPenetration).Value);

                    InstantiateHitEffectOnCollision(OnHitEffect);
                    Destroy(gameObject);
                }
                else if (Target != null && targetCollider.transform.gameObject == Target.gameObject)
                {
                    Debug.Log("No ability");

                    targetStat.TakeDamage(
                        ProjectileSender,
                        targetStat.GetStat(StatType.PhysicalResistances).Value,
                        targetStat.GetStat(StatType.MagicalResistances).Value,
                        TotalPhysicalDamage,
                        TotalMagicalDamage,
                        ProjectileSenderCharacterStats.GetStat(StatType.CriticalStrikeChance).Value,
                        175f,
                        ProjectileSenderCharacterStats.GetStat(StatType.PhysicalPenetration).Value,
                        ProjectileSenderCharacterStats.GetStat(StatType.MagicalPenetration).Value);

                    Debug.Log(TotalPhysicalDamage);
                    Debug.Log(TotalMagicalDamage);

                    InstantiateHitEffectOnCollision(OnHitEffect);
                    Destroy(gameObject);
                }
            }
        }
        else if (targetCollider.gameObject.GetComponent<SteleLogic>() != null)
        {
            targetCollider.gameObject.GetComponent<SteleLogic>().TakeDamage(ProjectileSender, 0, 0, 1, 0, 0, 0, 0, 0);

            InstantiateHitEffectOnCollision(OnHitEffect);
            Destroy(gameObject);
        }
    }

    private void InstantiateHitEffectOnCollision(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }
    #endregion
}