using UnityEngine;

public class AbilityIndicator : MonoBehaviour
{
    [SerializeField] private bool isAttachedToPlayer = false;

    void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                if (!isAttachedToPlayer)
                    transform.position = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z + 0.05f);
                else if (isAttachedToPlayer)
                    transform.rotation = Quaternion.LookRotation(transform.forward, hit.point - transform.position);
            }
        }
    }
}
