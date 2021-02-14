using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

enum CombatAttackType { Melee, Ranged }

public class CharacterCombatBehaviour : MonoBehaviour
{
    [Header("TARGETS")]
    [SerializeField] private Transform targetedEnemy; //Est en publique pour debug
    [SerializeField] private Transform knownTarget;

    [Header("BASIC ATTACK")]
    [SerializeField] private Transform basicRangedAttackEmiterPos;
    [SerializeField] private GameObject basicRangedAttackProjectile;

    [Header("COMBAT PARAMETERS")]
    [SerializeField] private CombatAttackType combatAttackType;
    [SerializeField] private float rotateSpeedBeforeAttacking = 0.075f;
    [SerializeField] private bool canPerformAttack = true;

    RaycastHit cursorHit;
    Ray RayFromCameraToMousePosition => Camera.main.ScreenPointToRay(Input.mousePosition);
    private CursorHandler CursorHandler => GetComponent<CursorHandler>();
    public Outline CharacterOutline { get => GetComponent<Outline>(); }

    private Stats CharacterStats => GetComponent<Stats>();
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private Animator CharacterAnimator => GetComponent<CharacterController>().CharacterAnimator;

    public Transform TargetedEnemy { get => targetedEnemy; set => targetedEnemy = value; }
    public Transform KnownTarget { get => knownTarget; set => knownTarget = value; }
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }

    private bool TargetIsNeitherAnEnnemyNorAnAlly => cursorHit.collider.GetComponent<Stats>().TypeOfUnit != TypeOfUnit.Ennemy || cursorHit.collider.GetComponent<Stats>().TypeOfUnit != TypeOfUnit.Ally;

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
        SetDynamicCursorAppearance();

        if (CharacterStats.IsDead)
        {
            TargetedEnemy = null;
            return;
        }

        SetTargetOnMouseClickButton();

        MoveTowardsExistingTargetOrPerformAttack();
    }

    void SetDynamicCursorAppearance()
    {
        if (CharacterController.CursorIsHoveringMiniMap) return;

        if (Physics.Raycast(RayFromCameraToMousePosition, out cursorHit, Mathf.Infinity))
        {
            if (cursorHit.collider != null)
            {
                if (cursorHit.collider.GetComponent<Stats>() != null)
                {
                    KnownTarget = cursorHit.collider.transform;
                    Stats knownTargetStats = KnownTarget.GetComponent<Stats>();

                    if (knownTargetStats.TypeOfUnit == TypeOfUnit.Ennemy)
                    {
                        CursorHandler.SetCursorToAttackAppearance();
                        CursorHandler.ActivateOutlineOnOver(KnownTarget.GetComponent<Outline>(), Color.red);
                    }
                    else if (knownTargetStats.TypeOfUnit == TypeOfUnit.Ally)
                    {
                        CursorHandler.ActivateOutlineOnOver(KnownTarget.GetComponent<Outline>(), Color.blue);
                    }
                }
                else if (cursorHit.collider.GetComponent<Stats>() == null || TargetIsNeitherAnEnnemyNorAnAlly)
                {
                    CursorHandler.SetCursorToNormalAppearance();

                    if (KnownTarget != null)
                    {
                        CursorHandler.DeactivateOutlineOnOver(KnownTarget.GetComponent<Outline>());
                        KnownTarget = null;
                    }
                }
            }
        }
    }

    void SetTargetOnMouseClickButton()
    {
        if (UtilityClass.RightClickIsPressed())
        {
            if (Physics.Raycast(RayFromCameraToMousePosition, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    TargetedEnemy = hit.collider.transform;
                }
                else if (!hit.collider.CompareTag("Enemy"))
                {
                    TargetedEnemy = null;
                    CharacterController.Agent.isStopped = false;
                    CharacterController.Agent.stoppingDistance = 0.2f;
                }
            }
        }
    }

    void MoveTowardsExistingTargetOrPerformAttack()
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
            }

            if (Vector3.Distance(transform.position, TargetedEnemy.position) <= CharacterStats.AttackRange && CanPerformAttack)
            {
                CharacterController.Agent.isStopped = true;

                if (combatAttackType == CombatAttackType.Melee)
                {
                    Debug.Log("Melee Attack performed !");
                    StartCoroutine(AttackInterval(combatAttackType));
                }
                else if (combatAttackType == CombatAttackType.Ranged)
                {
                    Debug.Log("Ranged Attack performed !");
                    StartCoroutine(AttackInterval(combatAttackType));
                }
            }
        }
    }

    IEnumerator AttackInterval(CombatAttackType attackType)
    {
        if (attackType == CombatAttackType.Melee)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("MeleeBasicAttack", true);
            //MeleeAttack(); Debug without animation
            CanPerformAttack = false;
        }
        else if (attackType == CombatAttackType.Ranged)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("RangedBasicAttack", true);
            //RangedAttack(); Debug without animation
            CanPerformAttack = false;
        }

        yield return new WaitForSeconds(1 / CharacterStats.CurrentAttackSpeed);

        if (attackType == CombatAttackType.Melee)
        {
            CharacterAnimator.SetBool("MeleeBasicAttack", false);
            CanPerformAttack = true;
        }
        else if (attackType == CombatAttackType.Ranged)
        {
            CharacterAnimator.SetBool("RangedBasicAttack", false);
            CanPerformAttack = true;
        }
    }

    public void MeleeAttack()
    {
        if (TargetedEnemy != null)
        {
            if (TargetedEnemy.GetComponent<Stats>() != null)
            {
                TargetedEnemy.GetComponent<Stats>().TakeDamage(CharacterStats.CurrentAttackDamage, CharacterStats.CurrentMagicDamage, CharacterStats.CurrentCriticalStrikeChance, CharacterStats.CurrentCriticalStrikeMultiplier, CharacterStats.CurrentArmorPenetration, CharacterStats.CurrentMagicResistancePenetration);
            }
        }

        CanPerformAttack = true;
    }

    public void RangedAttack()
    {
        GameObject autoAttackProjectile = Instantiate(basicRangedAttackProjectile, basicRangedAttackEmiterPos.position, basicRangedAttackProjectile.transform.rotation);

        AutoAttackProjectileController rangedAttackProjectile = autoAttackProjectile.GetComponent<AutoAttackProjectileController>();

        rangedAttackProjectile.ProjectileSender = transform;
        rangedAttackProjectile.Target = TargetedEnemy;

        CanPerformAttack = true;
    }
}
