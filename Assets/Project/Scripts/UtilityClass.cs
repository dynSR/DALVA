using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UtilityClass
{
    #region Left and right click pressure checker
    public static bool LeftClickIsPressed()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        else return false;
    }

    public static bool RightClickIsPressed()
    {
        if (Input.GetMouseButtonDown(1))
        {
            return true;
        }
        else return false;
    }
    #endregion

    #region Left and right key hold checker
    public static bool LeftClickIsHeld()
    {
        if (Input.GetMouseButton(0))
        {
            return true;
        }
        else return false;
    }

    public static bool RightClickIsHeld()
    {
        if (Input.GetMouseButton(1))
        {
            return true;
        }
        else return false;
    }
    #endregion

    #region Left and right click on UI elements pressure checker
    public static bool LeftClickIsPressedOnUIElement(PointerEventData requiredEventData)
    {
        if (requiredEventData.button == PointerEventData.InputButton.Left)
        {
            return true;
        }
        else return false;
    }

    public static bool RightClickIsPressedOnUIElement(PointerEventData requiredEventData)
    {
        if (requiredEventData.button == PointerEventData.InputButton.Right)
        {
            return true;
        }
        else return false;
    }
    #endregion

    #region Inputs pressure and hold checker
    public static bool IsKeyPressed(KeyCode key)
    {
        if (Input.GetKeyDown(key)) return true;
        else return false;
    }

    public static bool IsKeyMaintained(KeyCode key)
    {
        if (Input.GetKey(key)) return true;
        else return false;
    }
    #endregion

    #region Get main camera informations
    public static Camera GetMainCamera()
    {
        return Camera.main;
    }
    public static Vector3 GetMainCameraPosition()
    {
        return Camera.main.transform.position;
    }

    public static Ray RayFromMainCameraToMousePosition()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
    #endregion
}