using System.Collections.Generic;
using UnityEngine;

public class AbilityAreaOfEffectDisplayer : MonoBehaviour
{
    [SerializeField] private int groundLayer = 8;
    [SerializeField] private int entitiesLayer = 12;
    [SerializeField] private int ignoreRaycastLayer = 2;

    [SerializeField] private EntityStats Stats;
    public List<Transform> targets = new List<Transform>();

    private void OnDisable()
    {
        if(targets.Count >= 1)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                EntityDetection targetFound = targets[i].GetComponent<EntityDetection>();

                if (targetFound != null)
                    targetFound.DeactivateTargetOutlineOnHover(targets[i].GetComponent<Outline>());

                targets[i].gameObject.layer = 12;
            }

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
            Outline targetOutline = targets[i].GetComponent<Outline>();

            if (targetStats != null && targetOutline != null)
            {
                if (targets[i].GetComponent<EntityStats>().EntityTeam == Stats.EntityTeam)
                    targets[i].GetComponent<EntityDetection>().ActivateTargetOutlineOnHover(targets[i].GetComponent<Outline>(), Color.blue);
                else if (targets[i].GetComponent<EntityStats>().EntityTeam != Stats.EntityTeam)
                    targets[i].GetComponent<EntityDetection>().ActivateTargetOutlineOnHover(targets[i].GetComponent<Outline>(), Color.red);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Outline targetOutline = other.GetComponent<Outline>();

        //Enlever de la liste si other = i list
        if (targets.Count >= 1)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (other.transform.gameObject == targets[i].gameObject)
                {
                    if (targetOutline != null)
                        targets[i].GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(targets[i].GetComponent<Outline>());

                    targets[i].gameObject.layer = entitiesLayer;
                    targets.Remove(targets[i]);
                }
            }
        }
    }
}