﻿using DarkTonic.MasterAudio;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UtilityClass
{
    #region Left and right click pressure check
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

    #region Left and right key hold check
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

    #region Left and right click on UI elements pressure check
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

    #region Inputs pressure and hold check
    public static bool IsKeyPressed(KeyCode key)
    {
        if (Input.GetKeyDown(key)) return true;
        else return false;
    }

    public static bool IsKeyUnpressed(KeyCode key)
    {
        if (Input.GetKeyUp(key)) return true;
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

    public static bool ContainsParam(this Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }

    public static IEnumerator PlaySoundGroupWithDelay(string sound, Transform originCenter, float delay)
    {
        yield return new WaitForSeconds(delay);

        MasterAudio.PlaySound3DAtTransform(sound, originCenter);

        //Debug.Log("Firing Sound Event !");
    }

    public static void PlaySoundGroupImmediatly(string sound, Transform originCenter)
    {
        MasterAudio.PlaySound3DAtTransform(sound, originCenter);
    }
}