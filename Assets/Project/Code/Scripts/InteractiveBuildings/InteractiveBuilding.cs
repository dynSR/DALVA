using UnityEngine;

public class InteractiveBuilding : MonoBehaviour
{
    [SerializeField] private EntityTeam entityTeam;
    public PlayerInteractions InteractingPlayer /*{ get; set; }*/;
    private bool isInteractable = false;
    [SerializeField] private float reinitializationDelay = 45f;

    public EntityTeam EntityTeam { get => entityTeam; set => entityTeam = value; }
    protected EntityDetection EntityDetection => GetComponent<EntityDetection>();
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    public float ReinitializationDelay { get => reinitializationDelay; set => reinitializationDelay = value; }

    protected virtual void  LateUpdate()
    {
        EntityDetectionEnableState(IsInteractable);
    }

    public virtual void ResetAfterInteraction()
    {
        InteractingPlayer = null;
    }

    protected void EntityDetectionEnableState(bool isInteractive)
    {
        if (isInteractive && !EntityDetection.enabled)
        {
            EntityDetection.enabled = true;
        }
        else if (!isInteractive && EntityDetection.enabled)
        {
            EntityDetection.enabled = false;
        }
    }
}