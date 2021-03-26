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
    protected CharacterStat Stats => GetComponent<CharacterStat>();
    protected CharacterController Controller => GetComponent<CharacterController>();
    protected Animator Animator { get => characterAnimator; }
    #endregion

    protected virtual void Update()
    {
        if (Stats.IsDead)
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
            Controller.HandleCharacterRotation(transform, target.position, Controller.RotateVelocity, rotationSpeed);

            distance = Vector3.Distance(transform.position, target.position);
            Controller.Agent.stoppingDistance = minDistance;

            if (distance > minDistance)
            {
                //Debug.Log("Far from target");
                Controller.Agent.isStopped = false;
                Controller.SetAgentDestination(Controller.Agent, target.position);
            }
            else if (distance <= minDistance)
            {
                //Debug.Log("Close enough to target");
                Controller.Agent.ResetPath();
                Controller.Agent.isStopped = true;
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
            Target = null;

            ResetInteractionState();

            Debug.Log("TARGET IS DEAD WHILE INTERACTING");
            return;
        }

        AttackInteraction();
    }

    void AttackInteraction()
    {
        if (Target != null)
        {
            if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy 
                && CanPerformAttack)
            {
                StartCoroutine(AttackInterval());
                Debug.Log("Attack performed !");
            }
        }
    }

    IEnumerator AttackInterval()
    {
        Debug.Log("Attack Interval");
        Controller.CanMove = false;

        Animator.SetFloat("AttackSpeed", Stats.GetStat(StatType.Attack_Speed).Value);
        Animator.SetBool("Attack", true);

        //MeleeAttack(); //Debug without animation

        CanPerformAttack = false;

        yield return new WaitForSeconds(1 / Stats.GetStat(StatType.Attack_Speed).Value);
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
                targetStat.GetStat(StatType.Physical_Resistances).Value,
                targetStat.GetStat(StatType.Magical_Resistances).Value,
                Stats.GetStat(StatType.Physical_Power).Value,
                Stats.GetStat(StatType.Magical_Power).Value,
                Stats.GetStat(StatType.Critical_Strike_Chance).Value,
                175f,
                Stats.GetStat(StatType.Physical_Penetration).Value,
                Stats.GetStat(StatType.Magical_Penetration).Value);
        }
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
    }
    #endregion

    public virtual void ResetInteractionState()
    {
        if (!Controller.IsCasting)
            Controller.CanMove = true;

        Animator.SetTrigger("NoTarget");

        Animator.SetBool("Attack", false);
        CanPerformAttack = true;
    }
    #endregion
}