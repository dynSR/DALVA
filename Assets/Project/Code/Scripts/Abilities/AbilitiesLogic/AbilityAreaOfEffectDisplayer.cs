using System.Collections.Generic;
using UnityEngine;

public class AbilityAreaOfEffectDisplayer : MonoBehaviour
{
    [SerializeField] private EntityStats Stats;
    private List<Transform> targets = new List<Transform>();

    private void OnDisable()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(targets[i].GetComponent<Outline>());
            targets[i].gameObject.layer = 12;
        }

        targets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats targetStats = other.GetComponent<EntityStats>();
        Outline targetOutline = other.GetComponent<Outline>();

        //Ajouter dans la liste si a des stats et est un ennemi
        if (targetStats != null && targetOutline != null)
        {
            targets.Add(other.transform);

            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].gameObject.layer = 2;

                if (Stats.EntityTeam == targets[i].GetComponent<EntityStats>().EntityTeam) 
                    targets[i].GetComponent<EntityDetection>().ActivateTargetOutlineOnHover(targets[i].GetComponent<Outline>(), Color.blue);
                else if (Stats.EntityTeam != targets[i].GetComponent<EntityStats>().EntityTeam) 
                    targets[i].GetComponent<EntityDetection>().ActivateTargetOutlineOnHover(targets[i].GetComponent<Outline>(), Color.red);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Enlever de la liste si other = i list

        for (int i = 0; i < targets.Count; i++)
        {
            if (other.transform.gameObject == targets[i].gameObject)
            {
                targets[i].GetComponent<EntityDetection>().DeactivateTargetOutlineOnHover(targets[i].GetComponent<Outline>());
                targets[i].gameObject.layer = 12;
                targets.Remove(targets[i]);
            }
        }
    }
}