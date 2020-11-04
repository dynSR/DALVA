using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float followingSpeed;
    [Tooltip("Il faut mettre la moitié de la valeur de l'axe Y (en négatif) sur l'axe Z pour avoir le personnage visé par la caméra au centre de l'écran")]
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
