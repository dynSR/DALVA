using UnityEngine;

public class AbilityIndicator : MonoBehaviour
{
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z + 0.05f);
            }
        }
    }
}
