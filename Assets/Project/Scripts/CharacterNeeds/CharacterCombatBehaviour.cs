using System.Collections;
using UnityEngine;

public enum CombatAttackType { MeleeCombat, RangedCombat }

public class CharacterCombatBehaviour : MonoBehaviour
{
    [Header("TARGETS INFORMATIONS")]
    [SerializeField] private Transform targetedEnemy; //Est en publique pour debug
    [SerializeField] private Transform knownTarget;

    [Header("BASIC ATTACK")]
    [SerializeField] private Transform basicRangedAttackEmiterPos;
    [SerializeField] private GameObject basicRangedAttackProjectile;

    [Header("COMBAT PARAMETERS")]
    [SerializeField] private float rotateSpeedBeforeAttacking = 0.075f;
    [SerializeField] private bool canPerformAttack = true;

    RaycastHit cursorHit;
    Ray RayFromMainCameraToMouseScreenPosition => Camera.main.ScreenPointToRay(Input.mousePosition);

    private CursorHandler CursorHandler => GetComponent<CursorHandler>();
    public Outline CharacterOutline { get => GetComponent<Outline>(); }

    private CharacterStats CharacterStats => GetComponent<CharacterStats>();
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private Animator CharacterAnimator => GetComponent<CharacterController>().CharacterAnimator;

