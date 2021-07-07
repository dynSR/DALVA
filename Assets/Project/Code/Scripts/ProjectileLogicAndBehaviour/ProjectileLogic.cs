using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ProjectileType { None, TravelsForward, TravelsToAPosition }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType = ProjectileType.None;
    [SerializeField] private float projectileSpeed = 0f;
    [SerializeField] private float projectileAreaOfEffect = 0f;
    [SerializeField] private LayerMask entitiesLayerMaskValue;

    [SerializeField] private GameObject subProjectile;
    [SerializeField] private StatusEffect projectileStatusEffect;
    [SerializeField] private GameObject onHitVFX;
    [SerializeField] private GameObject destructionVFX;
    [SerializeField] private GameObject damageZone;

    public Ability Ability { get; set; }

    Vector3 targetPosition = Vector3.zero;
    List<Transform> nearTargets = new List<Transform>();

    public bool CanGoThroughTarget { get; set; }
    public bool CanHeal { get; set; }
    public bool CanBounce { get; set; }
    public bool CanApplyDamageInZone { get; set; }

    public float TotalPhysicalDamage { get; set; }
    public float TotalMagicalDamage { get; set; }

    public float BonusProjectileDamageOnMarkedTarget { get; set; }

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public float ProjectileSpeed { get => projectileSpeed; }
    public StatusEffect ProjectileStatusEffect { get => projectileStatusEffect; set => projectileStatusEffect = value; }
    public GameObject OnHitVFX { get => onHitVFX; }
    public GameObject DestructionVFX { get => destructionVFX; }

    public Transform Target { get; set; }

    public Transform ProjectileSender { get; set; }
    public EntityStats ProjectileSenderStats => ProjectileSender.GetComponent<EntityStats>();
  
    private Rigidbody Rb => GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (Target != null)
            targetPosition = Target.position;

        switch (projectileType)
        {
            case ProjectileType.TravelsForward:
                ProjectileTravelsToPosition(ProjectileSender);
                break;
            case ProjectileType.TravelsToAPosition:
                ProjectileMoveToATarget();
                break;
        }
    }

    #region Projectile Behaviours
    void ProjectileMoveToATarget()
    {
        if(Target == null 
            || ProjectileSender == null 
            || Target.GetComponent<EntityStats>() == null 
            || Target.GetComponent<EntityStats>().IsDead)
        {
            Destroy(gameObject);
            return;
        }

        Rb.MovePosition(Vector3.MoveTowards(
            transform.position,
            targetPosition + new Vector3(0, Target.localScale.y / 2, 0),
            ProjectileSpeed * Time.fixedDeltaTime));

        transform.LookAt(Target);
    }

    void ProjectileTravelsToPosition(Transform sender)
    {
        ProjectileSender = sender;
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
            //Debug.Log("Enemy touched !");

            EntityDetection entityFound = targetCollider.gameObject.GetComponent<EntityDetection>();
            EntityStats targetStats = targetCollider.GetComponent<EntityStats>();

            //Ability projectile damage appplication
            //Needs to be modified to only include Player - Interactive building - Monster - Minion
            if (entityFound.ThisTargetIsAPlayer(entityFound)
                || entityFound.ThisTargetIsAMinion(entityFound)
                || entityFound.ThisTargetIsAStele(entityFound)
                || entityFound.ThisTargetIsAMonster(entityFound))
            {
                //Debug.Log("Projectile Applies Damage !");

                if (Target == null)
                {
                    targetStats = targetCollider.GetComponent<EntityStats>();
                    Target = targetCollider.transform;
                }

                ApplyStatusEffectOnHit(targetCollider);

                if (Target != null && targetCollider.transform.gameObject == Target.gameObject)
                {
                    if (Ability != null)
                    {
                        if (ProjectileSenderStats.EntityTeam != targetStats.EntityTeam) ApplyProjectileAbilityDamage(targetStats);
                    }
                    else
                    {
                        ApplyProjectileAutoAttackDamage(targetStats);
                    }
                }
            }
        }
    }

    private void ApplyProjectileAbilityDamage(EntityStats targetStat)
    {
        //Debug.Log("Has an ability");

        float markBonusDamage;
        float healthThresholdBonusDamage;

        if (targetStat.EntityIsMarked)
        {
            CanBounce = true;
            markBonusDamage = Ability.AbilityDamageBonusOnMarkedTarget;
        }
        else
        {
            CanBounce = false;
            markBonusDamage = 0;
        }
        if (Ability != null && Ability.AbilityAddedDamageOnTargetHealthThreshold > 0 && targetStat.HealthPercentage <= Ability.TargetHealthThreshold)
        {
            healthThresholdBonusDamage = Ability.AbilityAddedDamageOnTargetHealthThreshold;
        }
        else healthThresholdBonusDamage = 0;
        
        if (Ability.AbilityMagicalDamage > 0)
        {
           TotalMagicalDamage = Ability.AbilityMagicalDamage + (ProjectileSenderStats.GetStat(StatType.MagicalPower).Value * (Ability.AbilityMagicalRatio + markBonusDamage + healthThresholdBonusDamage));
        } 
        else Ability.AbilityMagicalDamage = 0;

        if (ProjectileSenderStats.GetStat(StatType.DamageReduction).Value > 0)
        {
            TotalMagicalDamage -= TotalMagicalDamage * ProjectileSenderStats.GetStat(StatType.DamageReduction).Value;

            Debug.Log("REDUCING DAMAGE", transform);
        }

        if (targetStat.GetStat(StatType.IncreasedDamageTaken).Value > 0)
        {
            TotalMagicalDamage += TotalMagicalDamage * ProjectileSenderStats.GetStat(StatType.IncreasedDamageTaken).Value;

            Debug.Log("AUGMENTING DAMAGE", transform);
        }

        targetStat.TakeDamage(
        ProjectileSender,
        targetStat.GetStat(StatType.PhysicalResistances).Value,
        targetStat.GetStat(StatType.MagicalResistances).Value,
        0,
        TotalMagicalDamage,
        ProjectileSenderStats.GetStat(StatType.CriticalStrikeChance).Value,
        175f,
        ProjectileSenderStats.GetStat(StatType.PhysicalPenetration).Value,
        ProjectileSenderStats.GetStat(StatType.MagicalPenetration).Value);

        if (Ability.AbilityCanConsumeMark && targetStat.EntityIsMarked)
        {
            targetStat.DeactivateMarkFeedback();
            targetStat.ConsumeMark();
        }

        if (Ability.AbilityCanMark) StartCoroutine(targetStat.MarkEntity(Ability.AbilityMarkDuration));

        DestroyProjectile();
    }

    private void ApplyProjectileAutoAttackDamage(EntityStats targetStat)
    {
        //Debug.Log("No ability");

        if (ProjectileSenderStats.GetStat(StatType.PhysicalPower).Value > 0)
        {
            TotalPhysicalDamage = ProjectileSenderStats.GetStat(StatType.PhysicalPower).Value;
        }
        else TotalPhysicalDamage = 0;

        /*if (ProjectileSenderStats.GetStat(StatType.MagicalPower).Value > 0)
        {
            TotalMagicalDamage = ProjectileSenderStats.GetStat(StatType.MagicalPower).Value;
        }
        else TotalMagicalDamage = 0;*/

        TotalMagicalDamage = 0;

        if (ProjectileSenderStats.GetStat(StatType.DamageReduction).Value > 0)
        {
            TotalPhysicalDamage -= TotalPhysicalDamage * ProjectileSenderStats.GetStat(StatType.DamageReduction).Value;
            TotalMagicalDamage -= TotalMagicalDamage * ProjectileSenderStats.GetStat(StatType.DamageReduction).Value;

            Debug.Log("REDUCING DAMAGE", transform);
        }

        if (targetStat.GetStat(StatType.IncreasedDamageTaken).Value > 0)
        {
            TotalPhysicalDamage += TotalPhysicalDamage * ProjectileSenderStats.GetStat(StatType.IncreasedDamageTaken).Value;
            TotalMagicalDamage -= TotalMagicalDamage * ProjectileSenderStats.GetStat(StatType.DamageReduction).Value;

            Debug.Log("AUGMENTING DAMAGE", transform);
        }

        targetStat.TakeDamage(
            ProjectileSender,
            targetStat.GetStat(StatType.PhysicalResistances).Value,
            targetStat.GetStat(StatType.MagicalResistances).Value,
            TotalPhysicalDamage,
            TotalMagicalDamage,
            ProjectileSenderStats.GetStat(StatType.CriticalStrikeChance).Value,
            175f,
            ProjectileSenderStats.GetStat(StatType.PhysicalPenetration).Value,
            ProjectileSenderStats.GetStat(StatType.MagicalPenetration).Value);

        DestroyProjectile();
    }

    protected void ApplyStatusEffectOnHit(Collider targetCollider)
    {
        StatusEffectHandler targetStatusEffectHandler = targetCollider.GetComponent<StatusEffectHandler>();

        if (ProjectileStatusEffect != null && targetStatusEffectHandler != null)
        {
            ProjectileStatusEffect.ApplyEffect(targetCollider.transform);
        }
    }

    public void SetProjectileStatusEffect(StatusEffect effectToAttribute)
    {
        ProjectileStatusEffect = effectToAttribute;
    }

    protected void BounceOnOtherNearTarget()
    {
        //Find other near targets and for each of them go to it
        //Then if it hits the last nearer target, destroy

        Debug.Log("Bounce");

        Collider[] collidersFound = Physics.OverlapSphere(transform.position, projectileAreaOfEffect, entitiesLayerMaskValue);

        Debug.Log(collidersFound.Length);

        for (int i = 0; i < collidersFound.Length; i++)
        {
            Debug.Log(collidersFound[i].name);
        }

        foreach (Collider targetColliders in collidersFound)
        {
            EntityStats nearTargetStats = targetColliders.GetComponent<EntityStats>();

            if (nearTargetStats != null && Target != null && !nearTargets.Contains(targetColliders.transform))
            {
                if (Target.gameObject != targetColliders.gameObject 
                    && ProjectileSenderStats.EntityTeam != nearTargetStats.EntityTeam)
                {
                    nearTargets.Add(targetColliders.transform);

                    if (Ability.AbilityCanConsumeMark && nearTargetStats.EntityIsMarked)
                    {
                        nearTargetStats.DeactivateMarkFeedback();
                        nearTargetStats.ConsumeMark();
                    }  
                }
            }

            Debug.Log(nearTargets.Count);
        }

        for (int i = 0; i < nearTargets.Count; i++)
        {
            if (nearTargets.Count >= 1)
            {
                GameObject projectileInstance = Instantiate(subProjectile, new Vector3(Target.position.x, Target.position.y, Target.position.z + 0.15f), Quaternion.identity);

                ProjectileLogic _projectile = projectileInstance.GetComponent<ProjectileLogic>();
                _projectile.ProjectileSender = ProjectileSender;
                _projectile.Ability = Ability;
                _projectile.CanHeal = CanHeal;
                _projectile.Target = nearTargets[i];
            }
        }
    }

    private void DestroyProjectile()
    {
        if (!CanGoThroughTarget && !CanBounce)
            InstantiateHitEffectOnCollision(OnHitVFX);

        if (CanBounce)
        {
            BounceOnOtherNearTarget();
            Destroy(gameObject);
        }
        else if (!CanGoThroughTarget) Destroy(gameObject);

        if(CanApplyDamageInZone && damageZone != null)
        {
            Instantiate(damageZone, new Vector3(transform.position.x, 0.1f, transform.position.z), damageZone.transform.rotation);

            SentinelProjectileDamageZone sentinelProjectileDamageZone = damageZone.GetComponent<SentinelProjectileDamageZone>();
            sentinelProjectileDamageZone.projectile = this;
            sentinelProjectileDamageZone.projectileTarget = Target;

           //damageZone.SetActive(true);
           Destroy(gameObject);
        }
    }

    private void InstantiateHitEffectOnCollision(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }

    public void SpawnDestructionEffect(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }
    #endregion
}