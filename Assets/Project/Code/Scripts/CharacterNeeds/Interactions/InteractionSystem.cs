using System.Collections;
using UnityEngine;

public enum CombatType { MeleeCombat, RangedCombat }

public class InteractionSystem : MonoBehaviour
{
    [Header("TARGETS INFORMATIONS")]
    [SerializeField] private Transform target; //Est en publique pour debug
    [SerializeField] private Transform knownTarget; //Est en publique pour debug

    [Header("INTERACTIONS PARAMETERS")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private float rotationSpeed = 0.075f;
    [SerializeField] private float interactionRange = 1.5f;

    [Header("BASIC ATTACK")]
    [SerializeField] private Transform rangedAttackEmiterPosition;
    [SerializeField] private GameObject rangedAttackProjectile;

    [Header("INTERACTIONS STATE")]
    [SerializeField] private bool canPerformAttack = true;
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }
    public CombatType CombatAttackType { get; set; }
    public float InteractionRange { get => interactionRange; set => interactionRange = value; }

    public Transform Target { get => target; set => target = value; }
    public Transform KnownTarget { get => knownTarget; set => knownTarget = value; }

    public float distance; //put in private after tests
    public float StoppingDistance { get; set; }

    #region References
    protected CharacterStat CharacterStats => GetComponent<CharacterStat>();
    protected CharacterController CharacterController => GetComponent<CharacterController>();
    protected Animator CharacterAnimator { get => characterAnimator; }
    #endregion

    protected virtual void Update()
    {
        if (CharacterStats.IsDead)
        {
            Target = null;
            return;
        }

        MoveTowardsAnExistingTarget(Target, StoppingDistance);
    }

    #region Moving to a target
    public void MoveTowardsAnExistingTarget(Transform target, float minDistance)
    {
        if (target != null)
        {
            CharacterController.HandleCharacterRotation(transform, target.position, CharacterController.RotateVelocity, rotationSpeed);

            distance = Vector3.Distance(transform.position, target.position);
            CharacterController.Agent.stoppingDistance = minDistance;

            if (distance > minDistance)
            {
                //Debug.Log("Far from target");
                CharacterController.Agent.isStopped = false;
                CharacterController.Agent.SetDestination(target.position);
            }
            else if (distance <= minDistance)
            {
                //Debug.Log("Close enough to target");
                CharacterController.Agent.ResetPath();
                CharacterController.Agent.isStopped = true;
                Interact();
            }
        }
    }
    #endregion

    #region Interaction
    public virtual void Interact()
    {
        if (Target.GetComponent<CharacterStat>() != null 
            && Target.GetComponent<CharacterStat>().IsDead)
        {
            ResetInteractionState();
            CharacterAnimator.SetTrigger("NoTarget");
            Target = null;
            return;
        }

        AttackInteraction();
    }

    void AttackInteraction()
    {
        if (Target != null)
        {
            if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy && CanPerformAttack)
            {
                CharacterAnimator.SetBool("IsCollecting", false);

                if (CombatAttackType == CombatType.MeleeCombat)
                {
                    StartCoroutine(AttackInterval(CombatAttackType));
                    //Debug.Log("Melee Attack performed !");
                }
                else if (CombatAttackType == CombatType.RangedCombat)
                {
                    StartCoroutine(AttackInterval(CombatAttackType));
                    //Debug.Log("Ranged Attack performed !");
                }
            }
        }
    }

    IEnumerator AttackInterval(CombatType attackType)
    {
        Debug.Log("Attack Interval");

        if (attackType == CombatType.MeleeCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.GetStat(StatType.Attack_Speed).Value);
            CharacterAnimator.SetBool("Attack", true);

            //MeleeAttack(); //Debug without animation

            CanPerformAttack = false;
        }
        else if (attackType == CombatType.RangedCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.GetStat(StatType.Attack_Speed).Value);
            CharacterAnimator.SetBool("Attack", true);

            //RangedAttack(); //Debug without animation

            CanPerformAttack = false;
        }

        yield return new WaitForSeconds(1 / CharacterStats.GetStat(StatType.Attack_Speed).Value);

        if (attackType == CombatType.MeleeCombat)
        {
            CharacterAnimator.SetBool("Attack", false);
            CanPerformAttack = true;
        }
        else if (attackType == CombatType.RangedCombat)
        {
            CharacterAnimator.SetBool("Attack", false);
            CanPerformAttack = true;
        }
    }

    #region Behaviours of every type of attack - Melee / Ranged
    public void MeleeAttack()
    {
        if (Target != null 
            && Target.GetComponent<CharacterStat>() != null)
        {
            CharacterStat targetStat = Target.GetComponent<CharacterStat>();

            targetStat.TakeDamage(
                transform,
                targetStat.GetStat(StatType.Health).Value,
                targetStat.GetStat(StatType.Physical_Resistances).Value,
                targetStat.GetStat(StatType.Magical_Resistances).Value,
                CharacterStats.GetStat(StatType.Physical_Power).Value,
                CharacterStats.GetStat(StatType.Magical_Power).Value,
                CharacterStats.GetStat(StatType.Critical_Strike_Chance).Value,
                175f,
                CharacterStats.GetStat(StatType.Physical_Penetration).Value,
                CharacterStats.GetStat(StatType.Magical_Penetration).Value);
        }

        CanPerformAttack = true;
    }

    public void RangedAttack()
    {
        if (Target != null)
        {
            //Debug.Log("Auto Attack Projectile Instantiated");

            GameObject autoAttackProjectile = Instantiate(rangedAttackProjectile, rangedAttackEmiterPosition.position, rangedAttackProjectile.transform.rotation);

            ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

            attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;
            attackProjectile.ProjectileSender = transform;
            attackProjectile.Target = Target;
        }

        CanPerformAttack = true;
    }
    #endregion

    public virtual void ResetInteractionState()
    {
        CanPerformAttack = true;
        CharacterAnimator.SetBool("Attack", false);
    }
    #endregion
}