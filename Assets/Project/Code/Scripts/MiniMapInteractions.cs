using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapInteractions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    //Drag Orthographic top down camera here
    [SerializeField] private Transform player;
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private GameObject movementFeedbackToInstantiate;

    private Vector2 localCursor;
    private RaycastHit miniMapHit;

    private CharacterController PlayerController => player.GetComponent<CharacterController>();
    private CameraController PlayerCameraController => player.GetComponent<CharacterController>().CharacterCamera.GetComponent<CameraController>();

    private bool cameraWasLocked;

    void Update()
    {
        UtilityClass.HandleMotionAnimation(PlayerController.Agent, PlayerController.CharacterAnimator, "MoveSpeed", PlayerController.MotionSmoothTime);
        PlayerController.DebugPathing(player.GetComponent<CharacterController>().MyLineRenderer);
    }

    #region Pointer events
    public void OnPointerDown(PointerEventData eventData)
    {
        if (PlayerCameraController.CameraIsLocked)
        {
            cameraWasLocked = true;
            PlayerCameraController.CameraLockState = CameraLockState.Unlocked;
        }

        RaycastToMiniMap(eventData);
    }

    public void OnPointerUp(PointerEventData requiredEventData)
    {
        if (cameraWasLocked)
        {
            PlayerCameraController.CameraLockState = CameraLockState.Locked;
            cameraWasLocked = false;
        }
    }
    #endregion

    #region Functions helping to get a position from the minimap
    private void RaycastToMiniMap(PointerEventData requiredEventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, requiredEventData.position, requiredEventData.pressEventCamera, out localCursor))
        {
            Texture tex = GetComponent<RawImage>().texture;
            Rect r = GetComponent<RawImage>().rectTransform.rect;

            //Using the size of the texture and the local cursor, clamp the X,Y coords between 0 and width - height of texture
            float coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
            float coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

            //Convert coordX and coordY to % (0.0-1.0) with respect to texture width and height
            float recalcX = coordX / tex.width;
            float recalcY = coordY / tex.height;

            localCursor = new Vector2(recalcX, recalcY);

            ConvertAPointFromMiniMapToWorldSpace(localCursor, requiredEventData);

            Debug.DrawRay(miniMapCamera.transform.position, miniMapHit.point, Color.red, 0.5f);
        }
    }

    private void ConvertAPointFromMiniMapToWorldSpace(Vector2 localCursor, PointerEventData requiredEventData)
    {
        Ray miniMapRay = miniMapCamera.ScreenPointToRay(new Vector2(localCursor.x * miniMapCamera.pixelWidth, localCursor.y * miniMapCamera.pixelHeight));

        if (Physics.Raycast(miniMapRay, out miniMapHit, Mathf.Infinity))
        {
            //Debug.Log("Object touched by the character controller raycast " + miniMapHit.collider.gameObject);

            if (UtilityClass.LeftClickIsPressedOnUIElement(requiredEventData))
            {
                Debug.Log("LETF CLICK ON MAP");

                PlayerCameraController.MoveCameraToASpecificMiniMapPosition(miniMapHit.point);
            }

            if (UtilityClass.RightClickIsPressedOnUIElement(requiredEventData))
            {
                Debug.Log("RIGHT CLICK ON MAP");

                PlayerController.SetNavMeshDestination(miniMapRay);
                GameObject go = Instantiate(movementFeedbackToInstantiate, miniMapHit.point, Quaternion.identity);
            }
        }
    }
    #endregion

    #region OnDrag events
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGGGGGGGGGGGGG");

        if (eventData.IsPointerMoving() && eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.Use();
            RaycastToMiniMap(eventData);
        }
    }
    #endregion
}
