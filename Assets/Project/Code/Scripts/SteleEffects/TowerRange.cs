using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    Tower Tower => GetComponentInParent<Tower>();
    TowerAmelioration TowerAmelioration => GetComponentInParent<TowerAmelioration>();
    public SphereCollider SphereCollider => GetComponent<SphereCollider>();
    protected List<EntityStats> entitiesFound = new List<EntityStats>();

    [SerializeField] private RotationPivot rotationPivot;

    void Start() => SetTowerRange(GetComponentInParent<EntityStats>().GetStat(StatType.AttackRange).Value);

    private void LateUpdate()
    {
        if (entitiesFound.Count == 0)
        {
            if (rotationPivot.gameObject.activeInHierarchy)
                rotationPivot.gameObject.SetActive(false);
            return;
        }

        RefreshList();

        if (Tower.CanAttack && entitiesFound.Count > 0 && (entitiesFound[0] == null || !entitiesFound[0].IsDead))
        {
            if (TowerAmelioration.FinalEvolutionNumber != 1)
            {
                StartCoroutine(Tower.ShotProjectileOntoTarget(entitiesFound[0]));
            }
            else if (TowerAmelioration.FinalEvolutionNumber == 1)
            {
                if (!rotationPivot.gameObject.activeInHierarchy)
                    rotationPivot.gameObject.SetActive(true);

                rotationPivot.HandleRotation(rotationPivot.transform, entitiesFound[0].transform.position, rotationPivot.RotateVelocity, rotationPivot.RotationSpeed);
            }
        }
        else if (!Tower.CanAttack && entitiesFound.Count > 0 && (entitiesFound[0] == null || entitiesFound[0].IsDead))
        {
            Debug.Log("entitiesFound.Count > 0 && (entitiesFound[0] == null || entitiesFound[0].IsDead)");
            ResetTrigger();
            Tower.CanAttack = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entityStats.EntityTeam != EntityTeam.DALVA && !entitiesFound.Contains(entityStats))
        {
            //RefreshList();
            entitiesFound.Add(entityStats);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //RefreshList();

        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entitiesFound.Contains(entityStats)) entitiesFound.Remove(entityStats);
    }

    void ResetTrigger()
    {
        SphereCollider.enabled = false;
        SphereCollider.enabled = true;
    }

    void RefreshList()
    {
        for (int i = entitiesFound.Count - 1; i >= 0; i--)
        {
            if (entitiesFound[i] == null || entitiesFound[i].IsDead) entitiesFound.Remove(entitiesFound[i]);
        }
    }

    public void SetTowerRange(float value)
    {
        SphereCollider.radius = value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, SphereCollider.radius);
    }
}
