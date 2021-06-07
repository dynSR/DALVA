using System.Collections.Generic;
using UnityEngine;

public class AbilityAreaOfEffectDisplayer : MonoBehaviour
{
    [SerializeField] private int groundLayer = 8;
    [SerializeField] private int entitiesLayer = 12;
    [SerializeField] private int ignoreRaycastLayer = 2;

    [SerializeField] private EntityStats Stats;
    public List<Transform> targets = new List<Transform>();

    [SerializeField] private Color targetOutlineColor;

    private void OnDisable()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            EntityDetection targetFound = targets[i].GetComponent<EntityDetection>();

            if (targetFound != null)
                targetFound.DeactivateTargetOutlineOnHover(targetFound.Outline);

            targets[i].gameObject.layer = 12;
        }

        if (targets.Count >= 1)
        {
            targets.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Héhé");
        }

        if (other.gameObject.layer != groundLayer 
            && other.gameObject.layer != ignoreRaycastLayer 
            && !targets.Contains(other.transform))
            targets.Add(other.transform);

        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].gameObject.layer = ignoreRaycastLayer;

            EntityStats targetStats = targets[i].GetComponent<EntityStats>();
            EntityDetection entityDetected = targets[i].GetComponent<EntityDetection>();

            if (targetStats != null && entityDetected != null)
            {
                if (targets[i].GetComponent<EntityStats>().EntityTeam == Stats.EntityTeam)
                    entityDetected.ActivateTargetOutlineOnHover(entityDetected.Outline, Color.blue);
                else if (targets[i].GetComponent<EntityStats>().EntityTeam != Stats.EntityTeam)
                    entityDetected.ActivateTargetOutlineOnHover(entityDetected.Outline, targetOutlineColor);
            }
        }
    } 

    private void OnTriggerExit(Collider other)
    {
        EntityDetection targetDetected = other.GetComponent<EntityDetection>();

        //Enlever de la liste si other = i list
        if (targets.Count >= 1)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (other.transform.gameObject == targets[i].gameObject)
                {
                    if (targetDetected != null)
                        targetDetected.DeactivateTargetOutlineOnHover(targetDetected.Outline);

                    targets[i].gameObject.layer = entitiesLayer;
                    targets.Remove(targets[i]);
                }
            }
        }
    }
}