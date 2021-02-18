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

    void Start()
    {
        SetCursorToNormalAppearance();
    }

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

    public void ActivateTargetOutlineOnHover(Outline targetOutlineFound, Color newOutlineColor)
    {
        targetOutlineFound.enabled = true;
        targetOutlineFound.OutlineColor = newOutlineColor;
    }

    public void DeactivateTargetOutlineOnHover(Outline targetOutlineFound)
    {
        targetOutlineFound.enabled = false;
    }
}
