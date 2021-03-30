using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetimeValue = 0.2f;
    [SerializeField] private bool DestroyAfterTime = false;
    [SerializeField] private bool HideAfterTime = false;

    public float LifetimeValue { get => lifetimeValue; set => lifetimeValue = value; }

    private void OnEnable()
    {
        if(DestroyAfterTime)
            StartCoroutine(DestroyAfterATime(LifetimeValue));
        else if(HideAfterTime)
            StartCoroutine(HideAfterATime(LifetimeValue));
    }

    public IEnumerator DestroyAfterATime(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Destroy(gameObject);
    }

    public IEnumerator HideAfterATime(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        gameObject.SetActive(false);
    }
}