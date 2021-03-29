using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestCamp : MonoBehaviour
{
    [SerializeField] private List<NPCController> npcControllers;
    [SerializeField] private List<Transform> startingPositions;

    void Awake() => SetNPCPositionAndRotation();

    void SetNPCPositionAndRotation()
    {
        for (int i = 0; i < npcControllers.Count; i++)
        {
            npcControllers[i].StartingPosition = startingPositions[i];
            npcControllers[i].transform.LookAt(startingPositions[i]);
        }
    }
}