    public Transform TargetedEnemy { get => targetedEnemy; set => targetedEnemy = value; }
    public Transform KnownTarget { get => knownTarget; set => knownTarget = value; }
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }

    private bool TargetIsNeitherAnEnnemyNorAnAlly => cursorHit.collider.GetComponent<CharacterStats>().TypeOfUnit != TypeOfUnit.Ennemy || cursorHit.collider.GetComponent<CharacterStats>().TypeOfUnit != TypeOfUnit.Ally;

    public CombatAttackType CombatAttackType { get; set; }

    private void Start()
    {
        switch (CharacterStats.TypeOfUnit)
        {
            case TypeOfUnit.Self:
                CharacterOutline.OutlineColor = Color.white;
                break;
            case TypeOfUnit.Ennemy:
                CharacterOutline.OutlineColor = Color.red;
                break;
            case TypeOfUnit.Ally:
                CharacterOutline.OutlineColor = Color.blue;
                break;
            default:
                break;
        }
    }

    protected virtual void Update()
    {
        SetCursorAppearance();

        if (CharacterStats.IsDead)
        {
            TargetedEnemy = null;
            return;
        }

        SetTargetOnMouseClick();

        MoveTowardsAnExistingTarget();
    }

    #region Set cursor appearance when its hovering an entity
    void SetCursorAppearance()
    {
        if (CharacterController.CursorIsHoveringMiniMap) return;

        if (Physics.Raycast(RayFromMainCameraToMouseScreenPosition, out cursorHit, Mathf.Infinity))
        {
            if (cursorHit.collider != null)
            {
                if (cursorHit.collider.GetComponent<CharacterStats>() != null)
                {
                    KnownTarget = cursorHit.collider.transform;
                    CharacterStats knownTargetStats = KnownTarget.GetComponent<CharacterStats>();

                    if (knownTargetStats.TypeOfUnit == TypeOfUnit.Ennemy)
                    {
                        CursorHandler.SetCursorToAttackAppearance();
                        CursorHandler.ActivateTargetOutlineOnHover(KnownTarget.GetComponent<Outline>(), Color.red);
                    }
                    else if (knownTargetStats.TypeOfUnit == TypeOfUnit.Ally)
                    {
                        CursorHandler.ActivateTargetOutlineOnHover(KnownTarget.GetComponent<Outline>(), Color.blue);
                    }
                }
                else if (cursorHit.collider.GetComponent<CharacterStats>() == null || TargetIsNeitherAnEnnemyNorAnAlly)
                {
                    CursorHandler.SetCursorToNormalAppearance();

                    if (KnownTarget != null)
                    {
                        CursorHandler.DeactivateTargetOutlineOnHover(KnownTarget.GetComponent<Outline>());
                        KnownTarget = null;
                    }
                }
            }
        }
    }
    #endregion

    #region Set player target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.RightClickIsPressed())
        {
            if (Physics.Raycast(RayFromMainCameraToMouseScreenPosition, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<CharacterStats>() != null && hit.collider.GetComponent<CharacterStats>().TypeOfUnit == TypeOfUnit.Ennemy)
                {
                    TargetedEnemy = hit.collider.transform;
                }
                else
                {
                    TargetedEnemy = null;
                    CharacterController.Agent.isStopped = false;
                    CharacterController.Agent.stoppingDistance = 0.2f;
                }
            }
        }
    }
    #endregion

    #region Attacking behaviour from moving to a target to performing an attack
    void MoveTowardsAnExistingTarget()
    {
        if (TargetedEnemy != null)
        {
            CharacterController.HandleCharacterRotation(transform, TargetedEnemy.position, CharacterController.RotateVelocity, rotateSpeedBeforeAttacking);

            if (Vector3.Distance(transform.position, TargetedEnemy.position) > CharacterStats.AttackRange)
            {
                Debug.Log("Far from target");
                CharacterController.Agent.isStopped = false;
                CharacterController.Agent.SetDestination(TargetedEnemy.position);
                CharacterController.Agent.stoppingDistance = CharacterStats.AttackRange;
            }
            else if (Vector3.Distance(transform.position, TargetedEnemy.position) <= CharacterStats.AttackRange)
            {
                Debug.Log("Close enough to target");
                CharacterController.Agent.isStopped = true;
                CharacterController.Agent.stoppingDistance = CharacterStats.AttackRange;
            }

            PerformAnAttack();
        }
    }

    private void PerformAnAttack()
    {
        if (Vector3.Distance(transform.position, TargetedEnemy.position) <= CharacterStats.AttackRange && CanPerformAttack)
        {
            CharacterController.Agent.isStopped = true;

            if (CombatAttackType == CombatAttackType.MeleeCombat)
            {
                Debug.Log("Melee Attack performed !");
                StartCoroutine(AttackInterval(CombatAttackType));
            }
            else if (CombatAttackType == CombatAttackType.RangedCombat)
            {
                Debug.Log("Ranged Attack performed !");
                StartCoroutine(AttackInterval(CombatAttackType));
            }
        }
    }

    IEnumerator AttackInterval(CombatAttackType attackType)
    {
        Debug.Log("Attack Interval");

        if (attackType == CombatAttackType.MeleeCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("MeleeBasicAttack", true);
            //MeleeAttack(); //Debug without animation
            CanPerformAttack = false;
            Debug.Log("Attack Interval");
        }
        else if (attackType == CombatAttackType.RangedCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("RangedBasicAttack", true);
            //RangedAttack(); //Debug without animation
            CanPerformAttack = false;
        }

        yield return new WaitForSeconds(1 / CharacterStats.CurrentAttackSpeed);

        if (attackType == CombatAttackType.MeleeCombat)
        {
            CharacterAnimator.SetBool("MeleeBasicAttack", false);
            CanPerformAttack = true;
        }
        else if (attackType == CombatAttackType.RangedCombat)
        {
            CharacterAnimator.SetBool("RangedBasicAttack", false);
            CanPerformAttack = true;
        }
    }
    #endregion

    #region Behaviours of every type of attack - Melee / Ranged
    public void MeleeAttack()
    {
        if (TargetedEnemy != null)
        {
            if (TargetedEnemy.GetComponent<CharacterStats>() != null)
            {
                TargetedEnemy.GetComponent<CharacterStats>().TakeDamage(transform, CharacterStats.CurrentAttackDamage, CharacterStats.CurrentMagicDamage, CharacterStats.CurrentCriticalStrikeChance, CharacterStats.CurrentCriticalStrikeMultiplier, CharacterStats.CurrentArmorPenetration, CharacterStats.CurrentMagicResistancePenetration);
            }
        }

        CanPerformAttack = true;
    }

    public void RangedAttack()
    {
        GameObject autoAttackProjectile = Instantiate(basicRangedAttackProjectile, basicRangedAttackEmiterPos.position, basicRangedAttackProjectile.transform.rotation);

        Projectile attackProjectile = autoAttackProjectile.GetComponent<Projectile>();

        attackProjectile.ProjectileSender = transform;
        attackProjectile.Target = TargetedEnemy;

        CanPerformAttack = true;
    }
    #endregion
}