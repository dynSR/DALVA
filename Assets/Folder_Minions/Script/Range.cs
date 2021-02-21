using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    private Sc_Minions_Base parent;
    [SerializeField]
    private List<Transform> targets = new List<Transform>();
    private void Start()
    {
        parent = GetComponentInParent<Sc_Minions_Base>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(parent.enemyLayer) && !targets.Contains(other.transform))
        {
            Debug.Log("add "+ other.name);
            targets.Add(other.transform);
            if(parent.Target == null)
            {
                parent.Target = other.transform;
                Debug.Log("Target " + parent.Target.name);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(parent.enemyLayer) && parent.Target == other.transform)
        {
            parent.Target = null;
        }
    }

    public Transform GetNearestTarget()
    {
        Debug.Log("GetNearestTargetFonction");
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;
        Vector3 currentPos = transform.position;
        if (parent.Target == null)
        {
            targets.Remove(parent.Target);
        }
        for (int i = 0; i < targets.Count; i++)
        {
            if(targets[i]==null)
            {
                targets.Remove(targets[i]);
            }
        }
        foreach (Transform target in targets)
        {
            Debug.Log("enterInForeach");
            Vector3 difference = target.position - currentPos;
            float distanceToEnemy = difference.magnitude;
            Debug.Log("TargetOnForeach " + target);
            if (distanceToEnemy < shortestDistance)
            {
                Debug.Log("enterInIfDistanceToEnemy<ShortestDistance " + distanceToEnemy + " " + shortestDistance);
                Debug.Log("TargetInForeach " + target);
                shortestDistance = distanceToEnemy;
                nearestTarget = target;
                Debug.Log("ShortestDistance==distanceToEnemy  " + shortestDistance);
                Debug.Log("nearestTargetInForeach " + nearestTarget);
            }
        }

        if (nearestTarget != null && shortestDistance <= GetComponent<SphereCollider>().radius)
        {
            Debug.Log("nearestTarget "+ nearestTarget.name);
            return nearestTarget;
        }
        else
        {
            Debug.Log("return null");
            Debug.Log("nearestTarget " + nearestTarget);
            return null;
        }
    }
}
