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
    private EntityStats Stats => GetComponent<EntityStats>();
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
            EntityStats knownTargetStats;

            if (cursorHit.collider.GetComponent<EntityDetection>() != null && cursorHit.collider.GetComponent<EntityDetection>().enabled)
            {
                knownTargetDetected = cursorHit.collider.GetComponent<EntityDetection>();
                knownTargetStats = cursorHit.collider.GetComponent<EntityStats>();

                //Self
                if (knownTargetDetected.gameObject == transform.gameObject)
                {
                    SetCursorToNormalAppearance();
                    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.grey);
                    AssignKnownTarget(knownTargetDetected.transform);
                    return;
                }

                //Needs to be modified to only include Player - Interactive building - Monster - Minion
                switch (knownTargetDetected.TypeOfEntity)
                {
                    //case TypeOfEntity.Self:
                    //    SetCursorToNormalAppearance();
                    //    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.grey);
                    //    AssignKnownTarget(knownTargetDetected.transform);
                    //    break;
                    #region Enemies
                    case TypeOfEntity.EnemyPlayer: //Imagine its Only Player here
                        if (Stats.EntityTeam == knownTargetStats.EntityTeam)
                        {
                            SetCursorToNormalAppearance();
                            ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.blue);
                            AssignKnownTarget(knownTargetDetected.transform);
                        }
                        else if (Stats.EntityTeam != knownTargetStats.EntityTeam)
                        {
                            SetCursorToAttackAppearance();
                            ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.red);
                            AssignKnownTarget(knownTargetDetected.transform);
                        }
                        break;
                    case TypeOfEntity.Monster:
                        if (Stats.EntityTeam == knownTargetStats.EntityTeam)
                        {
                            SetCursorToNormalAppearance();
                            ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.blue);
                            AssignKnownTarget(knownTargetDetected.transform);
                        }
                        else if (Stats.EntityTeam != knownTargetStats.EntityTeam)
                        {
                            SetCursorToAttackAppearance();
                            ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.red);
                            AssignKnownTarget(knownTargetDetected.transform);
                        }
                        break;
                    //case TypeOfEntity.EnemyMinion:
                    //    SetCursorToAttackAppearance();
                    //    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                    //    AssignKnownTarget(knownTargetDetected.transform);
                    //    break;
                    //case TypeOfEntity.EnemyStele:
                    //    SetCursorToAttackAppearance();
                    //    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                    //    AssignKnownTarget(knownTargetDetected.transform);
                    //    break;
                    #endregion
                    #region Allies
                    //case TypeOfEntity.AllyPlayer:
                    //    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                    //    AssignKnownTarget(knownTargetDetected.transform);
                    //    break;
                    //case TypeOfEntity.AllyMinion:
                    //    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                    //    AssignKnownTarget(knownTargetDetected.transform);
                    //    break;
                    //case TypeOfEntity.AllyStele:
                    //    ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>());
                    //    AssignKnownTarget(knownTargetDetected.transform);
                    //    break;
                    #endregion
                    #region Interactive Buildings
                    case TypeOfEntity.Stele:
                        ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.yellow);
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                    case TypeOfEntity.Harvester:
                        ActivateTargetOutlineOnHover(knownTargetDetected.GetComponent<Outline>(), Color.yellow);
                        AssignKnownTarget(knownTargetDetected.transform);
                        break;
                        #endregion
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

    private void ActivateTargetOutlineOnHover(Outline targetOutlineFound, Color outlineColor)
    {
        targetOutlineFound.enabled = true;
        targetOutlineFound.OutlineColor = outlineColor;
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