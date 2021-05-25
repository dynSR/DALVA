using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectZoneCore : MonoBehaviour
{
    public List<EntityStats> statsOfEntitiesInTrigger = new List<EntityStats>();

    protected abstract void ApplyAffect(EntityStats target);
    protected abstract void RemoveEffect(EntityStats target);

    private void OnDisable()
    {
        RemoveEffectOnEachEntitiesFound();
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entityStats.EntityTeam != EntityTeam.DALVA)
        {
            //RemoveEffect(entityStats);
            ApplyAffect(entityStats);
            statsOfEntitiesInTrigger.Add(entityStats);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();
        RemoveEffect(entityStats);

        if(statsOfEntitiesInTrigger.Contains(entityStats)) statsOfEntitiesInTrigger.Remove(entityStats);
    }

    public void RemoveEffectOnEachEntitiesFound()
    {
        for (int i = 0; i < statsOfEntitiesInTrigger.Count; i++)
        {
            RemoveEffect(statsOfEntitiesInTrigger[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if(transform.parent  != null)
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius * transform.parent.localScale.magnitude / 1.73f);
    }
}
