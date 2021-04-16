using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLogic : MonoBehaviour
{
    [Header("CURSOR ICONS")]
    [SerializeField] private Texture2D normalCursorIcon;
    [SerializeField] private Texture2D attackCursorIcon;
    [SerializeField] private Texture2D interactionCursorIcon;

    public Texture2D NormalCursorIcon { get => normalCursorIcon; }
    public Texture2D AttackCursorIcon { get => attackCursorIcon; }
    public Texture2D InteractionCursorIcon { get => interactionCursorIcon; }

    private PlayerController Controller => GetComponent<PlayerController>();
    private EntityStats Stats => GetComponent<EntityStats>();
    private InteractionSystem TargetHandler => GetComponent<InteractionSystem>();

    RaycastHit cursorHit;

    void Start()
    {
        SetCursorToNormalAppearance();
    }

    private void LateUpdate()
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
            EntityStats knownTargetStats;

            if (cursorHit.collider.GetComponent<EntityDetection>() != null && cursorHit.collider.GetComponent<EntityDetection>().enabled)
            {
                knownTargetDetected = cursorHit.collider.GetComponent<EntityDetection>();
                knownTargetStats = cursorHit.collider.GetComponent<EntityStats>();

                AssignKnownTarget(knownTargetDetected.transform);

                //Self
                if (knownTargetDetected.gameObject == transform.gameObject)
                {
                    SetCursorToNormalAppearance();
                    knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.grey);
                    return;
                }

                //Needs to be modified to only include Player - Interactive building - Monster - Minion
                switch (knownTargetDetected.TypeOfEntity)
                {
                    #region Entities
                    case TypeOfEntity.Player: //Imagine its Only Player here
                        if (Stats.EntityTeam == knownTargetStats.EntityTeam)
                        {
                            SetCursorToNormalAppearance();
                            knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.blue);
                        }
                        else if (Stats.EntityTeam != knownTargetStats.EntityTeam)
                        {
                            SetCursorToAttackAppearance();
                            knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.red);
                        }
                        break;
                    case TypeOfEntity.Monster:
                        SetCursorToAttackAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.red);
                        break;
                    case TypeOfEntity.Minion:
                        SetCursorToAttackAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.red);
                        break;
                    #endregion
                    #region Interactive Buildings
                    case TypeOfEntity.Stele:
                        SetCursorToInteractionAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.yellow);
                                                break;
                    case TypeOfEntity.Harvester:
                        SetCursorToInteractionAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.yellow);
                        break;
                        #endregion
                }
            }
            else
            {
                SetCursorToNormalAppearance();

                if (TargetHandler.KnownTarget != null)
                {
                    if (TargetHandler.LastKnownTarget != null)
                    {
                        TargetHandler.LastKnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(TargetHandler.LastKnownTarget.GetComponent<Outline>());
                        TargetHandler.LastKnownTarget = null;
                    }

                    TargetHandler.KnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(TargetHandler.KnownTarget.GetComponent<Outline>());
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

    private void SetCursorToAttackAppearance()
    {
        //Debug.Log("Attack Cursor");
        Cursor.SetCursor(AttackCursorIcon, Vector2.zero, CursorMode.Auto);
    }

    private void SetCursorToInteractionAppearance()
    {
        Cursor.SetCursor(InteractionCursorIcon, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    private void AssignKnownTarget(Transform targetFound)
    {
        if (targetFound.GetComponent<EntityDetection>() != null)
        {
            if (TargetHandler.KnownTarget != null)
            {
                TargetHandler.LastKnownTarget = TargetHandler.KnownTarget;
                TargetHandler.KnownTarget = targetFound.transform;

                if (TargetHandler.LastKnownTarget != TargetHandler.KnownTarget && TargetHandler.LastKnownTarget.GetComponent<Outline>().enabled)
                {
                    TargetHandler.LastKnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(TargetHandler.LastKnownTarget.GetComponent<Outline>());
                }
            }
            else
            {
                TargetHandler.LastKnownTarget = targetFound.transform;
                TargetHandler.KnownTarget = targetFound.transform;
            }
        }
    }
}