using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionWaypointsManager : MonoBehaviour
{
    private Transform[] minionsGlobalWaypoints;

    public Transform[] MinionsGlobalWaypoints { get => minionsGlobalWaypoints; private set => minionsGlobalWaypoints = value; }

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
        MinionsGlobalWaypoints = new Transform[transform.childCount];

        for (int i = 0; i < MinionsGlobalWaypoints.Length; i++)
        {
            MinionsGlobalWaypoints[i] = transform.GetChild(i);
        }
    }
}
