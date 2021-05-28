using System.Collections.Generic;
using UnityEngine;

public enum ZoneType
{
    None,
    Frost,
    Weakness, 
    Burn
}

public abstract class StatusEffectZoneCore : MonoBehaviour
{
    [SerializeField] private ZoneType zoneType = ZoneType.None;
    public List<EntityStats> statsOfEntitiesInTrigger = new List<EntityStats>();

    protected abstract void ApplyAffect(EntityStats target);
    protected abstract void RemoveEffect(EntityStats target);

    private void OnDestroy()
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
        if (statsOfEntitiesInTrigger.Count >= 1) return;

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
        if (statsOfEntitiesInTrigger.Count == 0) return;

        Debug.Log("RemoveEffectOnEachEntitiesFound");

        for (int i = 0; i < statsOfEntitiesInTrigger.Count; i++)
        {
            RemoveEffect(statsOfEntitiesInTrigger[i]);

            Debug.Log("EH OH WHY IT IS NOT WORKING ?");

            switch (zoneType)
            {
                case ZoneType.Frost:
                    statsOfEntitiesInTrigger[i].Controller.DeactivateSlowVFX();
                    break;
                case ZoneType.Weakness:
                    statsOfEntitiesInTrigger[i].Controller.DeactivatePoisonVFX();
                    break;
                case ZoneType.Burn:
                    statsOfEntitiesInTrigger[i].Controller.DeactivateBurnVFX();
                    break;
            }
        }

        statsOfEntitiesInTrigger.Clear();
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
