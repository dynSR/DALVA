using UnityEngine;

public class StatusEffectLogic : MonoBehaviour
{
    [SerializeField] private StatusEffect statusEffect;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in trigger");
            statusEffect.ApplyEffect(other.transform);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is out of trigger");
        }
    }
}