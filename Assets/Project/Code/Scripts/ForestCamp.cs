using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestCamp : MonoBehaviour
{
    [SerializeField] private List<NPCController> npcControllers;
    [SerializeField] private List<Transform> startingPositions;

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

    void SetNPCPositionAndRotation()
    {
        for (int i = 0; i < npcControllers.Count; i++)
        {
            npcControllers[i].StartingPosition = startingPositions[i];
            npcControllers[i].transform.LookAt(startingPositions[i]);
        }
    }

    void SetEveryNPCToAggressionState()
    {
        int index = 0;

        for (int i = 0; i < npcControllers.Count; i++)
        {
            Debug.Log("SetEveryNPCToAggressionState()");
            
            if(!npcControllers[i].IsInIdleState)
            {
                index = Array.IndexOf(npcControllers.ToArray(), npcControllers[i]);
                Debug.Log(index);
            }

            for (int j = npcControllers.Count - 1; j >= 0; j--)
            {
                //And for the others update their target, target being npc's source of damage that have been attacked
                if (npcControllers[j].IsInIdleState)
                {
                    Debug.Log(npcControllers[j].name);
                    Debug.Log(npcControllers[index].name);

                    if (!npcControllers[j].NPCInteractions.HasATarget)
                    {
                        npcControllers[j].Stats.SourceOfDamage = npcControllers[index].Stats.SourceOfDamage;
                        npcControllers[j].SetSourceOfDamageAsTarget();
                    }
                }
            }
        }
    }
}