using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLogic : MonoBehaviour
{
    [Header("CURSOR ICONS")]
    [SerializeField] private Texture2D normalCursorIcon;
    [SerializeField] private Texture2D attackCursorIcon;

    public Texture2D NormalCursorIcon { get => normalCursorIcon; }
    public Texture2D AttackCursorIcon { get => attackCursorIcon; }

    private PlayerController Controller => GetComponent<PlayerController>();
    private InteractionSystem TargetHandler => GetComponent<InteractionSystem>();

    RaycastHit cursorHit;

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
        if (Controller.IsCursorHoveringUIElement) return;

        if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out cursorHit, Mathf.Infinity))
        {
            EntityDetection knownTargetDetected;

            if (cursorHit.collider.GetComponent<EntityDetection>() != null && cursorHit.collider.GetComponent<EntityDetection>().enabled)
            {
                knownTargetDetected = cursorHit.collider.GetComponent<EntityDetection>();

                switch (knownTargetDetected.TypeOfEntity)
                {
                    case TypeOfEntity.None:
                        break;
                    case TypeOfEntity.Self:
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                    case TypeOfEntity.Enemy:
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
                    case TypeOfEntity.Harvester:
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

                if (TargetHandler.KnownTarget != null)
                {
                    DeactivateTargetOutlineOnHover(TargetHandler.KnownTarget.GetComponent<Outline>());
                    TargetHandler.KnownTarget = null;
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
            TargetHandler.KnownTarget = targetFound.transform;
    }
    #endregion
}