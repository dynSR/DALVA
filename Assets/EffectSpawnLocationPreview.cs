using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawnLocationPreview : MonoBehaviour
{
    private void Awake()
    {
        if (gameObject.activeInHierarchy) gameObject.SetActive(false);
    }
}
