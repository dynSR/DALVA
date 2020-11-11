using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public enum DamageType { Physical, Magic }
public class DamagePopUp : MonoBehaviour
{
    private DamageType damageType;

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float damagePopUpLifeTime = 0.5f;
    [SerializeField] private float moveYSpeed = 20f;
    [SerializeField] private float desappearSpeed = 3f;

    [Header("COLORS PARAMETERS")]
    [HideInInspector] public Color textColor;
    [SerializeField] private Color physicalDamageColor;
    [SerializeField] private Color magicDamageColor;

    private TextMeshPro DamageValueText => GetComponent<TextMeshPro>();

    public static DamagePopUp Create(Vector3 position, GameObject damagePopUpGameObject, float damageValueToGet, DamageType damageType)
    {
        GameObject damagePopUpInstance = Instantiate(damagePopUpGameObject, position, damagePopUpGameObject.transform.rotation);

        DamagePopUp damagePopUp = damagePopUpInstance.GetComponent<DamagePopUp>();

        damagePopUp.Setup(damageValueToGet, damageType);

        return damagePopUp;
    }

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterFading(damagePopUpLifeTime, desappearSpeed,  gameObject));
    }

    void Update()
    {
        MoveUp();
    }

    private void MoveUp()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
    }

    private void Setup(float damageAmount, DamageType damageType)
    {
        DamageValueText.SetText(damageAmount.ToString("0"));

        switch (damageType)
        {
            case DamageType.Physical:
                textColor = physicalDamageColor;
                DamageValueText.color = textColor;
                break;
            case DamageType.Magic:
                textColor = magicDamageColor;
                DamageValueText.color = textColor;
                break;
            default:
                break;
        }
    }

    private IEnumerator DestroyAfterFading(float time, float desappearSpeed, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(time);

        do
        {
            textColor.a -= desappearSpeed * Time.deltaTime;
            DamageValueText.color = textColor;

            yield return new WaitForEndOfFrame();
        } while (textColor.a > 0);

        Destroy(objectToDestroy);
    }
}
