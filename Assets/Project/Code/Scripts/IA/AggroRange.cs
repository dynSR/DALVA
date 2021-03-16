using UnityEngine;

public class AggroRange : MonoBehaviour
{
    private NPCController Controller => GetComponentInParent<NPCController>();
    private NPCInteractions Interactions => GetComponentInParent<NPCInteractions>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EntityDetection>() != null
            && other.GetComponent<VisibilityState>().IsVisible
            && other.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
        {
            if (Interactions.HasATarget) return;
            else
                Controller.Interactions.Target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Interactions.HasATarget) return;

        if (Controller.Interactions.Target == other.transform)
        {
            Controller.Interactions.Target = null;
        }
    }
}