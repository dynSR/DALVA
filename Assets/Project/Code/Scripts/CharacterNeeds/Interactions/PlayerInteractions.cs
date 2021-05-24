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

    private CursorLogic CursorLogicAttached => GetComponent<CursorLogic>();

    protected override void Update()
    {
        base.Update();

        if (!GameManager.Instance.GameIsInPlayMod() || Controller.IsCasting) return;

        SetTargetOnMouseClick();
    }
   
    #region Set player's target when he clicks on an enemy entity
    void SetTargetOnMouseClick()
    {
        if (UtilityClass.IsKeyPressed(AttackOnMoveInput))
        {
            CursorLogicAttached.SetCursorToAttackOnMoveAppearance();
            SetAttackRangeSize();
            DisplayAttackRange();
            PlayerIsTryingToAttack = true;
            return;
        }
        else if (PlayerIsTryingToAttack && UtilityClass.LeftClickIsPressed())
        {
            HideAttackRange();
            CursorLogicAttached.SetCursorToNormalAppearance();
            PlayerIsTryingToAttack = false;
        }

        if (UtilityClass.RightClickIsPressed())
        {
            //Debug.Log("Set target on mouse click");
            ResetTarget();

            if (GetComponent<PlayerController>().IsCursorHoveringUIElement) return;

            if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
            {
                EntityDetection targetFound = hit.collider.GetComponent<EntityDetection>();
                InteractiveBuilding interactiveBuilding = hit.collider.GetComponent<InteractiveBuilding>();

                if (targetFound != null && targetFound.enabled && !targetFound.ThisTargetIsASteleEffect(targetFound))
                {
                    Target = targetFound.transform;
                    //Controller.HandleCharacterRotation(Target);

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
            else if (Target.GetComponent<SteleLogic>() != null)
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
        if (Controller.IsCasting) return;

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

    private void SetAttackRangeSize()
    {
        attackRange.GetComponent<SphereCollider>().radius = Stats.GetStat(StatType.AttackRange).Value;

        SpriteRenderer attackRangeRenderer = attackRange.GetComponent<SpriteRenderer>();
        attackRangeRenderer.size = new Vector2(Stats.GetStat(StatType.AttackRange).Value * 2, Stats.GetStat(StatType.AttackRange).Value * 2);
    }

    private void DisplayAttackRange()
    {
        attackRange.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void HideAttackRange()
    {
        attackRange.GetComponent<SpriteRenderer>().enabled = false;
    }
    #endregion
}