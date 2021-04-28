using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToDefend : MonoBehaviour
{
    public delegate void HealthValueHandler(int value);
    public static event HealthValueHandler OnHealthValueChanged;

    public int health;
    public List<EntityStats> targetFoundStats = new List<EntityStats>();
    public EntityTeam team;

    private void Start()
    {
        OnHealthValueChanged?.Invoke(health);
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats stats = other.GetComponent<EntityStats>();

        if(stats != null && stats.EntityTeam != team && stats.EntityTeam != EntityTeam.NEUTRAL)
        {
            health -= stats.DamageAppliedToThePlaceToDefend;

            OnHealthValueChanged?.Invoke(health);

            if (health <= 0)
            {
                GameManager.Instance.Defeat();
            }
        }
    }
}
