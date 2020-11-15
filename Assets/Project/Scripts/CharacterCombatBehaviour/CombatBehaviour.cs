using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

enum CombatAttackType { Melee, Ranged }

public class CombatBehaviour : MonoBehaviour
{
    [Header("COMBAT PARAMETERS")]
    [SerializeField] private Transform targetedEnemy;
    [SerializeField] private Transform basicRangedAttackEmmiterPos;
    [SerializeField] private GameObject basicRangedAttackProjectile;
    [SerializeField] private float rotateSpeedForAttack;
    [SerializeField] private CombatAttackType combatAttackType;
    [SerializeField] private bool canPerformAttack = true;
   
    private CursorHandler CursorHandler => GetComponent<CursorHandler>();

    Ray RayFromCameraToMousePosition => Camera.main.ScreenPointToRay(Input.mousePosition);

    private Stats CharacterStats => GetComponent<Stats>();
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private LaunchProjectile LaunchProjectile => GetComponent<LaunchProjectile>();
    private Animator CharacterAnimator => GetComponent<CharacterController>().CharacterAnimator;

    public Transform TargetedEnemy { get => targetedEnemy; set => targetedEnemy = value; }
    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }

    protected virtual void Update()
    {
        SetDynamicCursorAppearance();

        if (CharacterStats.IsDead)
        {
            TargetedEnemy = null;
            return;
        }

        SetTargetOnMouseClickButton(1);

        MoveTowardsExistingTargetOrPerformAttack();
    }

    void SetDynamicCursorAppearance()
    {
        if (Physics.Raycast(RayFromCameraToMousePosition, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                CursorHandler.SetCursorToAttackAppearance();
            }
            else if (!hit.collider.CompareTag("Enemy"))
            {
                CursorHandler.SetCursorToNormalAppearance();
            }
        }
    }

    void SetTargetOnMouseClickButton(int mouseClickButton)
    {
        if (Input.GetMouseButtonDown(mouseClickButton))
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
            CharacterController.HandleCharacterRotation(transform, TargetedEnemy.position, CharacterController.RotateVelocity, rotateSpeedForAttack);

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

            if (CanPerformAttack)
            {
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
            //CharacterAnimator.SetBool("MeleeBasicAttack", true);
            MeleeAttack();
            CanPerformAttack = false;
        }
        else if (attackType == CombatAttackType.Ranged)
        {
            //CharacterAnimator.SetBool("RangedBasicAttack", true);
            RangedAttack();
            CanPerformAttack = false;
        }

        yield return new WaitForSeconds(1 / CharacterStats.CurrentAttackSpeed);

        if (attackType == CombatAttackType.Melee)
        {
            //CharacterAnimator.SetBool("MeleeBasicAttack", false);
            CanPerformAttack = true;
        }
        else if (attackType == CombatAttackType.Ranged)
        {
            //CharacterAnimator.SetBool("RangedBasicAttack", false);
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
        ProjectileController rangedAttackProjectile = basicRangedAttackProjectile.GetComponent<ProjectileController>();

        rangedAttackProjectile.ProjectileSender = transform;
        rangedAttackProjectile.Target = TargetedEnemy;
        rangedAttackProjectile.ProjectileSenderCharacterStats = CharacterStats;

        StartCoroutine(LaunchProjectile.LaunchAProjectile(rangedAttackProjectile.gameObject, basicRangedAttackEmmiterPos, ProjectileType.TravelsToATarget));

        CanPerformAttack = true;
    }
}
