using System;
using System.Collections.Generic;
using UnityEngine;

public class ForestCamp : MonoBehaviour
{
    [SerializeField] private List<NPCController> npcControllers;
    [SerializeField] private List<Transform> startingPositions;
    private bool deathEventIsHandled = false;

    public GameObject minimapObject;

    [Header("SCALING")]
    public float scalingFactor = 0.01f;
    public List<Stat> statsToScale;

    public bool canScaleStats = false;

    private void OnEnable()
    {
        for (int i = 0; i < npcControllers.Count; i++)
        {
            npcControllers[i].OnExitingIdleState += SetEveryNPCToAggressionState;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < npcControllers.Count; i++)
        {
            npcControllers[i].OnExitingIdleState -= SetEveryNPCToAggressionState;
        }
    }

    void Awake() => SetNPCPositionAndRotation();

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //    ScaleEntitiesStats();
    }

    private void LateUpdate()
    {
        if(EveryEntityIsDead() && !deathEventIsHandled)
        {
            ProcessRespawnForEntities();
        }

        if (EveryEntityIsUp() && deathEventIsHandled)
        {
            ScaleEntitiesStats();
        }
    }

    private void ProcessRespawnForEntities()
    {
        foreach (NPCController controller in npcControllers)
        {
            StartCoroutine(controller.Stats.ProcessRespawnTimer(controller.Stats.TimeToRespawn));
        }
    }

    void DisplayCampMinimapIcon()
    {
        if (minimapObject != null && !minimapObject.activeInHierarchy)
        {
            minimapObject.SetActive(true);
        }
    }

    void HideCampMinimapIcon()
    {
        if (minimapObject != null && minimapObject.activeInHierarchy)
        {
            minimapObject.SetActive(false);
        }
    }

    bool EveryEntityIsDead()
    {
        bool everyEntityIsDead = false;
        int count = 0;

        foreach (NPCController controller in npcControllers)
        {
            if (controller.Stats.IsDead)
            {
                count++;
            }

            if (count >= npcControllers.Count)
            {
                everyEntityIsDead = true;
                HideCampMinimapIcon();
            }
            else everyEntityIsDead = false;
        }

        return everyEntityIsDead;
    }

    bool EveryEntityIsUp()
    {
        bool everyEntityIsUp = false;
        int count = 0;

        foreach (NPCController controller in npcControllers)
        {
            if (!controller.Stats.IsDead)
            {
                count++;
            }

            if (count >= npcControllers.Count)
            {
                everyEntityIsUp = true;
                DisplayCampMinimapIcon();
                deathEventIsHandled = false;
                StopAllCoroutines();
            }
            else everyEntityIsUp = false;
        }

        return everyEntityIsUp;
    }

    void SetNPCPositionAndRotation()
    {
        for (int i = 0; i < npcControllers.Count; i++)
        {
            npcControllers[i].StartingPosition = npcControllers[i].StartingPosition;
            npcControllers[i].transform.LookAt(npcControllers[i].PositionToLookAt);
        }
    }

    void SetEveryNPCToAggressionState()
    {
        int index = 0;

        for (int i = 0; i < npcControllers.Count; i++)
        {
            //Debug.Log("SetEveryNPCToAggressionState()");
            
            if(!npcControllers[i].IsInIdleState)
            {
                index = Array.IndexOf(npcControllers.ToArray(), npcControllers[i]);
                //Debug.Log(index);
            }

            for (int j = npcControllers.Count - 1; j >= 0; j--)
            {
                //And for the others update their target, target being npc's source of damage that have been attacked
                if (npcControllers[j].IsInIdleState)
                {
                    //Debug.Log(npcControllers[j].name);
                    //Debug.Log(npcControllers[index].name);

                    if (!npcControllers[j].NPCInteractions.HasATarget)
                    {
                        npcControllers[j].Stats.SourceOfDamage = npcControllers[index].Stats.SourceOfDamage;
                        npcControllers[j].SetSourceOfDamageAsTarget();
                    }
                }
            }
        }
    }

    void ScaleEntitiesStats()
    {
        if (!canScaleStats) return;

        Debug.Log("Scaling npc stats properties !");

        foreach (NPCController controller in npcControllers)
        {
            for (int i = 0; i < statsToScale.Count; i++)
            {
                for (int j = 0; j < controller.Stats.entityStats.Count; j++)
                {
                    if (statsToScale[i].StatType  == controller.Stats.entityStats[j].StatType)
                    {
                        if (controller.Stats.entityStats[j].Value <= 0) continue;

                        controller.Stats.entityStats[j].AddModifier(new StatModifier(scalingFactor, statsToScale[i].StatType, StatModType.PercentAdd, this));
                    }
                }
            }

            controller.Stats.UpdateStats();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        for (int i = 0; i < npcControllers.Count; i++)
        {
            Gizmos.DrawWireSphere(npcControllers[i].transform.position, npcControllers[i].GetComponent<NPCController>().AggressionLimitsValue);
        }
    }
}