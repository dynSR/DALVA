using System.Collections;
using UnityEngine;

public class RotationPivot : MonoBehaviour
{
    public float RotationSpeed;
    public float RotateVelocity;

    public void HandleRotation(Transform transform, Vector3 target, float rotateVelocity, float rotationSpeed)
    {
        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            rotationSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }
}