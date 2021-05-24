using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapInteractions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Transform Player => GetComponentInParent<PlayerHUDManager>().Player;
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private GameObject movementFeedbackToInstantiate;
    [SerializeField] private RawImage miniMapRenderingImage;

    private Vector2 localCursor;
    private RaycastHit miniMapHit;

    private PlayerController PlayerController => Player.GetComponent<PlayerController>();
    private CameraController PlayerCameraController => Player.GetComponent<PlayerController>().CharacterCamera.GetComponent<CameraController>();

    private bool cameraWasLocked;

    void Update()
    {
        //if (!PlayerController.Agent.hasPath) return;

        PlayerController.HandleMotionAnimation(PlayerController.Agent, PlayerController.CharacterAnimator, "MoveSpeed", PlayerController.MotionSmoothTime);
        PlayerController.DebugPathing(Player.GetComponent<PlayerController>().MyLineRenderer);
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
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRenderingImage.rectTransform, requiredEventData.position, requiredEventData.pressEventCamera, out localCursor))
        {
            Texture tex = miniMapRenderingImage.texture;
            Rect r = miniMapRenderingImage.rectTransform.rect;

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
        PlayerController.Agent.isStopped = false;

        if (Physics.Raycast(miniMapRay, out miniMapHit, Mathf.Infinity))
        {
            //Debug.Log("Object touched by the character controller raycast " + miniMapHit.collider.gameObject);

            if (UtilityClass.LeftClickIsPressedOnUIElement(requiredEventData) && GameManager.Instance.GameIsInPlayMod())
            {
                Debug.Log("LETF CLICK ON MAP");

                PlayerCameraController.MoveCameraToASpecificMiniMapPosition(miniMapHit.point);
            }

            if (UtilityClass.RightClickIsPressedOnUIElement(requiredEventData) && GameManager.Instance.GameIsInPlayMod() && !PlayerController.IsStunned && !PlayerController.IsRooted && !PlayerController.IsCasting)
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
