using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private enum CameraLockState { Lock, Unlocked }
    [SerializeField] private CameraLockState cameraLockState;
    [SerializeField] private KeyCode changeCameraLockStateKey;
    [SerializeField] private KeyCode cameraFocusOnTargetKey;

    [Header("CAMERA LOCKED PARAMETERS")]
    [SerializeField] private float screenEdgesThreshold = 50;
    [SerializeField] private float cameraMovementSpeed = 15;
    private Vector3 cameraPosition;

    [Header("CAMERA LOCKED PARAMETERS")]
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float cameraFollowingSpeed;
    [Tooltip("Il faut mettre la moitié de la valeur de l'axe Y (en négatif) sur l'axe Z pour avoir le personnage visé par la caméra au centre de l'écran")]
    [SerializeField] private Vector3 cameraOffset;

    public Transform TargetToFollow { get => targetToFollow; set => targetToFollow = value; }

    void Update()
    {
        if(Input.GetKeyDown(changeCameraLockStateKey))
        {
            switch (cameraLockState)
            {
                case CameraLockState.Lock:
                    cameraLockState = CameraLockState.Unlocked;
                    break;
                case CameraLockState.Unlocked:
                    cameraLockState = CameraLockState.Lock;
                    break;
                default:
                    break;
            }
        }

        if (cameraLockState == CameraLockState.Lock)
        {
            Debug.Log("Camera is locked and it's following a target");
            FollowATarget(TargetToFollow);
        }
            
        else if (cameraLockState == CameraLockState.Unlocked)
        {
            Debug.Log("Camera is unlocked and it's scrolling with edges or directionnal arrows");
            MoveCameraWithDirectionnalArrows();
            MoveCameraWithMouse();

            if (Input.GetKey(cameraFocusOnTargetKey))
            {
                FollowATarget(TargetToFollow);
            }
        }
    }

    void FollowATarget(Transform targetToFollow)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetToFollow.position.x, targetToFollow.position.y + cameraOffset.y, targetToFollow.position.z + cameraOffset.z), cameraFollowingSpeed);
    }

    void MoveCameraWithMouse()
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
    }

    void MoveCameraWithDirectionnalArrows()
    {
        cameraPosition = transform.position;

        // move on +X axis
        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraPosition.x += cameraMovementSpeed * Time.deltaTime;
        }
        // move on -X axis
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraPosition.x -= cameraMovementSpeed * Time.deltaTime;
        }
        // move on +Z axis
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraPosition.z += cameraMovementSpeed * Time.deltaTime;
        }
        // move on -Z axis
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraPosition.z -= cameraMovementSpeed * Time.deltaTime;
        }

        transform.position = cameraPosition;
    }
}
