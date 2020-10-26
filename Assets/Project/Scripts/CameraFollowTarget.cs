using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float followingSpeed;
    [SerializeField] private Vector3 cameraOffset;

    void Update()
    {
        FollowATarget(targetToFollow);
    }

    void FollowATarget(Transform targetToFollow)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetToFollow.position.x , targetToFollow.position.y + cameraOffset.y, targetToFollow.position.z + cameraOffset.z), followingSpeed);
    }
}
