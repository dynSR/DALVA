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

    public Ability Ability { get; set; }

    Vector3 targetPosition = Vector3.zero;
    List<Transform> nearTargets = new List<Transform>();

    public bool CanGoThroughTarget { get; set; }
    public bool CanHeal { get; set; }
    public bool CanBounce { get; set; }

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

    //private void Awake()
    //{
    //    if (Ability != null && Ability.AbilityStatusEffect != null) ProjectileStatusEffect = Ability.AbilityStatusEffect;
    //}

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

        if (ProjectileType == ProjectileType.TravelsToAPosition && Target != null && (Target.GetComponent<EntityStats>().IsDead || ProjectileSender == null)) Destroy(gameObject);
    }

    #region Projectile Behaviours
    void ProjectileMoveToATarget()
    {
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
                        else if (CanHeal && ProjectileSenderStats.EntityTeam == targetStats.EntityTeam) ApplyProjectileAbilityHeal(targetStats);
                    }
                    else
                    {
                        ApplyProjectileAutoAttackDamage(targetStats);
                    }
                }
            }
        }
        //else if (targetCollider.gameObject.GetComponent<SteleLogic>() != null)
        //{
        //    ApplyProjectileDamageToAnInteractiveBuilding(targetCollider);
        //}
    }

    private void ApplyProjectileAbilityDamage(EntityStats targetStat)
    {
        //Debug.Log("Has an ability");

        float markBonusDamage;
        float healthThresholdBonusDamage;

        if (targetStat.EntityIsMarked)
        {
            targetStat.EntityIsMarked = false;
            markBonusDamage = Ability.AbilityDamageBonusOnMarkedTarget;
        }
        else markBonusDamage = 0;

        if (Ability != null && Ability.AbilityAddedDamageOnTargetHealthThreshold > 0 && targetStat.HealthPercentage <= Ability.TargetHealthThreshold)
        {
            healthThresholdBonusDamage = Ability.AbilityAddedDamageOnTargetHealthThreshold;
        }
        else healthThresholdBonusDamage = 0;

        if (Ability.AbilityPhysicalDamage > 0)
        {
            TotalPhysicalDamage = Ability.AbilityPhysicalDamage + (ProjectileSenderStats.GetStat(StatType.PhysicalPower).Value * (Ability.AbilityPhysicalRatio + markBonusDamage + healthThresholdBonusDamage));
        } 
        else Ability.AbilityPhysicalDamage = 0;

        if (Ability.AbilityMagicalDamage > 0)
        {
           TotalMagicalDamage = Ability.AbilityMagicalDamage + (ProjectileSenderStats.GetStat(StatType.MagicalPower).Value * (Ability.AbilityMagicalRatio + markBonusDamage + healthThresholdBonusDamage));
        } 
        else Ability.AbilityMagicalDamage = 0;

        targetStat.TakeDamage(
        ProjectileSender,
        targetStat.GetStat(StatType.PhysicalResistances).Value,
        targetStat.GetStat(StatType.MagicalResistances).Value,
        TotalPhysicalDamage,
        TotalMagicalDamage,
        ProjectileSenderStats.GetStat(StatType.CriticalStrikeChance).Value,
        175f,
        ProjectileSenderStats.GetStat(StatType.PhysicalPenetration).Value,
        ProjectileSenderStats.GetStat(StatType.MagicalPenetration).Value,
        ProjectileSenderStats.GetStat(StatType.DamageReduction).Value);

        if (Ability.AbilityCanMark) StartCoroutine(targetStat.MarkEntity(Ability.AbilityMarkDuration, ProjectileSenderStats.EntityTeam));

        DestroyProjectile();
    }

    private void ApplyProjectileAbilityHeal(EntityStats targetStat)
    {
        if (!targetStat.EntityIsMarked) return;

        float markBonusHeal;

        if (targetStat.EntityIsMarked)
        {
            targetStat.EntityIsMarked = false;
            markBonusHeal = Ability.AbilityHealBonusOnMarkedTarget;
        }
        else markBonusHeal = 0;

        if (targetStat.EntityIsMarked)
            targetStat.EntityIsMarked = false;

        targetStat.Heal(targetStat.transform, Ability.AbilityHealValue + (
            ProjectileSenderStats.GetStat(StatType.MagicalPower).Value * (Ability.AbilityMagicalRatio + markBonusHeal)), 
            ProjectileSenderStats.GetStat(StatType.HealAndShieldEffectiveness).Value);

        DestroyProjectile();
    }

    private void ApplyProjectileAutoAttackDamage(EntityStats targetStat)
    {
        //Debug.Log("No ability");

        float markBonusDamage;

        if (targetStat.EntityIsMarked)
        {
            targetStat.EntityIsMarked = false;
            markBonusDamage = BonusProjectileDamageOnMarkedTarget;
        }
        else markBonusDamage = 0;

        if (ProjectileSenderStats.GetStat(StatType.PhysicalPower).Value > 0)
        {
            TotalPhysicalDamage = ProjectileSenderStats.GetStat(StatType.PhysicalPower).Value + markBonusDamage;
        }
        else TotalPhysicalDamage = 0;

        //Auto Attack Scales only on Physical Power
        //if (ProjectileSenderStats.GetStat(StatType.MagicalPower).Value > 0)
        //{
        //    TotalMagicalDamage = ProjectileSenderStats.GetStat(StatType.MagicalPower).Value + markBonusDamage;
        //}
        //else TotalMagicalDamage = 0;

        TotalMagicalDamage = 0;

        targetStat.TakeDamage(
            ProjectileSender,
            targetStat.GetStat(StatType.PhysicalResistances).Value,
            targetStat.GetStat(StatType.MagicalResistances).Value,
            TotalPhysicalDamage,
            TotalMagicalDamage,
            ProjectileSenderStats.GetStat(StatType.CriticalStrikeChance).Value,
            175f,
            ProjectileSenderStats.GetStat(StatType.PhysicalPenetration).Value,
            ProjectileSenderStats.GetStat(StatType.MagicalPenetration).Value, 
            ProjectileSenderStats.GetStat(StatType.DamageReduction).Value);

        //Peut être utilisé pour marquer les cibles avec des auto attaques de portée ?_?
        //if (ProjectileSenderStats.GetComponent<InteractionSystem>().AutoAttackCanMark) StartCoroutine(targetStat.MarkEntity(0.5f));

        //Debug.Log(TotalPhysicalDamage);
        //Debug.Log(TotalMagicalDamage);

        DestroyProjectile();
    }

    //private void ApplyProjectileDamageToAnInteractiveBuilding(Collider targetCollider)
    //{
    //    //targetCollider.gameObject.GetComponent<SteleLogic>().TakeDamage(ProjectileSender, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f);

    //    DestroyProjectile();
    //}

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

            if (nearTargetStats != null && Target != null && targetColliders.gameObject != Target.gameObject
                && !nearTargets.Contains(targetColliders.transform))
            {
                //nearTargets.Add(targetColliders.transform);

                if (CanHeal && ProjectileSenderStats.EntityTeam == nearTargetStats.EntityTeam 
                    && nearTargetStats.EntityIsMarked 
                    && nearTargetStats.GetStat(StatType.Health).Value < nearTargetStats.GetStat(StatType.Health).MaxValue)
                {
                    nearTargets.Add(targetColliders.transform);
                }
                else if (ProjectileSender.gameObject != targetColliders.gameObject && ProjectileSenderStats.EntityTeam != nearTargetStats.EntityTeam)
                {
                    nearTargets.Add(targetColliders.transform);

                    if (Ability.AbilityCanConsumeMark)
                        nearTargetStats.EntityIsMarked = false;
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