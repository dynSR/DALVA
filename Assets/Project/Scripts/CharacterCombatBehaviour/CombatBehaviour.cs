using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

enum CombatAttackType { Melee, Ranged }

public class CombatBehaviour : MonoBehaviour
{
    [Header("COMBAT PARAMETERS")]
    [SerializeField] private Transform targetedEnemy;
    [SerializeField] private float rotateSpeedForAttack;
    [SerializeField] private CombatAttackType combatAttackType;
    private bool canPerformAttack = false;

    private CursorHandler CursorHandler => GetComponent<CursorHandler>();

    Ray RayFromCameraToMousePosition => Camera.main.ScreenPointToRay(Input.mousePosition);

    
    private Stats CharacterStats => GetComponent<Stats>();
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private Animator CharacterAnimator => GetComponent<CharacterController>().CharacterAnimator;

    public Transform TargetedEnemy { get => targetedEnemy; set => targetedEnemy = value; }

    protected virtual void Start()
    {
        
    }

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
                    CharacterController.Agent.stoppingDistance = 0.2f;
                }
            }
        }
    }

    void MoveTowardsExistingTargetOrPerformAttack()
    {
        if (TargetedEnemy != null)
        {
            if (Vector3.Distance(transform.position, TargetedEnemy.position) > CharacterStats.AttackRange)
            {
                CharacterController.Agent.SetDestination(TargetedEnemy.position);
                CharacterController.Agent.stoppingDistance = CharacterStats.AttackRange;

                CharacterController.HandleCharacterRotation(TargetedEnemy.position, CharacterController.RotateVelocity, rotateSpeedForAttack);
            }
            else
            {
                if (combatAttackType == CombatAttackType.Melee)
                {
                    if (canPerformAttack)
                    {
                        Debug.Log("Melee Attack performed !");
                        //Start Coroutine
                        StartCoroutine(MeleeAttackInterval());
                    }
                }
            }
        }
    }

    IEnumerator MeleeAttackInterval()
    {
        canPerformAttack = false;
        //CharacterAnimator.SetBool("BasicAttack", true);
        yield return new WaitForSeconds(CharacterStats.CurrentAttackSpeed / (100 + CharacterStats.CurrentAttackSpeed) * 0.01f);

        if (TargetedEnemy == null)
        {
            //CharacterAnimator.SetBool("BasicAttack", false);
            canPerformAttack = true;
        }
    }

    public void MeleeAttack()
    {
        if (TargetedEnemy != null)
        {
            if (TargetedEnemy.GetComponent<Stats>() != null)
            {
                TargetedEnemy.GetComponent<Stats>().TakeDamage(CharacterStats.CurrentAttackDamage, 0);
            }
        }

        canPerformAttack = true;
    }

}
