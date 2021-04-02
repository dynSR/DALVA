using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteleState
{
    Inactive, 
    Active, 
    StandBy,
}

public class SteleLogic : MonoBehaviour
{
    [SerializeField] private SteleState steleState;

    public PlayerInteractions InteractingPlayer /*{ get; set; }*/;
    public bool IsInteractable { get; set; }
    private EntityDetection EntityDetection => GetComponent<EntityDetection>();

    void Start()
    {
        
    }

    void LateUpdate()
    {
        EnableEntityDetection();
    }


    private void EnableEntityDetection()
    {
        if (IsInteractable)
        {
            EntityDetection.enabled = true;
        }
        else
        {
            EntityDetection.enabled = false;
        }
    }
}