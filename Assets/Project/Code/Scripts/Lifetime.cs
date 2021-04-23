using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetimeValue = 0.2f;
    [SerializeField] private bool destroyAfterTime = false;
    [SerializeField] private bool hideAfterTime = false;

    public float LifetimeValue { get => lifetimeValue; set => lifetimeValue = value; }
    public bool HideAfterTime { get => hideAfterTime; set => hideAfterTime = value; }
    public bool DestroyAfterTime { get => destroyAfterTime; set => destroyAfterTime = value; }

    #region Refs
    private ProjectileLogic projectile;
    #endregion

    private void Awake()
    {
        if(GetComponent<ProjectileLogic>() != null)
            projectile = GetComponent<ProjectileLogic>();
    }

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

        if(projectile != null)
        {
            ImmediatlyHideObject();
            projectile.SpawnDestructionEffect(projectile.DestructionVFX);
            Debug.Log("Spwaned destruction VFX");
        }

        Destroy(gameObject);
    }

    public IEnumerator HideAfterATime(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        gameObject.SetActive(false);
    }

    private void ImmediatlyHideObject()
    {
        gameObject.SetActive(false);
    }
}