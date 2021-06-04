using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestCamp : MonoBehaviour
{
    [SerializeField] private List<NPCController> npcControllers;
    [SerializeField] private List<Transform> startingPositions;

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
        if (Input.GetKeyDown(KeyCode.W))
            ScaleEntitiesStats();
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

        foreach (NPCController item in npcControllers)
        {
            for (int i = 0; i < statsToScale.Count; i++)
            {
                for (int j = 0; j < item.Stats.entityStats.Count; j++)
                {
                    if (statsToScale[i].StatType  == item.Stats.entityStats[j].StatType)
                    {
                        if (item.Stats.entityStats[j].Value <= 0) continue;

                        item.Stats.entityStats[j].AddModifier(new StatModifier(scalingFactor, statsToScale[i].StatType, StatModType.PercentAdd, this));
                    }
                }
            }
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