using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LaunchGameObject : MonoBehaviour
{
    [SerializeField] private Transform emmiterPosition;
    [SerializeField] private LayerMask layer;
    
    public Transform EmmiterPosition { get => emmiterPosition; }

    public void LaunchProjectile(GameObject projectile, Transform spawnLocation)
    {
        Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);
    }

    public void TurnCharacterTowardsLaunchDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        GameObject renderer = transform.GetChild(0).gameObject;

        if (Physics.Raycast(ray, out hit, 100f, layer))
        {

            Vector3 targetPoint = ray.GetPoint(hit.distance);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - renderer.transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            renderer.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 150f * Time.deltaTime);
        }
    }
}
