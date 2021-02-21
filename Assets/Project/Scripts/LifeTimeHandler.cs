using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimeHandler : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.2f;
    [SerializeField] private bool hasAPredeterminedLifeTime;

    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    private void OnEnable()
    {
        if (!hasAPredeterminedLifeTime) return;
        
        StartCoroutine(DestroyAfterATime(LifeTime));
    }

    public IEnumerator DestroyAfterATime(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
    }
}
