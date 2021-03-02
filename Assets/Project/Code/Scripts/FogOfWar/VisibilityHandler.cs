using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnVisibilityChanged(Transform transform);

public class VisibilityHandler : MonoBehaviour
{
    [SerializeField] private int entitiesLayerValue;

    //public static event OnVisibilityChanged ;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == entitiesLayerValue && 
            other.GetComponent<VisibilityState>() != null &&
            !other.GetComponent<VisibilityState>().IsVisible)
        {
            //Debug.Log(other.name + " IN");
            other.GetComponent<VisibilityState>().SetToVisible();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == entitiesLayerValue && 
            other.GetComponent<VisibilityState>() != null &&
            other.GetComponent<VisibilityState>().IsVisible)
        {
            //Debug.Log(other.name + " OUT");
            other.GetComponent<VisibilityState>().SetToInvisible();
        }
    }
}