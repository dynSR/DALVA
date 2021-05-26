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
        SortEntities();

        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && !entityStats.IsDead && entityStats.EntityTeam != EntityTeam.DALVA)
        {
            ApplyAffect(entityStats);
            statsOfEntitiesInTrigger.Add(entityStats);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SortEntities();

        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && !entityStats.IsDead && statsOfEntitiesInTrigger.Contains(entityStats))
        {
            RemoveEffect(entityStats);

            statsOfEntitiesInTrigger.Remove(entityStats);
        }
    }

    void SortEntities()
    {
        for (int i = 0; i < statsOfEntitiesInTrigger.Count; i++)
        {
            if (statsOfEntitiesInTrigger[i].IsDead)
            {
                RemoveEffect(statsOfEntitiesInTrigger[i]);
                statsOfEntitiesInTrigger.Remove(statsOfEntitiesInTrigger[i]);
                statsOfEntitiesInTrigger.Sort();
            }
        }
    }

    public void RemoveEffectOnEachEntitiesFound()
    {
        for (int i = 0; i < statsOfEntitiesInTrigger.Count; i++)
        {
            RemoveEffect(statsOfEntitiesInTrigger[i]);
            statsOfEntitiesInTrigger.Clear();
        }
    }

    public void AugmentZoneRange(float scaleValue)
    {
        transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
    }

    public void ResetTrigger()
    {
        Collider colliderAttached = GetComponent<Collider>();

        colliderAttached.enabled = false;
        colliderAttached.enabled = true;

        RemoveEffectOnEachEntitiesFound();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if(transform.parent  != null)
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius * transform.parent.localScale.magnitude / 1.73f);
    }
}
