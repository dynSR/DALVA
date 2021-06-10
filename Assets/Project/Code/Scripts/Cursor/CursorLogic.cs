using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLogic : MonoBehaviour
{
    [Header("CURSOR ICONS")]
    [SerializeField] private Texture2D normalCursorIcon;
    [SerializeField] private Texture2D attackCursorIcon;
    [SerializeField] private Texture2D interactionCursorIcon;
    [SerializeField] private Texture2D attackOnMoveCursorIcon;

    public Texture2D NormalCursorIcon { get => normalCursorIcon; }
    public Texture2D AttackCursorIcon { get => attackCursorIcon; }
    public Texture2D InteractionCursorIcon { get => interactionCursorIcon; }
    public Texture2D AttackOnMoveCursorIcon { get => attackOnMoveCursorIcon; }

    private PlayerController Controller => GetComponent<PlayerController>();
    private EntityStats Stats => GetComponent<EntityStats>();
    private PlayerInteractions Interactions => GetComponent<PlayerInteractions>();

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
        if (Controller.IsCursorHoveringUIElement
            || Controller.IsStunned
            || Interactions.PlayerIsTryingToAttack
            || !GameManager.Instance.GameIsInPlayMod())
        {
            SetCursorToNormalAppearance();
            return;
        }

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
                    knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.white);
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
                            knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.blue);
                        }
                        else if (Stats.EntityTeam != knownTargetStats.EntityTeam)
                        {
                            SetCursorToAttackAppearance();
                            knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.red);
                        }
                        break;
                    case TypeOfEntity.Monster:
                        SetCursorToAttackAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.red);
                        break;
                    case TypeOfEntity.Minion:
                        SetCursorToAttackAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.red);
                        break;
                    case TypeOfEntity.SteleEffect:
                        SetCursorToNormalAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.blue);
                        break;
                    #endregion
                    #region Interactive Buildings
                    case TypeOfEntity.Stele:
                        SetCursorToInteractionAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.yellow);

                        SetSentinelOutline(knownTargetDetected.transform, true);

                        break;
                    case TypeOfEntity.Harvester:
                        SetCursorToInteractionAppearance();
                        knownTargetDetected.ActivateTargetOutlineOnHover(knownTargetDetected.Outline, Color.yellow);
                        break;
                        #endregion
                }
            }
            else //Nothing is aimed
            {
                SetCursorToNormalAppearance();

                if (Interactions.KnownTarget != null)
                {
                    if (Interactions.LastKnownTarget != null)
                    {
                        Interactions.KnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(Interactions.KnownTarget.GetComponent<EntityDetection>().Outline);
                        Interactions.LastKnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(Interactions.LastKnownTarget.GetComponent<EntityDetection>().Outline);

                        SetSentinelOutline(Interactions.KnownTarget, false);

                        //Interactions.LastKnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(Interactions.LastKnownTarget.GetComponent<EntityDetection>().Outline);
                        Interactions.LastKnownTarget = null;
                        Interactions.KnownTarget = null;
                    }
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

        if (!PlayerHUDManager.Instance.IsShopWindowOpen)
            Cursor.SetCursor(AttackCursorIcon, Vector2.zero, CursorMode.Auto);
    }

    private void SetCursorToInteractionAppearance()
    {
        if (!PlayerHUDManager.Instance.IsShopWindowOpen)
            Cursor.SetCursor(InteractionCursorIcon, Vector2.zero, CursorMode.Auto);
    }

    public void SetCursorToAttackOnMoveAppearance()
    {
        Cursor.SetCursor(AttackOnMoveCursorIcon, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    private void AssignKnownTarget(Transform targetFound)
    {
        if (targetFound.GetComponent<EntityDetection>() != null)
        {
            if (Interactions.KnownTarget != null)
            {
                Interactions.LastKnownTarget = Interactions.KnownTarget;
                Interactions.KnownTarget = targetFound.transform;

                if (Interactions.LastKnownTarget != Interactions.KnownTarget && Interactions.LastKnownTarget.GetComponent<EntityDetection>().Outline.enabled)
                {
                    Interactions.LastKnownTarget.GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(Interactions.LastKnownTarget.GetComponent<EntityDetection>().Outline);

                    SetSentinelOutline(Interactions.LastKnownTarget, false);
                }
            }
            else
            {
                Interactions.LastKnownTarget = targetFound.transform;
                Interactions.KnownTarget = targetFound.transform;
            }
        }
    }

    private void SetSentinelOutline(Transform target, bool activate)
    {
        SteleLogic steleLogic = target.GetComponent<SteleLogic>();

        if (steleLogic == null) return;

        if (steleLogic.SteleEffect == SteleEffect.Sentinel)
        {
            if (activate)
            {
                SteleOutline steleOutline = steleLogic.SpawnedEffectObject.GetComponent<SteleOutline>();
                steleOutline.SetOutlineInteractionColor();
            }
            else if (!activate)
            {
                SteleOutline steleOutline = steleLogic.SpawnedEffectObject.GetComponent<SteleOutline>();
                steleOutline.SetOutlineDefaultColor();
            }
        } 
    }
}