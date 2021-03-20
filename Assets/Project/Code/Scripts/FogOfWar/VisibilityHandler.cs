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
}