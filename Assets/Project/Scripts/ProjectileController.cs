using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    private Transform projectileEmiter;

    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private float projectileDamage;
    [SerializeField] private GameObject onHitEffect;

    private Rigidbody rb;
    public Transform ProjectileEmiter { get => projectileEmiter; set => projectileEmiter = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(DestroyProjectileAfterTime());
    }

    void FixedUpdate()
    {
        rb.MovePosition(transform.position += transform.forward * projectileSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //InstantiateHitEffect(onHitEffect);
        Destroy(gameObject);
    }

    void InstantiateHitEffect(GameObject objToInstantiate)
    {
        Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }

    IEnumerator DestroyProjectileAfterTime()
    {
        yield return new WaitForSeconds(projectileLifeTime);
        //InstantiateHitEffect(onHitEffect);
        Destroy(gameObject);
    }
}
