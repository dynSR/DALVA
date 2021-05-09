using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    Tower Tower => GetComponentInParent<Tower>();
    public SphereCollider SphereCollider => GetComponent<SphereCollider>();
    protected List<EntityStats> entitiesFound = new List<EntityStats>();


    void Start() => SetTowerRange(GetComponentInParent<EntityStats>().GetStat(StatType.AttackRange).Value);

    private void Update()
    {
        if (entitiesFound.Count == 0) return;

        if (Tower.CanAttack && entitiesFound.Count > 0 && entitiesFound[0] != null && !entitiesFound[0].IsDead)
            StartCoroutine(Tower.ShotProjectileOntoTarget(entitiesFound[0]));
        else if (entitiesFound.Count > 0 && entitiesFound[0] == null || entitiesFound[0].IsDead)
        {
            Tower.CanAttack = true;
            RefreshList();
        }
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

    public void SetTowerRange(float value)
    {
        SphereCollider.radius = value;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, GetComponentInParent<EntityStats>().GetStat(StatType.AttackRange).Value);
    }
}
