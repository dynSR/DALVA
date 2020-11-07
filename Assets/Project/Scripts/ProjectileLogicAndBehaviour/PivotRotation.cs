using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PivotRotation : MonoBehaviourPun
{
    private Vector2 direction;
    private float angleToRotate;

    void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            RotateAroundAPivot(hit.point);
        }
    }

    public void RotateAroundAPivot(Vector3 pointToRotateTowards)
    {
        pointToRotateTowards = new Vector3(pointToRotateTowards.x, transform.position.y, pointToRotateTowards.z);
        transform.LookAt(pointToRotateTowards);
    }
}
