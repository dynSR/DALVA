using System.Collections;
using UnityEngine;

public enum CombatType { MeleeCombat, RangedCombat, None }

public class InteractionSystem : MonoBehaviour
{
    public delegate void AttackStateHandler();
    public event AttackStateHandler OnAttacking;

    [Header("TARGETS INFORMATIONS")]
    [SerializeField] private Transform target; //Est en publique pour debug
    [SerializeField] private Transform knownTarget; //Est en publique pour debug
    public Transform LastKnownTarget { get; set; }

    [Header("INTERACTIONS PARAMETERS")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private float rotationSpeed = 0.075f;
    [SerializeField] private float interactionRange = 1.5f;

    [Header("BASIC ATTACK")]
    [SerializeField] private Transform rangedAttackEmiterPosition;
    [SerializeField] private GameObject rangedAttackProjectile;
    float pPBonus = 0;
    float mPBonus = 0;

    [Header("INTERACTIONS STATE")]
    [SerializeField] private bool canPerformAttack = true;
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }
    public bool HasPerformedAttack { get; set; }
    public float InteractionRange { get => interactionRange; set => interactionRange = value; }

    public Transform Target { get => target; set => target = value; }
    public Transform KnownTarget { get => knownTarget; set => knownTarget = value; }

    public float distance; //put in private after tests
    public float StoppingDistance { get; set; }

    #region References
    protected EntityStats Stats => GetComponent<EntityStats>();
    protected CharacterController Controller => GetComponent<CharacterController>();
    protected Animator Animator { get => characterAnimator; }
    #endregion

    protected virtual void Update()
    {
        if (Stats.IsDead)
        {
            Target = null;
            LastKnownTarget = null;

            if (KnownTarget != null)
            {
                KnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(KnownTarget.GetComponent<Outline>());
                KnownTarget = null;
            } 
           
            return;
        }

        if (target != null)
            distance = Vector3.Distance(transform.position, target.position);

        MoveTowardsAnExistingTarget(Target, StoppingDistance);
    }

    #region Moving to a target
    public void MoveTowardsAnExistingTarget(Transform target, float minDistance)
    {
        if (target != null)
        {
            Controller.HandleCharacterRotation(transform, target.position, Controller.RotateVelocity, rotationSpeed);

            Controller.Agent.stoppingDistance = minDistance;

            if (distance > minDistance)
            {
                //Debug.Log("Far from target");
                ResetInteractionState();
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
        if (Target.GetComponent<EntityStats>() != null 
            && Target.GetComponent<EntityStats>().IsDead
            || !Target.GetComponent<EntityDetection>().enabled)
        {
            ResetInteractionState();

            if(Target == Stats.SourceOfDamage)
                Stats.SourceOfDamage = null;

            Target = null;

            Debug.Log("TARGET IS DEAD WHILE INTERACTING");
            return;
        }

        AttackInteraction();
    }

    void AttackInteraction()
    {
       if (Target != null)
       {
            EntityDetection targetFound = Target.GetComponent<EntityDetection>();
            InteractiveBuilding interactiveBuilding = Target.GetComponent<InteractiveBuilding>();

            if (interactiveBuilding != null && interactiveBuilding.EntityTeam == EntityTeam.NEUTRAL) return;

            if (CanPerformAttack)
            {
                //if its a building
                if (targetFound.ThisTargetIsAStele(targetFound) && interactiveBuilding != null && interactiveBuilding.EntityTeam != Stats.EntityTeam)
                {
                    StartCoroutine(AttackInterval());
                    Debug.Log("Attack performed on building !");
                }
                //else if its an entity
                else if (Target.GetComponent<EntityStats>() != null
                    && (targetFound.ThisTargetIsAPlayer(targetFound)
                    || targetFound.ThisTargetIsAMinion(targetFound)
                    || targetFound.ThisTargetIsAMonster(targetFound))
                    && Target.GetComponent<EntityStats>().EntityTeam != Stats.EntityTeam)
                {
                    StartCoroutine(AttackInterval());
                    Debug.Log("Attack performed on entity!");
                }
            }
       }
    }

    IEnumerator AttackInterval()
    {
        Debug.Log("Attack Interval");
        Controller.CanMove = false;

        Animator.SetFloat("AttackSpeed", Stats.GetStat(StatType.AttackSpeed).Value);
        Animator.SetBool("Attack", true);

        //MeleeAttack(); //Debug without animation

        CanPerformAttack = false;

        yield return new WaitForSeconds(1 / Stats.GetStat(StatType.AttackSpeed).Value);
    }

    #region Behaviours of every type of attack - Melee / Ranged
    public void MeleeAttack()
    {
        if (Target != null 
            && Target.GetComponent<EntityStats>() != null)
        {
            Debug.Log("Melee Attack");

            EntityStats targetStat = Target.GetComponent<EntityStats>();

            pPBonus = Stats.GetStat(StatType.BonusPhysicalPower).Value;
            mPBonus = Stats.GetStat(StatType.BonusMagicalPower).Value;

            targetStat.TakeDamage(
                transform,
                targetStat.GetStat(StatType.PhysicalResistances).Value,
                targetStat.GetStat(StatType.MagicalResistances).Value,
                Stats.GetStat(StatType.PhysicalPower).Value + pPBonus,
                Stats.GetStat(StatType.MagicalPower).Value + mPBonus,
                Stats.GetStat(StatType.CriticalStrikeChance).Value,
                175f,
                Stats.GetStat(StatType.PhysicalPenetration).Value,
                Stats.GetStat(StatType.MagicalPenetration).Value);

            HasPerformedAttack = true;

            OnAttacking?.Invoke();
        }
    }

    public void RangedAttack()
    {
        if (Target != null)
        {
            Debug.Log("Ranged Attack");

            pPBonus = 0;
            mPBonus = 0;

            GameObject autoAttackProjectile = Instantiate(rangedAttackProjectile, rangedAttackEmiterPosition.position, rangedAttackProjectile.transform.rotation);

            ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

            attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;

            attackProjectile.ProjectileSender = transform;
            attackProjectile.Target = Target;

            if (Stats.GetStat(StatType.BonusPhysicalPower) != null && Stats.GetStat(StatType.BonusPhysicalPower) != null)
            {
                pPBonus = Stats.GetStat(StatType.BonusPhysicalPower).Value;
                mPBonus = Stats.GetStat(StatType.BonusMagicalPower).Value;
            }
            
            HasPerformedAttack = true; 

            OnAttacking?.Invoke();

            attackProjectile.TotalPhysicalDamage = Stats.GetStat(StatType.PhysicalPower).Value + pPBonus;
            attackProjectile.TotalMagicalDamage = Stats.GetStat(StatType.MagicalPower).Value + mPBonus;
        }
    }
    #endregion

    public void AttackEvent()
    {
        OnAttacking?.Invoke();
    }

    public virtual void ResetInteractionState()
    {
        if (!Controller.IsCasting)
            Controller.CanMove = true;

        if (Target != null)
            Animator.SetTrigger("NoTarget");

        Animator.SetBool("Attack", false);
        CanPerformAttack = true;
        HasPerformedAttack = false;
    }
    #endregion
}