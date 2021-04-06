using System.Collections.Generic;
using UnityEngine;

public class VisibilityHandler : MonoBehaviour
{
    [SerializeField] private int entitiesLayerValue;
    [SerializeField] private int obstaclesLayerValue;
    [SerializeField] List<Transform> entitiesInRange = new List<Transform>();
    [SerializeField] List<Transform> visibleEntities = new List<Transform>();

    private void LateUpdate()
    {
        //if (entitiesInRange.Count >= 1)
        //{
        //    ////visibleEntities.Clear();

        //    //for (int i = 0; i < entitiesInRange.Count; i++)
        //    //{
        //    //    if (Physics.Linecast(transform.position, entitiesInRange[i].position, out RaycastHit hit))
        //    //    {
        //    //        Transform target = hit.transform;

        //    //        if (target.gameObject.layer != obstaclesLayerValue)
        //    //        {
        //    //            Debug.Log("hit an entity");

        //    //            if (!target.GetComponent<VisibilityState>().IsVisible)
        //    //            {
        //    //                if (!visibleEntities.Contains(target))
        //    //                    visibleEntities.Add(target);

        //    //                target.GetComponent<VisibilityState>().SetToVisible();
        //    //            }

        //    //            if (visibleEntities.Count <= 0 || !visibleEntities.Contains(entitiesInRange[i]))
        //    //            {
        //    //                entitiesInRange[i].GetComponent<VisibilityState>().SetToInvisible();
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == entitiesLayerValue &&
            other.GetComponent<VisibilityState>() != null)
        {
            //Debug.Log(other.name + " IN");
            if(!entitiesInRange.Contains(other.transform))
                entitiesInRange.Add(other.transform);
            //other.GetComponent<VisibilityState>().SetToVisible();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.layer == entitiesLayerValue && 
        //    other.GetComponent<VisibilityState>() != null)
        //{
        //    for (int i = 0; i < entitiesInRange.Count; i++)
        //    {
        //        if (other.transform == entitiesInRange[i])
        //        {
        //            entitiesInRange[i].GetComponent<VisibilityState>().SetToInvisible();
        //            entitiesInRange.Remove(entitiesInRange[i]);
        //        }
        //    }
        //}
    }
}