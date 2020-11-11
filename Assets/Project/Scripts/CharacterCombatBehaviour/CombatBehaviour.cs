using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

enum CombatAttackType { Melee, Ranged }

public class CombatBehaviour : MonoBehaviour
{
    [Header("COMBAT PARAMETERS")]
    [SerializeField] private Transform targetedEnemy;
    [SerializeField] private float rotateSpeedForAttack;
    [SerializeField] private CombatAttackType combatAttackType;
    private bool performAttack = false;

    
    [Header("CURSOR ICONS")]
    [SerializeField] private Texture2D normalCursorIcon;
    [SerializeField] private Texture2D attackCursorIcon;

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
        if (CharacterStats.IsDead)
        {
            if (TargetedEnemy != null)
            {
                TargetedEnemy = null;
            }
            return;
        }

        SetDynamicCursorAppearance();
        SetTargetOnMouseClickButton(1);

        MoveTowardsExistingTargetOrPerformAttack();
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
                    Debug.Log("Melee Attack performed !");
                    //Start Coroutine
                }
            }
        }
    }

    #region Cursor Appearance 
    void SetDynamicCursorAppearance()
    {
        if (Physics.Raycast(RayFromCameraToMousePosition, out RaycastHit hit, Mathf.Infinity) && hit.collider.CompareTag("Enemy"))
        {
            SetCursorToAttackAppearance();
            Debug.Log(hit.transform.name);
        }
        else
        {
            SetCursorToNormalAppearance();
        }
    }

    void SetCursorToNormalAppearance()
    {
        //Debug.Log("Normal Cursor");
        //Cursor.SetCursor(normalCursorIcon, Input.mousePosition, CursorMode.ForceSoftware);
    }

    void SetCursorToAttackAppearance()
    {
        //Debug.Log("Attack Cursor");
        //Cursor.SetCursor(attackCursorIcon, Input.mousePosition, CursorMode.ForceSoftware);
    }
    #endregion
}
