using UnityEngine;

public class PlayerInteractions : InteractionSystem
{
    [SerializeField] private bool isHarvesting = false;
    [SerializeField] private bool isInteractingWithAStele = false;
    public bool IsHarvesting { get => isHarvesting; set => isHarvesting = value; }
    public bool IsInteractingWithAStele { get => isInteractingWithAStele; set => isInteractingWithAStele = value; }

    protected override void Update()
    {
        base.Update();
        SetTargetOnMouseClick();
    }
   
    #region Set player's target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.RightClickIsPressed() && !Controller.IsCasting)
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

                    //Target in an enemy entity
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyPlayer
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyMinion
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyStele
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Monster)
                        StoppingDistance = Stats.GetStat(StatType.AttackRange).Value;
                    //Target is an interactive building
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Stele)
                        StoppingDistance = InteractionRange;
                }
                else
                {
                    //Ground hit
                    ResetAgentState();
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
            else if (Target.GetComponent<SteleLogic>() != null && Target.GetComponent<SteleLogic>().SteleState != SteleState.Active  /* + vérification team ? */)
            {
                Target.GetComponent<SteleLogic>().InteractingPlayer = null;
            }
        }

        ResetAgentState();

        ResetInteractionState();

        Target = null;
    }

    void ResetAgentState()
    {
        Controller.Agent.isStopped = false;
        Controller.Agent.stoppingDistance = 0.2f;
    }

    public override void Interact()
    {
        base.Interact();

        HarvesterInteraction();
        SteleInteraction();
    }

    void SteleInteraction()
    {
        if (Target != null)
        {
            SteleLogic stele = Target.GetComponent<SteleLogic>();

            if (stele != null && stele.IsInteractable && stele.SteleState != SteleState.Active  /* + vérification team ? */)
            {
                IsInteractingWithAStele = true;
                Animator.SetBool("Attack", false);
                Animator.SetBool("IsCollecting", false);

                stele.InteractingPlayer = this;

                Debug.Log("Interaction with a stele !");
            }
        }
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

                harvester.InteractingPlayer = this;

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