﻿using UnityEngine;
using UnityEngine.AI;

public enum ProjectileType { None, TravelsForward, TravelsToAPosition }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject onHitEffect;
    [SerializeField] private AudioClip onHitSound;

    [SerializeField] private Transform projectileSender;

    [SerializeField] private Transform target;
    [SerializeField] private CharacterStat targetStats;

    Vector3 targetPosition;

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public float ProjectileSpeed { get => projectileSpeed; }
    public GameObject OnHitEffect { get => onHitEffect; }
    
    public Transform Target { get => target; set => target = value; }
    public CharacterStat TargetCharacterStats { get => targetStats; set => targetStats = value; }

    public Transform ProjectileSender { get => projectileSender; set => projectileSender = value; }
    public CharacterStat ProjectileSenderCharacterStats => ProjectileSender.GetComponent<CharacterStat>();
  
    private Rigidbody Rb => GetComponent<Rigidbody>();

    private void Start()
    {
        if (ProjectileType == ProjectileType.TravelsToAPosition)
        {
            if (Target != null)
                TargetCharacterStats = Target.GetComponent<CharacterStat>();
        }
    }

    private void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjectileType.None:
                break;
            case ProjectileType.TravelsForward:
                ProjectileTravelsForward(ProjectileSender);
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
        if (Target.GetComponent<NavMeshAgent>())
        {
            targetPosition = Target.position;

            Rb.MovePosition(Vector3.MoveTowards(transform.position,
                targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0),
                ProjectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ProjectileTravelsForward(Transform sender)
    {
        ProjectileSender = sender;
        Rb.MovePosition(transform.position += transform.forward * (ProjectileSpeed * Time.fixedDeltaTime));
    }
    #endregion

    #region On hit behaviour
    private void OnTriggerEnter(Collider other)
    {
        OnProjectileDestruction();

        ApplyDamageOnTargetHit(other);
    }

    protected void ApplyDamageOnTargetHit(Collider targetCollider)
    {
        if (targetCollider.gameObject.GetComponent<CharacterStat>() != null)
        {
            Debug.Log("Enemy touched !");

            EntityDetection entityFound = targetCollider.gameObject.GetComponent<EntityDetection>();

            if (entityFound.TypeOfEntity == TypeOfEntity.Enemy)
            {
                Debug.Log("Projectile Applies Damage !");
                CharacterStat targetStat = targetCollider.GetComponent<CharacterStat>();

                targetStat.TakeDamage(
                    ProjectileSender,
                    targetStat.GetStat(StatType.Physical_Resistances).Value,
                    targetStat.GetStat(StatType.Magical_Resistances).Value,
                    ProjectileSenderCharacterStats.GetStat(StatType.Physical_Power).Value,
                    ProjectileSenderCharacterStats.GetStat(StatType.Magical_Power).Value,
                    ProjectileSenderCharacterStats.GetStat(StatType.Critical_Strike_Chance).Value,
                    175f,
                    ProjectileSenderCharacterStats.GetStat(StatType.Physical_Penetration).Value,
                    ProjectileSenderCharacterStats.GetStat(StatType.Magical_Penetration).Value);

                Destroy(gameObject);
            }
        }
    }

    protected void OnProjectileDestruction()
    {
        PlaySoundOnCollision();
        InstantiateHitEffectOnCollision(OnHitEffect);
    }

    private void PlaySoundOnCollision()
    {
        if (onHitSound != null)
            AudioSource.PlayClipAtPoint(onHitSound, transform.position);
    }

    private void InstantiateHitEffectOnCollision(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }
    #endregion
}