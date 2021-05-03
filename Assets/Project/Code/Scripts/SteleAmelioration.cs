using UnityEngine;

public abstract class SteleAmelioration : MonoBehaviour
{
    [SerializeField] private SteleLogic stele;

    protected abstract void UpgradeEffect();
}