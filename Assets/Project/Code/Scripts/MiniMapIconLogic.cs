using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIconLogic : MonoBehaviour
{
    private EntityStats Stats => GetComponentInParent<EntityStats>();

    private void OnEnable()
    {
        if (Stats == null) return;

        Stats.OnEntityDeath += HideMiniMapIcon;
        Stats.OnEntityRespawn += DisplayMiniMapIcon;
    }

    private void OnDisable()
    {
        if (Stats == null) return;

        Stats.OnEntityDeath -= HideMiniMapIcon;
        Stats.OnEntityRespawn -= DisplayMiniMapIcon;
    }

    void DisplayMiniMapIcon()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void HideMiniMapIcon()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}