using UnityEngine;

public interface IRegenerable
{
   void RegenerateHealth(Transform target, float regenerationThreshold);
}
