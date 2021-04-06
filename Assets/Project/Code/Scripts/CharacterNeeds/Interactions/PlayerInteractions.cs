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
                    //Update your target for the other player
                    if(GameObject.Find("GameNetworkManager") != null)
                    GetComponent<PhotonView>().RPC("InteractionUpdate", RpcTarget.Others, hit.collider.gameObject.name, "Targeting", GetComponent<PhotonView>().ViewID);

                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
                        StoppingDistance = Stats.GetStat(StatType.AttackRange).Value;
                    if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester
                        || Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Stele)
                        StoppingDistance = InteractionRange;
                }
                else
                {
                    //Ground hit
                    if (GameObject.Find("GameNetworkManager") != null)
                        GetComponent<PhotonView>().RPC("InteractionUpdate", RpcTarget.Others, null, "Targeting", GetComponent<PhotonView>().ViewID);
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
            else if (Target.GetComponent<SteleLogic>() != null && Target.GetComponent<SteleLogic>().SteleState != SteleState.Active  /* + vérification team ? */)
            {
                Target.GetComponent<SteleLogic>().InteractingPlayer = null;
            }
        }

        ResetInteractionState();

        Target = null;
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
                if (player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy)
                    player.StoppingDistance = player.Stats.GetStat(StatType.AttackRange).Value;
                if (player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester
                    || player.Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Stele)
                    player.StoppingDistance = player.InteractionRange;
            }
            else
            {
                player.Controller.Agent.isStopped = false;
                player.Controller.Agent.stoppingDistance = 0.2f;
            }

        }
    }

    #endregion
}