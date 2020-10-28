using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    private Vector2 direction;
    private float angleToRotate;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            RotateAroundAPivot(hit.point);
        }
    }


    public void RotateAroundAPivot(Vector3 pointToRotateTowards)
    {
        //direction = pointToRotateTowards - transform.position;

        //angleToRotate = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        //transform.eulerAngles = new Vector3(0, 0, angleToRotate);

        pointToRotateTowards = new Vector3(pointToRotateTowards.x, transform.position.y, pointToRotateTowards.z);
        transform.LookAt(pointToRotateTowards);
    }
}
