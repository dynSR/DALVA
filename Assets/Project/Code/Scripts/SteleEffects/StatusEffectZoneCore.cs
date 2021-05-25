using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectZoneCore : MonoBehaviour
{
    public Color effectColor;

    protected abstract void ApplyAffect(EntityStats target);
    protected abstract void RemoveEffect(EntityStats target);

    private void OnTriggerEnter(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entityStats.EntityTeam != EntityTeam.DALVA)
        {
            ApplyAffect(entityStats);
            SetOutlineEffect(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();

        if (entityStats != null && entityStats.EntityTeam != EntityTeam.DALVA)
            SetOutlineEffect(other);
    }

    private void OnTriggerExit(Collider other)
    {
        EntityStats entityStats = other.GetComponent<EntityStats>();
        RemoveEffect(entityStats);
    }

    private void SetOutlineEffect(Collider other)
    {
        EntityDetection otherEntityDetection = other.GetComponent<EntityDetection>();
        otherEntityDetection.CanSetOutlineColorToBlack = false;

        if (otherEntityDetection.Outline != null && !otherEntityDetection.Outline.enabled)
        {
            Debug.Log("Reassigning outline color");
            otherEntityDetection.ActivateTargetOutlineOnHover(otherEntityDetection.Outline, effectColor);
        }  
    }

    private void ResetOutlineEffect(Collider other)
    {
        EntityDetection otherEntityDetection = other.GetComponent<EntityDetection>();
        otherEntityDetection.CanSetOutlineColorToBlack = true;

        if (otherEntityDetection.Outline != null && !otherEntityDetection.Outline.enabled)
        {
            Debug.Log("Reassigning outline color");
            otherEntityDetection.ActivateTargetOutlineOnHover(otherEntityDetection.Outline, Color.black);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius * transform.parent.localScale.magnitude / 1.73f);
    }
}
