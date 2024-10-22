﻿using System.Collections;
using UnityEngine;

public class AggroRange : MonoBehaviour
{
    #region Refs
    private SphereCollider m_Collider => GetComponent<SphereCollider>();
    private NPCController Controller => GetComponentInParent<NPCController>();
    private NPCInteractions Interactions => GetComponentInParent<NPCInteractions>();
    private EntityStats Stats => GetComponentInParent<EntityStats>();
    #endregion

    public bool isAGuardian = false;

    private void OnTriggerEnter(Collider other)
    {
        AssignTarget(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ResetTarget(other);
        //CheckForNewTarget();
    }

    private void AssignTarget(Collider other)
    {
        EntityDetection entityFound = other.GetComponent<EntityDetection>();
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null 
            && entityStats.EntityTeam != Stats.EntityTeam
            //&& entityStats.EntityTeam != EntityTeam.NEUTRAL
            && ((!entityFound.ThisTargetIsAMonster(entityFound) && (!isAGuardian || isAGuardian)) || (entityFound.ThisTargetIsAMonster(entityFound) && isAGuardian))
            && !entityFound.ThisTargetIsAStele(entityFound)
            && !entityFound.ThisTargetIsASteleEffect(entityFound))
        {
            if (!entityStats.IsDead)
            {
                Debug.Log(entityStats.name, transform);

                if (Interactions.HasATarget) return;
                else
                    Interactions.Target = other.transform;
            }
        }
    }

    private void ResetTarget(Collider other)
    {
        if (Interactions.HasATarget && other.gameObject == Interactions.Target.gameObject) 
            Interactions.Target = null;
    }

    public void CheckForNewTarget()
    {
        StartCoroutine(ToggleColliderComponent());
    }

    private IEnumerator ToggleColliderComponent()
    {
        m_Collider.enabled = false;

        yield return new WaitForEndOfFrame();

        m_Collider.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Color color = Color.yellow;
        Gizmos.color = color;

        Gizmos.DrawWireSphere(transform.position, m_Collider.radius);
    }
}