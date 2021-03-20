using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    [SerializeField] private List<Transform> visibleEntities = new List<Transform>();
    public List<Transform> VisibleEntities { get => visibleEntities; }

    #region Singleton
    public static FogOfWarManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public bool EntityIsContained(Transform source)
    {
        bool isContained = false;

        if (VisibleEntities.Count <= 0)
        {
            isContained = false;
        }
        else
        {
            for (int i = 0; i < VisibleEntities.Count; i++)
            {
                if (VisibleEntities[i].transform == source)
                {
                    isContained = true;
                }
                else
                    isContained = false;
            }
        }

        return isContained;
    }
}