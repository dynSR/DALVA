using UnityEngine;

public class RampartRotatingProjectile : MonoBehaviour
{
    [SerializeField] private float projectileDamage = 25f;
    [SerializeField] private float delayBeforeReappearing = 3f;
    [SerializeField] private StatusEffect root;
    private float localTimer = 0f;
    private bool canUpdateTimer = false;

    void LateUpdate()
    {
        if (!gameObject.activeInHierarchy && localTimer == 0f) canUpdateTimer = true;

        if (canUpdateTimer)
        {
            localTimer += Time.deltaTime;

            if (localTimer >= delayBeforeReappearing)
            {
                gameObject.SetActive(true);
                localTimer = 0;
                canUpdateTimer = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats otherStats = other.GetComponent<EntityStats>();

        if (otherStats != null && !otherStats.IsDead && otherStats.EntityTeam == EntityTeam.HULRYCK)
        {
            otherStats.TakeDamage(null, 0, 0, 0, projectileDamage, 0, 0, 0, 0);
            root.ApplyEffect(otherStats.transform);
            gameObject.SetActive(false);
        }
    }
}
