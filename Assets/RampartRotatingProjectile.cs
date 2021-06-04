using UnityEngine;

public class RampartRotatingProjectile : MonoBehaviour
{
    [SerializeField] private float projectileDamage = 25f;
    [SerializeField] private float delayBeforeReappearing = 3f;
    [SerializeField] private StatusEffect root;
    public GameObject explosionVFX;
    private float localTimer = 0f;
    private bool canUpdateTimer = false;

    void LateUpdate()
    {
        if (!transform.GetChild(0).gameObject.activeInHierarchy && localTimer == 0f) canUpdateTimer = true;

        if (canUpdateTimer)
        {
            localTimer += Time.deltaTime;
            Debug.Log("canUpdateTimer" + canUpdateTimer);

            if (localTimer >= delayBeforeReappearing)
            {
                ShowProjectile();
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

            InstantiateExplosionVFX(transform.position);
            HideProjectile();
        }
    }

    void ShowProjectile()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Collider>().enabled = true;
    }

    void HideProjectile()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }

    void InstantiateExplosionVFX(Vector3 pos)
    {
        Instantiate(explosionVFX, pos, explosionVFX.transform.rotation);
    }
}
