using UnityEngine;
using UnityEngine.AI;
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

    #region Character Destination and motion handling, including rotation
    public static void SetAgentDestination(Vector3 pos, NavMeshAgent agent)
    {
        agent.SetDestination(pos);
    }

    public static void HandleMotionAnimation(NavMeshAgent agent, Animator animator, string animationFloatName, float smoothTime)
    {
        float moveSpeed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat(animationFloatName, moveSpeed, smoothTime, Time.deltaTime);
    }

    public static void HandleCharacterRotation(Transform transform, Vector3 target, float rotateVelocity, float rotationSpeed)
    {
        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            rotationSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }
    #endregion
}