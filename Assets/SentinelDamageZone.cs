using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelDamageZone : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private EntityStats stats;
    [SerializeField] private float delayBetweenDamageApplications = 0.15f;
    private List<EntityStats> entitiesFound = new List<EntityStats>();

    private void Start()
    {
        InvokeRepeating("ApplyDamageOverTime", 1, delayBetweenDamageApplications);
    }

    void LateUpdate()
    {
        RefreshList();
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

    void ApplyDamageOverTime()
    {
        if (entitiesFound.Count <= 0) return;

        for (int i = 0; i < entitiesFound.Count; i++)
        {
            if (entitiesFound[i] != null && !entitiesFound[i].IsDead)
            {
                entitiesFound[i].TakeDamage(parent, 0, 0, stats.GetStat(StatType.PhysicalPower).Value, 0, 0, 0, 0, 0);
            }
        }
    }
}
