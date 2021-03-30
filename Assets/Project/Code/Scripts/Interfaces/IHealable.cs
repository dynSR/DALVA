using UnityEngine;

public interface ICurable
{
    void Heal(Transform target, float healAmount);
}
