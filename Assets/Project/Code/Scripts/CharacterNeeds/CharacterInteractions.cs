using System.Collections;
using UnityEngine;

public enum CombatType { MeleeCombat, RangedCombat }

public class CharacterInteractions : MonoBehaviour
{
    [Header("TARGETS INFORMATIONS")]
    [SerializeField] private Transform target; //Est en publique pour debug
    [SerializeField] private Transform knownTarget;

    [Header("BASIC ATTACK")]
    [SerializeField] private Transform attackEmiterPosition;
    [SerializeField] private GameObject rangedAttackProjectile;

    [Header("COMBAT PARAMETERS")]
    [SerializeField] private float rotateSpeedBeforeAttacking = 0.075f;
    [SerializeField] private bool canPerformAttack = true;

    [Header("INTERACTIONS PARAMETERS")]
    [SerializeField] private float interactionRange = 1.5f;
    public bool isCollectingHarvester = false;

    #region References
    private CharacterStats CharacterStats => GetComponent<CharacterStats>();
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private Animator CharacterAnimator => GetComponent<CharacterController>().CharacterAnimator;
    #endregion

    public Transform Target { get => target; set => target = value; }
    public Transform KnownTarget { get => knownTarget; set => knownTarget = value; }
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }
    public bool IsCollecting { get => isCollectingHarvester; set => isCollectingHarvester = value; }


    public CombatType CombatAttackType { get; set; }

    protected virtual void Update()
    {
        if (CharacterStats.IsDead)
        {
            Target = null;
            return;
        }

        SetTargetOnMouseClick();

        MoveTowardsAnExistingTarget();
    }

    #region Set player target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.RightClickIsPressed())
        {
            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<EntityDetection>() != null)
                {
                    Target = hit.collider.transform;
                }
                else
                {
                    Target = null;
                    CharacterController.Agent.isStopped = false;
                    CharacterController.Agent.stoppingDistance = 0.2f;
                }
            }
        }
    }
    #endregion

    #region Moving to a target
    void MoveTowardsAnExistingTarget()
    {
        if (Target != null)
        {
            CharacterController.HandleCharacterRotation(transform, Target.position, CharacterController.RotateVelocity, rotateSpeedBeforeAttacking);

            float distance = Vector3.Distance(transform.position, Target.position);

            if (distance > CharacterStats.AttackRange)
            {
                Debug.Log("Far from target");
                CharacterController.Agent.isStopped = false;
                CharacterController.Agent.SetDestination(Target.position);
            }
            else if (distance <= CharacterStats.AttackRange)
            {
                Debug.Log("Close enough to target");
                CharacterController.Agent.isStopped = true;
                CharacterController.Agent.stoppingDistance = CharacterStats.AttackRange;

                TryPerformAnAttack();
                TryInteract();
            }
        }
    }
    #endregion

    #region Interactions

    private void TryInteract()
    {
        if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Stele && 
            Target.GetComponent<Harvester>().harvestState != HarvestState.APlayerIsCollectingHarvestedRessources)
        {
            Debug.Log("Interaction with stele !");

            Target.GetComponent<Harvester>().harvestState = HarvestState.APlayerIsCollectingHarvestedRessources;

            IsCollecting = true;
            CharacterAnimator.SetBool("IsCollecting", true);
            Target.GetComponent<Harvester>().PlayerFound = transform;
        }
    }

    private void TryPerformAnAttack()
    {
        if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy && CanPerformAttack)
        {
            IsCollecting = false;

            if (CombatAttackType == CombatType.MeleeCombat)
            {
                Debug.Log("Melee Attack performed !");
                StartCoroutine(AttackInterval(CombatAttackType));
            }
            else if (CombatAttackType == CombatType.RangedCombat)
            {
                Debug.Log("Ranged Attack performed !");
                StartCoroutine(AttackInterval(CombatAttackType));
            }
        }
    }

    IEnumerator AttackInterval(CombatType attackType)
    {
        Debug.Log("Attack Interval");

        if (attackType == CombatType.MeleeCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("Attack", true);
            //MeleeAttack(); //Debug without animation
            CanPerformAttack = false;
            Debug.Log("Attack Interval");
        }
        else if (attackType == CombatType.RangedCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("Attack", true);
            //RangedAttack(); //Debug without animation
            CanPerformAttack = false;
        }

        yield return new WaitForSeconds(1 / CharacterStats.CurrentAttackSpeed);

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
    #endregion

    #region Behaviours of every type of attack - Melee / Ranged
    public void MeleeAttack()
    {
        if (Target != null)
        {
            if (Target.GetComponent<CharacterStats>() != null)
            {
                Target.GetComponent<CharacterStats>().TakeDamage(transform, CharacterStats.CurrentAttackDamage, CharacterStats.CurrentMagicDamage, CharacterStats.CurrentCriticalStrikeChance, CharacterStats.CurrentCriticalStrikeMultiplier, CharacterStats.CurrentArmorPenetration, CharacterStats.CurrentMagicResistancePenetration);
            }
        }

        CanPerformAttack = true;
    }

    public void RangedAttack()
    {
        Debug.Log("Auto Attack Projectile Instantiated");

        GameObject autoAttackProjectile = Instantiate(rangedAttackProjectile, attackEmiterPosition.position, rangedAttackProjectile.transform.rotation);

        ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

        attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;
        attackProjectile.ProjectileSender = transform;
        attackProjectile.Target = Target;

        CanPerformAttack = true;
    }
    #endregion
}