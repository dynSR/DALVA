using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetimeValue = 0.2f;

    public float LifetimeValue { get => lifetimeValue; set => lifetimeValue = value; }

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterATime(LifetimeValue));
    }

    public IEnumerator DestroyAfterATime(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
    }
}
