using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionWaypointsManager : MonoBehaviour
{
    private List<Transform> minionsGlobalWaypoints = new List<Transform>();

    public List<Transform> MinionsGlobalWaypoints { get => minionsGlobalWaypoints; private set => minionsGlobalWaypoints = value; }

    #region Singleton
    public static MinionWaypointsManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            GetChilds();
        }
    }
    #endregion

    private void GetChilds()
    {
        foreach (Transform child in transform)
        {
            MinionsGlobalWaypoints.Add(child);
        }
    }
}
