using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapMovementHandler : MonoBehaviour, IPointerClickHandler
{
    //Drag Orthographic top down camera here
    [SerializeField] private Transform player;
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private GameObject movementFeedbackToInstantiate;

    void Update()
    {
        player.GetComponent<CharacterController>().HandleMotionAnimation();
        player.GetComponent<CharacterController>().DebugPathing(player.GetComponent<CharacterController>().MyLineRenderer);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localCursor;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
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

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("LETF CLICK ON MAP");
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                CastMiniMapRayToWorld(localCursor);
                Debug.Log("RIGHT CLICK ON MAP");
            }
            
        }
    }

    private void CastMiniMapRayToWorld(Vector2 localCursor)
    {
        Ray miniMapRay = miniMapCamera.ScreenPointToRay(new Vector2(localCursor.x * miniMapCamera.pixelWidth, localCursor.y * miniMapCamera.pixelHeight));

        if (Physics.Raycast(miniMapRay, out RaycastHit miniMapHit, Mathf.Infinity))
        {
            //Debug.Log("Object touched by the character controller raycast " + miniMapHit.collider.gameObject);

            player.GetComponent<CharacterController>().SetNavMeshDestinationWithRayCast(miniMapRay);
            GameObject go = Instantiate(movementFeedbackToInstantiate, miniMapHit.point, Quaternion.identity);
        }
    }
}
