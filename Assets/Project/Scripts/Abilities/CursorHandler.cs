using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    [SerializeField] private Texture2D normalCursorIcon;
    [SerializeField] private Texture2D attackCursorIcon;
    
    Ray Ray => Camera.main.ScreenPointToRay(Input.mousePosition);

    void Start()
    {
        SetCursorToNormalAppearance();
    }

    void Update()
    {
        SetDynamicCursorAppearance();
    }

    #region Cursor Appearance 
    void SetDynamicCursorAppearance()
    {
        if (Physics.Raycast(Ray, out RaycastHit hit) && hit.transform.CompareTag("Enemy"))
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
        Debug.Log("Normal Cursor");
        //Cursor.SetCursor(normalCursorIcon, Input.mousePosition, CursorMode.ForceSoftware);
    }

    void SetCursorToAttackAppearance()
    {
        Debug.Log("Attack Cursor");
        //Cursor.SetCursor(attackCursorIcon, Input.mousePosition, CursorMode.ForceSoftware);
    }
    #endregion
}