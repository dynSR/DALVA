using UnityEngine;

public class VisibilityHandler : MonoBehaviour
{
    [SerializeField] private int entitiesLayerValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == entitiesLayerValue && 
            other.GetComponent<VisibilityState>() != null)
        {
            //Debug.Log(other.name + " IN");
            other.GetComponent<VisibilityState>().SetToVisible();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == entitiesLayerValue && 
            other.GetComponent<VisibilityState>() != null)
        {
            //Debug.Log(other.name + " OUT");
            other.GetComponent<VisibilityState>().SetToInvisible();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == entitiesLayerValue &&
            other.gameObject.GetComponent<VisibilityState>() != null)
        {
            //Debug.Log(other.name + " IN");
            other.gameObject.GetComponent<VisibilityState>().SetToVisible();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == entitiesLayerValue &&
           other.gameObject.GetComponent<VisibilityState>() != null)
        {
            //Debug.Log(other.name + " OUT");
            other.gameObject.GetComponent<VisibilityState>().SetToInvisible();
        }
    }
}