using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    [SerializeField] private List<Transform> visibleEntities = new List<Transform>();

    private void OnEnable()
    {
        //+=
        //+=
    }

    private void OnDisable()
    {
        //-=
        //-=
    }

    private void AddVisibleEntity(Transform entity)
    {
        visibleEntities.Add(entity);
    }

    private void RemoveVisibleEntity(Transform entity)
    {
        visibleEntities.Remove(entity);
    }
}