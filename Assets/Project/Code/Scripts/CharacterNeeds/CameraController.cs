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

    [Header("CAMERA BOUNDARIES PARAMETERS")]
    [SerializeField] private BoxCollider cameraConfiner;

    public float zoom = 80f;

    public bool CameraIsLocked => CameraLockState == CameraLockState.Locked;
    public bool CameraIsUnlocked => CameraLockState == CameraLockState.Unlocked;

    public Transform TargetToFollow { get => targetToFollow; set => targetToFollow = value; }
    public CameraLockState CameraLockState { get => cameraLockState; set => cameraLockState = value; }

    private void Awake()
    {
        cameraConfiner = GameObject.FindWithTag("CameraConfiner").GetComponent<BoxCollider>();
    }

    private void Start()
    {
        FollowATarget(TargetToFollow);
    }

    void Update()
    {
        if(UtilityClass.IsKeyPressed(changeCameraLockStateKey) && GameManager.Instance.GameIsInPlayMod())
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
        if (!GameManager.Instance.GameIsInPlayMod()) return;

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

        HandlerCameraZoom();
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

        SetCameraPosition(cameraPosition);

        //transform.position = cameraPosition;
        //cameraPosition.Normalize();
        ////BoundsCameraPosition(cameraPosition);
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

        SetCameraPosition(cameraPosition);

        //transform.position = cameraPosition;
        ////transform.position = BoundsCameraPosition(cameraPosition);
        //cameraPosition.Normalize();
    }
    #endregion

    void HandlerCameraZoom()
    {
        float zoomChangeAmount = 80f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomChangeAmount * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomChangeAmount * Time.deltaTime;
        }

        zoom = Mathf.Clamp(zoom, 20f, 60f);
        GetComponent<Camera>().fieldOfView = zoom;
    }

    void SetCameraPosition(Vector3 newPos)
    {
        transform.position = BoundsCameraPosition(newPos);
        newPos.Normalize();
    }

    Vector3 BoundsCameraPosition(Vector3 newPos)
    {
        newPos = new Vector3(
            Mathf.Clamp(newPos.x, cameraConfiner.bounds.min.x + cameraOffset.x, cameraConfiner.bounds.max.x + cameraOffset.x),
            newPos.y,
             Mathf.Clamp(newPos.z, cameraConfiner.bounds.min.z + cameraOffset.z, cameraConfiner.bounds.max.z + cameraOffset.z)
            );

        return newPos;
    }

    #region Minimap Camera movements
    public void MoveCameraToASpecificMiniMapPosition(Vector3 pos)
    {
       transform.position = new Vector3(pos.x + cameraOffset.x, transform.position.y, pos.z + cameraOffset.z);
    }
    #endregion
}