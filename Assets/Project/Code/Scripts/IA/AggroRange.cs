using System.Collections;
using UnityEngine;

public class AggroRange : MonoBehaviour
{
    #region Refs
    private Collider m_Collider => GetComponent<Collider>();
    private NPCController Controller => GetComponentInParent<NPCController>();
    private NPCInteractions Interactions => GetComponentInParent<NPCInteractions>();
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        AssignTarget(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ResetTarget(other);
    }

    private void AssignTarget(Collider other)
    {
        if (other.GetComponent<EntityDetection>() != null
            && !other.GetComponent<CharacterStat>().IsDead
            && other.GetComponent<VisibilityState>().IsVisible
            && other.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
        {
            if (Interactions.HasATarget) return;
            else
                Controller.Interactions.Target = other.transform;
        }
    }

    private void ResetTarget(Collider other)
    {
        if (Interactions.HasATarget && other.gameObject == Controller.Interactions.Target.gameObject) 
            Controller.Interactions.Target = null;
    }

    public void CheckForNewTarget()
    {
        StartCoroutine(ToggleColliderComponent());
    }

    private IEnumerator ToggleColliderComponent()
    {
        m_Collider.enabled = false;

        yield return new WaitForSeconds(.2f);

        m_Collider.enabled = true;
    }
}