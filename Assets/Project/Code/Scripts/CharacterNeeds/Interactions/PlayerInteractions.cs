using UnityEngine;

public class PlayerInteractions : InteractionSystem
{
    [SerializeField] private bool isHarvesting = false;
    public bool IsHarvesting { get => isHarvesting; set => isHarvesting = value; }

    protected override void Update()
    {
        base.Update();
        SetTargetOnMouseClick();
    }
   
    #region Set player's target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.RightClickIsPressed())
        {
            Debug.Log("Set target on mouse click");
            ResetTarget();

            if (GetComponent<PlayerController>().IsCursorHoveringUIElement) return;

            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<EntityDetection>() != null 
                    && hit.collider.GetComponent<EntityDetection>().enabled)
                {
                    Target = hit.collider.transform;

                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
                        StoppingDistance = Stats.GetStat(StatType.Attack_Range).Value;
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester)
                        StoppingDistance = InteractionRange;
                }
                else
                {
                    //Ground hit
                    Controller.Agent.isStopped = false;
                    Controller.Agent.stoppingDistance = 0.2f;
                }
            }
        }
    }
    #endregion

    #region Interaction
    private void ResetTarget()
    {
        if (Target != null)
        {
            if (Target.GetComponent<HarvesterLogic>() != null)
            {
                Target.GetComponent<HarvesterLogic>().ResetAfterInteraction();
            }
        }

        ResetInteractionState();

        Target = null;
    }

    public override void Interact()
    {
        base.Interact();

        HarvesterInteraction();
    }

    void HarvesterInteraction()
    {
        if (Target != null)
        {
            HarvesterLogic harvester = Target.GetComponent<HarvesterLogic>();

            if (harvester != null && harvester.IsInteractable)
            {
                IsHarvesting = true;
                Animator.SetBool("Attack", false);
                Animator.SetBool("IsCollecting", true);

                harvester.interactingPlayer = this;

                Debug.Log("Interaction with harvester !");
            }
        }
    }

    public override void ResetInteractionState()
    {
        base.ResetInteractionState();

        IsHarvesting = false;
        Animator.SetBool("IsCollecting", false);
    }
    #endregion
}