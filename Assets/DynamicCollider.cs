using System.Collections.Generic;
using UnityEngine;

public class DynamicCollider : MonoBehaviour
{
    private MeshFilter MeshFilter => GetComponent<MeshFilter>();
    private MeshCollider MeshCollider => GetComponent<MeshCollider>();

    private void LateUpdate()
    {
        //MeshCollider.sharedMesh = MeshFilter.mesh;
        //MeshCollider.sharedMesh = null;
        //MeshCollider.sharedMesh = MeshFilter.mesh;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("LOL HEHE AHAH HIHI OUHOUH");
    }
}