using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    [Header("CURSOR ICONS")]
    [SerializeField] private Texture2D normalCursorIcon;
    [SerializeField] private Texture2D attackCursorIcon;

    public Texture2D NormalCursorIcon { get => normalCursorIcon; }
    public Texture2D AttackCursorIcon { get => attackCursorIcon; }
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private CharacterInteractionsHandler CharacterCombatBehaviour => GetComponent<CharacterInteractionsHandler>();

    RaycastHit cursorHit;

    private bool NoneTypeOfEntityDetected => cursorHit.collider.GetComponent<EntityDetection>() == null ||
        cursorHit.collider.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.None;

    void Start()
    {
        SetCursorToNormalAppearance();
    }

    private void Update()
    {
        SetCursorAppearance();
    }

    #region Set cursor appearance when its hovering an entity
    void SetCursorAppearance()
    {
        if (CharacterController.CursorIsHoveringMiniMap) return;

        if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out cursorHit, Mathf.Infinity))
        {
            EntityDetection knownTargetDetected;

            if (cursorHit.collider.GetComponent<EntityDetection>() != null)
            {
                knownTargetDetected = cursorHit.collider.GetComponent<EntityDetection>();

                switch (knownTargetDetected.TypeOfEntity)
                {
                    case TypeOfEntity.None:
                        break;
                    case TypeOfEntity.Self:
                        ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                    case TypeOfEntity.Ennemy:
                        SetCursorToAttackAppearance();
                        ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                    case TypeOfEntity.Ally:
                        ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                    case TypeOfEntity.Stele:
                        ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                SetCursorToNormalAppearance();

                if (CharacterCombatBehaviour.KnownTarget != null)
                {
                    DeactivateTargetOutlineOnHover(CharacterCombatBehaviour.KnownTarget.GetComponent<Outline>());
                    CharacterCombatBehaviour.KnownTarget = null;
                }
            }
        }
    }
    #endregion

    #region Cursor behaviours
    public void SetCursorToNormalAppearance()
    {
        //Debug.Log("Normal Cursor");
        Cursor.SetCursor(NormalCursorIcon, Vector2.zero, CursorMode.Auto);
    }

    public void SetCursorToAttackAppearance()
    {
        //Debug.Log("Attack Cursor");
        Cursor.SetCursor(AttackCursorIcon, Vector2.zero, CursorMode.Auto);
    }

    private void ActivateTargetOutlineOnHover(Outline targetOutlineFound)
    {
        targetOutlineFound.enabled = true;
    }

    public void DeactivateTargetOutlineOnHover(Outline targetOutlineFound)
    {
        targetOutlineFound.enabled = false;
    }

    private void AssignKnownTarget(Transform targetFound)
    {
        if (targetFound.GetComponent<EntityDetection>() != null)
            CharacterCombatBehaviour.KnownTarget = targetFound.transform;
    }
    #endregion
}