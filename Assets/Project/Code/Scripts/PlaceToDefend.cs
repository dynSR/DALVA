using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToDefend : MonoBehaviour
{
    public delegate void HealthValueHandler(int value);
    public static event HealthValueHandler OnHealthValueChanged;

    public int health;
    public EntityTeam team;
    public GameObject passingThroughPortalVFX;

    private void Start()
    {
        OnHealthValueChanged?.Invoke(GameManager.Instance.DalvaLifePoints);
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats stats = other.GetComponent<EntityStats>();
        NPCController npcController = other.GetComponent<NPCController>();

        if (stats != null && stats.EntityTeam != team && stats.EntityTeam != EntityTeam.NEUTRAL)
        {
            GameManager.Instance.DalvaLifePoints -= stats.DamageAppliedToThePlaceToDefend;
            Destroy(stats.gameObject);

            OnHealthValueChanged?.Invoke(GameManager.Instance.DalvaLifePoints);

            if(!passingThroughPortalVFX.activeInHierarchy)
                passingThroughPortalVFX.SetActive(true);

            if (GameManager.Instance.DalvaLifePoints <= 0)
            {
                GameManager.Instance.Defeat();
            }

            if (npcController != null && npcController.IsABossWaveMember 
                || npcController != null && GameManager.Instance.itsFinalWave)
            {
                GameManager.Instance.UpdateRemainingMonsterValue(-1);
            }
        }
    }

}
