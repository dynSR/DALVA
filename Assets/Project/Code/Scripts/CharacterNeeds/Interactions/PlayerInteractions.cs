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
            Target = null;

            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<EntityDetection>() != null 
                    && hit.collider.GetComponent<EntityDetection>().enabled)
                {
                    Target = hit.collider.transform;
                    ResetInteractionState();

                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
                        StoppingDistance = CharacterStats.GetStat(StatType.Attack_Range).Value;
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester)
                        StoppingDistance = InteractionRange;
                }
                // Ground hit
                else
                {
                    CharacterController.Agent.isStopped = false;
                    CharacterController.Agent.stoppingDistance = 0.2f;
                    ResetInteractionState();
                }
            }
        }
    }
    #endregion

    #region Interaction
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

            if (harvester != null && harvester.IsInteractable && harvester.interactingPlayer == null)
            {
                IsHarvesting = true;

                harvester.interactingPlayer = transform.GetComponent<InteractionSystem>();

                CharacterAnimator.SetBool("Attack", false);
                CharacterAnimator.SetBool("IsCollecting", true);

                //Debug.Log("Interaction with harvester !");
            }
        }
    }

    public override void ResetInteractionState()
    {
        base.ResetInteractionState();

        IsHarvesting = false;
        CharacterAnimator.SetBool("IsCollecting", false);
    }
    #endregion
}