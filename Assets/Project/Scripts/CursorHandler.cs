﻿using System.Collections;
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
        //cursorSpriteRenderer.sprite = normalCursorSprite;
    }

    public void SetCursorToAttackAppearance()
    {
        //Debug.Log("Attack Cursor");
        Cursor.SetCursor(AttackCursorIcon, Vector2.zero, CursorMode.Auto);
        //cursorSpriteRenderer.sprite = attackCursorSprite;
    }
}