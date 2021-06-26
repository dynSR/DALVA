using UnityEngine;

public class AnimatedComponentHandler : MonoBehaviour
{
    private Animator AnimatorComponent => GetComponent<Animator>();
    public ParticleSystem[] particleEffects;

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (GameManager.Instance.GameIsInPause() && AnimatorComponent.speed == 1)
            {
                AnimatorComponent.speed = 0;
                ToggleUsedParticleEffects(false, true);
            }
            else if (!GameManager.Instance.GameIsInPause() && AnimatorComponent.speed == 0)
            {
                AnimatorComponent.speed = 1;
                ToggleUsedParticleEffects(true, false);
            }
        }
    }

    void ToggleUsedParticleEffects (bool setToOn = false, bool setToOff = false)
    {
        if (particleEffects.Length > 0)
        {
            if (setToOn)
            {
                foreach (ParticleSystem particles in particleEffects)
                {
                    particles.Play();
                }
            }
            else if (setToOff)
            {
                foreach (ParticleSystem particles in particleEffects)
                {
                    particles.Stop();
                }
            }
        }
        
    }
}