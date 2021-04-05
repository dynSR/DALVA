using UnityEngine;

public class AbilityIndicator : MonoBehaviour
{
    [SerializeField] private bool isAttachedToPlayer = false;

    void LateUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                if (!isAttachedToPlayer)
                    transform.position = new Vector3(hit.point.x, 0.05f, hit.point.z);
                else if (isAttachedToPlayer)
                    transform.rotation = Quaternion.LookRotation(transform.forward, hit.point - transform.position);
            }
        }
    }
}
