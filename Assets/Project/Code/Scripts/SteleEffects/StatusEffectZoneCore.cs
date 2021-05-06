using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectZoneCore : MonoBehaviour
{
    protected List<EntityStats> entitiesFound = new List<EntityStats>();

    protected abstract void ApplyAffect(EntityStats target);
    protected abstract void RemoveEffect(EntityStats target);

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entityStats.EntityTeam != EntityTeam.DALVA)
        {
            RefreshList();
            entitiesFound.Add(entityStats);
            ApplyAffect(entityStats);
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
            RemoveEffect(entitiesFound[i]);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}
