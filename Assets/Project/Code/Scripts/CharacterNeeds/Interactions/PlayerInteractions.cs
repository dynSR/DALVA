using UnityEngine;
using Photon.Pun;

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
        if (UtilityClass.RightClickIsPressed() && !Controller.IsCasting
            && (GameObject.Find("GameNetworkManager") == null || GetComponent<PhotonView>().IsMine))
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
                    //Update your target for the other players
                    if (GameObject.Find("GameNetworkManager") != null)
                        GetComponent<PhotonView>().RPC("InteractionUpdate", RpcTarget.Others, hit.collider.gameObject.name, "Targeting", GetComponent<PhotonView>().ViewID);

                    //Needs to be modified to only include Player -Interactive building - Monster - Minion
                    //Target in an enemy entity
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyPlayer
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyMinion
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyStele
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Monster)
                        StoppingDistance = Stats.GetStat(StatType.AttackRange).Value;
                    //Target is an interactive building
                    if (Target.GetComponent<InteractiveBuilding>() != null)
                    {
                        if (Target.GetComponent<InteractiveBuilding>().EntityTeam == EntityTeam.NEUTRAL
                            || Target.GetComponent<InteractiveBuilding>().EntityTeam == Stats.EntityTeam) 
                            StoppingDistance = InteractionRange;
                        else if (Target.GetComponent<InteractiveBuilding>().EntityTeam != Stats.EntityTeam) 
                            StoppingDistance = Stats.GetStat(StatType.AttackRange).Value;
                    }  
                }
                else
                {
                    //Ground hit
                    if (GameObject.Find("GameNetworkManager") != null)
                        GetComponent<PhotonView>().RPC("InteractionUpdate", RpcTarget.Others, null, "Targeting", GetComponent<PhotonView>().ViewID);
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

            if (stele != null && stele.IsInteractable && stele.SteleState != SteleState.Active /*uncomment here if a stele can be interactive even when active*/)
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

    #region Network

    //InteractionUpdate
    [PunRPC]
    public void InteractionUpdate(string target, string interaction, int photonviewID, PhotonMessageInfo info)
    {
        if (interaction == "Targeting")
        {
            PlayerInteractions player = PhotonView.Find(photonviewID).GetComponent<PlayerInteractions>();

            player.ResetTarget();

            if (target != null)
            {
                player.Target = GameObject.Find(target).transform;

                if (player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyPlayer
                        || player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyMinion
                        || player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.EnemyStele
                        || player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Monster)
                    player.StoppingDistance = player.Stats.GetStat(StatType.AttackRange).Value;
                //Target is an interactive building
                if (player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester
                    || player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Stele)
                    player.StoppingDistance = player.InteractionRange;
            }
            else
            {
                player.ResetAgentState();
            }

        }
    }

    #endregion
}