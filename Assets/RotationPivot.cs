using System.Collections;
using UnityEngine;

public class RotationPivot : MonoBehaviour
{
    public float RotationSpeed;
    public float RotateVelocity;
    [SerializeField] private ParticleSystem particleEffect;
    [SerializeField] private float fadingSpeed;
    public bool ParticleStarColorAlphaEqualsZero => particleEffect.main.startColor.color.a <= 0f;

    //public void RotateAroundAPivot(Vector3 pointToRotateTowards)
    //{
    //    pointToRotateTowards = new Vector3(pointToRotateTowards.x, transform.position.y, pointToRotateTowards.z);
    //    transform.LookAt(pointToRotateTowards);
    //}

    public void HandleRotation(Transform transform, Vector3 target, float rotateVelocity, float rotationSpeed)
    {
        Quaternion rotationToLookAt = Quaternion.LookRotation(target - transform.position);

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref rotateVelocity,
            rotationSpeed * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    public void FadeParticleEffectOvertime(float targetValue)
    {
        if (!gameObject.activeInHierarchy && targetValue == 255f)
        {
            gameObject.SetActive(true);
        }

        Debug.Log("CALLING PARTICLE EFFECT FADE");

        float colorAlpha = particleEffect.main.startColor.color.a;

        Color colorToAssign =  Color.white;
        colorToAssign.a = colorAlpha;

        if (colorAlpha != targetValue)
        {
            //0 > 255
            if (colorAlpha > targetValue && targetValue == 0f)
            {
                colorAlpha -= Time.deltaTime * fadingSpeed;
            }
            //255 > 0
            else if (colorAlpha < targetValue && targetValue == 255f)
            {
                colorAlpha += Time.deltaTime * fadingSpeed;
            }
        }
        
        colorToAssign.a = colorAlpha;

        var main = particleEffect.main;
        main.startColor = colorToAssign;
    }
}