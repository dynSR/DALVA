using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    Tower Tower => GetComponentInParent<Tower>();
    protected List<EntityStats> entitiesFound = new List<EntityStats>();

    private void Update()
    {
        if (Tower.CanAttack && entitiesFound.Count > 0 && entitiesFound[0] != null && !entitiesFound[0].IsDead)
            StartCoroutine(Tower.ShotProjectileOntoTarget(entitiesFound[0]));
        else if (entitiesFound.Count > 0 && entitiesFound[0] == null || entitiesFound[0].IsDead) RefreshList();
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entityStats.EntityTeam != EntityTeam.DALVA)
        {
            RefreshList();
            entitiesFound.Add(entityStats);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RefreshList();

        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entitiesFound.Contains(entityStats)) entitiesFound.Remove(entityStats);
    }


    void RefreshList()
    {
        for (int i = entitiesFound.Count - 1; i >= 0; i--)
        {
            if (entitiesFound[i] == null) entitiesFound.RemoveAt(i);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}
