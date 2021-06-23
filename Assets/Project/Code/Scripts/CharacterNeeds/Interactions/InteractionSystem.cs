using DarkTonic.MasterAudio;
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
    public Transform QueuedTarget;
    public Transform LastKnownTarget { get; set; }

    [Header("INTERACTIONS PARAMETERS")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private float rotationSpeed = 0.075f;
    [SerializeField] private float interactionRange = 1.5f;

    [Header("BASIC ATTACK")]
    [SerializeField] private bool autoAttackCanMark = false;
    [SerializeField] private StatusEffect autoAttackEffect;
    [SerializeField] private Transform rangedAttackEmiterPosition;
    [SerializeField] private GameObject rangedAttackProjectile;
    float pPBonus = 0;
    float mPBonus = 0;

    [Header("INTERACTIONS STATE")]
    [SerializeField] private bool canPerformAttack = true;
    [SerializeField] private bool hasPerformedAttack = false;
    public bool IsAttacking = false;
    public bool NeedToMove = false;
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }
    public bool HasPerformedAttack { get => hasPerformedAttack; set => hasPerformedAttack = value; }
    public float InteractionRange { get => interactionRange; set => interactionRange = value; }
    public bool AutoAttackCanMark { get => autoAttackCanMark; set => autoAttackCanMark = value; }
    public StatusEffect AutoAttackEffect { get => autoAttackEffect; set => autoAttackEffect = value; }

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
                KnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(KnownTarget.GetComponent<EntityDetection>().Outline);
                KnownTarget = null;
            } 
           
            return;
        }

        if (Controller.IsStunned || Controller.IsRooted) return;

        if (Target == null && IsAttacking)
        {
            IsAttacking = false;
        }

        if (Target != null)
        {
            distance = Vector3.Distance(transform.position, Target.position);

            if (distance > StoppingDistance)
            {
                //Debug.Log("Far from target");
                if (CanPerformAttack)
                    ResetInteractionState();

                NeedToMove = true;
                IsAttacking = false;

                if (Controller.Agent.enabled)
                {
                    Controller.Agent.isStopped = false;
                    Controller.SetAgentDestination(Controller.Agent, Target.position);
                }
            }
            else if (distance <= StoppingDistance)
            {
                //Debug.Log("Close enough to target");

                NeedToMove = false;

                if (Controller.Agent.enabled)
                {
                    Controller.Agent.ResetPath();
                    Controller.Agent.isStopped = true;
                }
                
                Interact();
            }
        }
    }

    #region Moving to a target
    public void MoveTowardsAnExistingTarget(Transform _target, float minDistance)
    {
        //Debug.Log(_target.name);
        Debug.Log("Moving Towards Target");

        if (_target != null)
        {
            Controller.HandleCharacterRotationBeforeCasting(transform, _target.position, Controller.RotateVelocity, Controller.RotationSpeed);

            Controller.Agent.stoppingDistance = minDistance;
        }
    }
    #endregion

    #region Interaction
    public virtual void Interact()
    {
        EntityStats targetStats = Target.GetComponent<EntityStats>();

        if (targetStats != null && targetStats.IsDead || Target == null)
        {
            ResetInteractionState();

            if(Target == Stats.SourceOfDamage)
                Stats.SourceOfDamage = null;

            Target = null;

            if (IsAttacking) IsAttacking = false;

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
            EntityStats targetStats = Target.GetComponent<EntityStats>();

            if (CanPerformAttack && !IsAttacking)
            {
                if (targetStats != null
                    && targetStats.EntityTeam != Stats.EntityTeam
                    && !targetStats.IsDead
                    && (targetFound.ThisTargetIsAPlayer(targetFound)
                    || targetFound.ThisTargetIsAMinion(targetFound)
                    || targetFound.ThisTargetIsAMonster(targetFound)))
                {
                    StartCoroutine(AttackInterval(Target));
                    Debug.Log("Attack performed on entity!");
                }
            }
        }
    }

    IEnumerator AttackInterval(Transform target)
    {
        Debug.Log("Attack Interval");
        Controller.CanMove = false;

        Controller.HandleCharacterRotationBeforeCasting(transform, target.position, Controller.RotateVelocity, Controller.RotationSpeed);

        Animator.SetFloat("AttackSpeed", Stats.GetStat(StatType.AttackSpeed).Value);

        Animator.SetBool("Attack", true);
        Animator.SetLayerWeight(1, 1);

        //MeleeAttack(); //Debug without animation

        IsAttacking = true;
        CanPerformAttack = false;

        yield return new WaitForSeconds(1 / Stats.GetStat(StatType.AttackSpeed).Value);
    }

    #region Behaviours of every type of attack - Melee / Ranged
    public void MeleeAttack()
    {
        if (Stats.IsDead || Target == null) return;

        EntityStats targetStat = Target.GetComponent<EntityStats>();

        if (targetStat != null)
        {
            //Debug.Log("Melee Attack");

            pPBonus = Stats.GetStat(StatType.BonusPhysicalPower).Value;

            float totalPhysicalDamage = Stats.GetStat(StatType.PhysicalPower).Value + pPBonus;
            float totalMagicalDamage = Stats.GetStat(StatType.MagicalPower).Value + mPBonus;

            if (Stats.GetStat(StatType.DamageReduction).Value > 0)
            {
                totalPhysicalDamage -= totalPhysicalDamage * Stats.GetStat(StatType.DamageReduction).Value;
                totalMagicalDamage -= totalMagicalDamage * Stats.GetStat(StatType.DamageReduction).Value;

                //Debug.Log("REDUCING DAMAGE", transform);
                //Debug.Log(totalPhysicalDamage, transform);
                //Debug.Log(totalMagicalDamage, transform);
            }

            if (targetStat.GetStat(StatType.IncreasedDamageTaken).Value > 0)
            {
                totalPhysicalDamage += totalPhysicalDamage * Stats.GetStat(StatType.IncreasedDamageTaken).Value;
                totalMagicalDamage += totalMagicalDamage * Stats.GetStat(StatType.IncreasedDamageTaken).Value;

                //Debug.Log("AUGMENTING DAMAGE", transform);
            }

            targetStat.TakeDamage(
                transform,
                targetStat.GetStat(StatType.PhysicalResistances).Value,
                targetStat.GetStat(StatType.MagicalResistances).Value,
                totalPhysicalDamage,
                totalMagicalDamage,
                Stats.GetStat(StatType.CriticalStrikeChance).Value,
                175f,
                Stats.GetStat(StatType.PhysicalPenetration).Value,
                Stats.GetStat(StatType.MagicalPenetration).Value);

            //Peut être utilisé pour marquer les cibles avec des auto attaques de mélée ?_?
            //if (AutoAttackCanMark) targetStat.MarkEntity(0.5f, targetStat.EntityTeam);

            if (AutoAttackEffect != null) AutoAttackEffect.ApplyEffect(Target);

            UtilityClass.PlaySoundGroupImmediatly("SFX_SE_Minion_Impact", targetStat.transform);

            HasPerformedAttack = true;

            OnAttacking?.Invoke();
        }
    }

    public void RangedAttack()
    {
        if (Stats.IsDead) return;

        if (Target != null)
        {
            //Debug.Log("Ranged Attack");

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

            if (AutoAttackEffect != null) attackProjectile.ProjectileStatusEffect = AutoAttackEffect;

            HasPerformedAttack = true;

            OnAttacking?.Invoke();

            attackProjectile.TotalPhysicalDamage = Stats.GetStat(StatType.PhysicalPower).Value + pPBonus;
            attackProjectile.TotalMagicalDamage = Stats.GetStat(StatType.MagicalPower).Value + mPBonus;
        }
    }
    #endregion

    public virtual void ResetInteractionState()
    {
        Debug.Log("ResetInteractionState");

        if (!Controller.IsCasting && !Controller.IsRooted && !Controller.IsStunned)
            Controller.CanMove = true;

        if (!IsAttacking 
            || Target != null && Target.GetComponent<InteractiveBuilding>() != null
            || Target != null && Target.GetComponent<EntityStats>() != null && Target.GetComponent<EntityStats>().IsDead 
            || Target == null 
            || GetComponent<NPCController>() != null && GetComponent<NPCController>().IsACampNPC
            || NeedToMove)
        {
            Animator.SetLayerWeight(1, 0);
            Animator.SetBool("Attack", false);
        }

        CanPerformAttack = true;
        HasPerformedAttack = false;
    }
    #endregion
}