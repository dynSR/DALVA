using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CameraLockState { Locked, Unlocked }

public class CameraController : MonoBehaviour
{
    [Header("LOCK STATE & INPUTS")]
    [SerializeField] private KeyCode changeCameraLockStateKey;
    [SerializeField] private KeyCode cameraFocusOnTargetKey;
    [SerializeField] private CameraLockState cameraLockState;

    [Header("CAMERA LOCKED PARAMETERS")]
    [SerializeField] private float screenEdgesThreshold = 35f;
    [SerializeField] private float cameraMovementSpeed = 15f;
    private Vector3 cameraPosition;

    [Header("CAMERA LOCKED PARAMETERS")]
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float cameraFollowingSpeed;
    [SerializeField] private Vector3 cameraOffset;

    public bool CameraIsLocked => CameraLockState == CameraLockState.Locked;
    public bool CameraIsUnlocked => CameraLockState == CameraLockState.Unlocked;

    public Transform TargetToFollow { get => targetToFollow; set => targetToFollow = value; }
    public CameraLockState CameraLockState { get => cameraLockState; set => cameraLockState = value; }

    void Update()
    {
        if(UtilityClass.IsKeyPressed(changeCameraLockStateKey))
        {
            switch (CameraLockState)
            {
                case CameraLockState.Locked:
                    CameraLockState = CameraLockState.Unlocked;
                    break;
                case CameraLockState.Unlocked:
                    CameraLockState = CameraLockState.Locked;
                    break;
                default:
                    break;
            }
        }
    }

    private void LateUpdate()
    {
        if (CameraIsLocked)
        {
            //Debug.Log("Camera is locked and it's following a target");
            FollowATarget(TargetToFollow);
        }
        else if (CameraIsUnlocked)
        {
            Debug.Log("Camera is unlocked and it's scrolling with edges or directionnal arrows");
            MoveCameraWithDirectionnalArrows();
            MoveCameraOnHittingScreenEdges();

            if (UtilityClass.IsKeyMaintained(cameraFocusOnTargetKey))
            {
                FollowATarget(TargetToFollow);
            }
        }
    }

    #region Camera Behaviours
    void FollowATarget(Transform targetToFollow)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetToFollow.position.x + cameraOffset.x, targetToFollow.position.y + cameraOffset.y, targetToFollow.position.z + cameraOffset.z), cameraFollowingSpeed);
    }
    #endregion

    #region Camera movements with directionnal arrows and mouse cursor (on hitting screen edges)
    void MoveCameraOnHittingScreenEdges()
    {
        cameraPosition = transform.position;

        // move on +X axis
        if (Input.mousePosition.x >= Screen.width - screenEdgesThreshold)
        {
            cameraPosition.x += cameraMovementSpeed * Time.deltaTime; 
        }
        // move on -X axis
        if (Input.mousePosition.x <= screenEdgesThreshold)
        {
            cameraPosition.x -= cameraMovementSpeed * Time.deltaTime;
        }
        // move on +Z axis
        if (Input.mousePosition.y >= Screen.height - screenEdgesThreshold)
        {
            cameraPosition.z += cameraMovementSpeed * Time.deltaTime;
        }
        // move on -Z axis
        if (Input.mousePosition.y <= screenEdgesThreshold)
        {
            cameraPosition.z -= cameraMovementSpeed * Time.deltaTime;
        }

        transform.position = cameraPosition;
        cameraPosition.Normalize();
    }

    void MoveCameraWithDirectionnalArrows()
    {
        cameraPosition = transform.position;

        // move on +X axis
        if (UtilityClass.IsKeyMaintained(KeyCode.RightArrow))
        {
            cameraPosition.x += cameraMovementSpeed * Time.deltaTime;
        }
        // move on -X axis
        if (UtilityClass.IsKeyMaintained(KeyCode.LeftArrow))
        {
            cameraPosition.x -= cameraMovementSpeed * Time.deltaTime;
        }
        // move on +Z axis
        if (UtilityClass.IsKeyMaintained(KeyCode.UpArrow))
        {
            cameraPosition.z += cameraMovementSpeed * Time.deltaTime;
        }
        // move on -Z axis
        if (UtilityClass.IsKeyMaintained(KeyCode.DownArrow))
        {
            cameraPosition.z -= cameraMovementSpeed * Time.deltaTime;
        }

        transform.position = cameraPosition;
        cameraPosition.Normalize();
    }
    #endregion

    #region Minimap Camera movements
    public void MoveCameraToASpecificMiniMapPosition(Vector3 pos)
    {
       transform.position = new Vector3(pos.x + cameraOffset.x, transform.position.y, pos.z + cameraOffset.z);
    }
    #endregion
}