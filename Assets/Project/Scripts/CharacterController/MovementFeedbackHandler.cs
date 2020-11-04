using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFeedbackHandler : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.2f;

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterATime());
    }

    IEnumerator DestroyAfterATime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
