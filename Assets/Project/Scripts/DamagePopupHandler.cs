using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public enum DamageType { Physical, Magic, Critical }

public class DamagePopupHandler : MonoBehaviour
{
    private DamageType damageType;

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float damagePopUpLifeTime = 0.5f;
    [SerializeField] private float moveXSpeed = 20f;
    [SerializeField] private float moveYSpeed = 20f;
    [SerializeField] private float desappearSpeed = 3f;

    [Header("COLORS PARAMETERS")]
    private Color textColor;
    [SerializeField] private Color physicalDamageColor;
    [SerializeField] private Color magicDamageColor;
    [SerializeField] private Color criticalDamageColor;

    private TextMeshPro DamageValueText => GetComponent<TextMeshPro>();

    public static DamagePopupHandler Create(Vector3 position, GameObject damagePopUpGameObject, float damageValueToGet, DamageType damageType)
    {
        GameObject damagePopUpInstance = Instantiate(damagePopUpGameObject, position, damagePopUpGameObject.transform.rotation);

        DamagePopupHandler damagePopUp = damagePopUpInstance.GetComponent<DamagePopupHandler>();

        damagePopUp.Setup(damageValueToGet, damageType);

        return damagePopUp;
    }

    private void OnEnable()
    {
        StartCoroutine(FadeAndDestroy(damagePopUpLifeTime, desappearSpeed, gameObject));
    }

    void LateUpdate()
    {
        MoveUp();
    }

    private void MoveUp()
    {
        transform.LookAt(UtilityClass.GetMainCameraPosition());
        transform.Rotate(0, 180, 0);
        transform.position += new Vector3(moveXSpeed, moveYSpeed) * Time.deltaTime;
    }

    private void Setup(float damageAmount, DamageType damageType)
    {
        if(damageType != DamageType.Critical)
            DamageValueText.SetText(damageAmount.ToString("0"));
        else
            DamageValueText.SetText(damageAmount.ToString("0") + " !");

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
            case DamageType.Critical:
                textColor = criticalDamageColor;
                DamageValueText.color = textColor;
                break;
            default:
                break;
        }
    }

    private IEnumerator FadeAndDestroy(float delay, float fadingSpeed, GameObject popup)
    {
        yield return new WaitForSeconds(delay);

        do
        {
            textColor.a -= fadingSpeed * Time.deltaTime;
            DamageValueText.color = textColor;

            yield return new WaitForEndOfFrame();
        } while (textColor.a > 0);

        Destroy(popup);
    }
}
