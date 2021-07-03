using UnityEngine;
using Photon.Pun;

public class PlayerInteractions : InteractionSystem
{
    [SerializeField] private KeyCode attackOnMoveInput;
    [SerializeField] private GameObject attackRange;
    private bool playerIsTryingToAttack = false;
    public bool PlayerIsTryingToAttack { get => playerIsTryingToAttack; set => playerIsTryingToAttack = value; }

    [SerializeField] private bool isHarvesting = false;
    [SerializeField] private bool isInteractingWithAStele = false;
    public KeyCode AttackOnMoveInput { get => attackOnMoveInput; set => attackOnMoveInput = value; }
    public bool IsHarvesting { get => isHarvesting; set => isHarvesting = value; }
    public bool IsInteractingWithAStele { get => isInteractingWithAStele; set => isInteractingWithAStele = value; }

    protected override void Update()
    {
        base.Update();

        if (!GameManager.Instance.GameIsInPlayMod() || Controller.IsCasting) return;

        SetTargetOnMouseClick();
    }
   
    #region Set player's target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.RightClickIsPressed())
        {
            if (GetComponent<PlayerController>().IsCursorHoveringUIElement) return;

            //Debug.Log("Set target on mouse click");
            ResetTarget();

            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                EntityDetection targetFound = hit.collider.GetComponent<EntityDetection>();
                InteractiveBuilding interactiveBuilding = hit.collider.GetComponent<InteractiveBuilding>();

                if (targetFound != null 
                    && targetFound.enabled 
                    && !targetFound.ThisTargetIsASteleEffect(targetFound))
                {
                    //Ajouter la détection d'une autre cible éventuelle pour la mettre en queue ppour pouvoir l'attaquer en suivant
                    //Target = targetFound.transform;

                    AssignTarget(targetFound.transform);

                    targetFound.DisplaySelectionEffect();

                    //Needs to be modified to only include Player -Interactive building - Monster - Minion
                    //Target in an enemy entity
                    if (targetFound.ThisTargetIsAPlayer(targetFound)
                        || targetFound.ThisTargetIsAMonster(targetFound)
                        || targetFound.ThisTargetIsAMinion(targetFound))
                    {
                        StoppingDistance = Stats.GetStat(StatType.AttackRange).Value;
                    }
                    //Target is an interactive building
                    if (interactiveBuilding != null)
                    {
                        if (interactiveBuilding.EntityTeam == EntityTeam.NEUTRAL || interactiveBuilding.EntityTeam == Stats.EntityTeam)
                            StoppingDistance = InteractionRange;
                    }

                    MoveTowardsAnExistingTarget(Target, StoppingDistance);
                }
                else
                {
                    //Ground hit
                    IsAttacking = false;

                    ResetTarget(true);
                    ResetAgentState();
                }
            }
        }
    }
    #endregion

    #region Interaction
    public void ResetTarget(bool canResetTarget = false)
    {
        if (Target != null && Target != LastKnownTarget)
        {
            if (Target.GetComponent<HarvesterLogic>() != null)
            {
                Target.GetComponent<HarvesterLogic>().ResetAfterInteraction();
                Target = null;
            }
            else if (Target.GetComponent<SteleLogic>() != null)
            {
                Target.GetComponent<SteleLogic>().InteractingPlayer = null;
                Target = null;
            }
            else if (canResetTarget)
            {
                Target = null;
            }
        }

        ResetAgentState();

        ResetInteractionState();

        //Target = null;
    }

    void ResetAgentState()
    {
        if(Controller.Agent.enabled)
        {
            Controller.Agent.isStopped = false;
            Controller.Agent.stoppingDistance = 0.2f;
        }
    }

    public override void Interact()
    {
        if (Controller.IsCasting) return;

        Controller.HandleCharacterRotationBeforeCasting(transform, Target.position, Controller.RotateVelocity, Controller.RotationSpeed);

        base.Interact();

        HarvesterInteraction();
        SteleInteraction();
    }

    void SteleInteraction()
    {
        if (Target != null)
        {
            SteleLogic stele = Target.GetComponent<SteleLogic>();

            if (stele != null && stele.IsInteractable)
            {
                IsInteractingWithAStele = true;
                Animator.SetBool("Attack", false);
                Animator.SetBool("IsCollecting", false);

                //CursorLogicAttached.SetCursorToNormalAppearance();

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

    private void AssignTarget(Transform targetFound)
    {
        if(targetFound.gameObject == gameObject)
        {
            Debug.Log("Its character mage gameObject");
            return;
        }

        if(Target == null)
        {
            Target = targetFound;
        }
        else if (Target != null && IsAttacking)
        {
            //Cas d'une cible récolteur ou stèle
            EntityDetection entityDetected = targetFound.GetComponent<EntityDetection>();

            if (entityDetected != null
                && (entityDetected.ThisTargetIsAHarvester(entityDetected)
                || entityDetected.ThisTargetIsAStele(entityDetected)))
            {
                Target = targetFound;

                if (QueuedTarget != null) QueuedTarget = null;

                IsAttacking = false;
                
                return;
            }
            //
            if (targetFound != Target)
                QueuedTarget = targetFound;
        }
    }
}