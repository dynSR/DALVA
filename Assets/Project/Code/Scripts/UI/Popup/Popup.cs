using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviourPun
{
    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float popUpLifeTime = 0.5f;
    [SerializeField] private float moveXSpeed = 3f;
    [SerializeField] private float moveYSpeed = 3f;
    [SerializeField] private float desappearSpeed = 3f;

    [Header("COLORS")]
    [SerializeField] private Color physicalDamageColor;
    [SerializeField] private Color magicalDamageColor;
    [SerializeField] private Color criticalColor;
    [SerializeField] private Color healColor;
    [SerializeField] private Color shieldColor;
    [SerializeField] private Color ressourcesColor;

    [Header("ICONS")]
    [SerializeField] private Sprite physicalDamageIcon;
    [SerializeField] private Sprite magicalDamageIcon;

    private Color textColor;
    Vector3 initRot;

    private TextMeshPro ValueText => GetComponent<TextMeshPro>();
    private SpriteRenderer Icon => GetComponentInChildren<SpriteRenderer>();

    public Sprite MagicalDamageIcon { get => magicalDamageIcon; }
    public Sprite PhysicalDamageIcon { get => physicalDamageIcon; }

    public static Popup Create(Vector3 position, GameObject popupGO, float value, StatType stat, Sprite icon, bool isCritical = false, bool targetIsInvulnerable = false)
    {
        GameObject popupInstance = Instantiate(popupGO, position, popupGO.transform.rotation);

        Popup popup = popupInstance.GetComponent<Popup>();

        popup.Setup(value, stat, icon, isCritical, targetIsInvulnerable);

        return popup;
    }

    private void OnEnable()
    {
        StartCoroutine(FadeAndDestroy(popUpLifeTime, desappearSpeed, gameObject));
        initRot = transform.eulerAngles;
    }

    void LateUpdate()
    {
        MoveUp();
    }

    private void FreezeLocalRotation()
    {
        transform.eulerAngles = initRot;
    }

    private void MoveUp()
    {
        FreezeLocalRotation();
        transform.position += new Vector3(moveXSpeed, moveYSpeed) * Time.deltaTime;
    }

    private void Setup(float value, StatType stat, Sprite icon, bool isCritical = false, bool targetIsInvulnerable = false)
    {
        //ChangeRectScale(new Vector3(1.5f, 1.5f, 1.5f));

        if (targetIsInvulnerable)
        {
            ValueText.SetText(value.ToString("INVULNERABLE"));
            return;
        }

        if (isCritical) ValueText.SetText(value.ToString("0") + " !");
        //else if (itsAHeal)
        //{
        //    //ChangeRectScale(new Vector3(2.5f, 2.5f, 2.5f));
        //    textColor = healColor;
        //    ValueText.SetText("+ " + value.ToString("0"));
        //}
        else ValueText.SetText(value.ToString("0"));

        switch (stat)
        {
            case StatType.PhysicalPower:
                if (isCritical) textColor = criticalColor;
                else textColor = physicalDamageColor;
                break;
            case StatType.MagicalPower:
                textColor = magicalDamageColor;
                break;
            case StatType.RessourcesGiven:
                textColor = ressourcesColor;
                ValueText.SetText("+ " + value.ToString("0"));
                break;
            case StatType.Health:
                textColor = healColor;
                ValueText.SetText("+ " + value.ToString("0"));
                break;
            case StatType.Shield:
                textColor = shieldColor;
                ValueText.SetText("+ " + value.ToString("0"));
                break;
        }

        ValueText.color = textColor;

        //Commented in case you want to use icon to represent damage type uncomment and use it
        //if (icon != null)
        //    Icon.sprite = icon;
    }

    private void ChangeRectScale(Vector3 newValue)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(newValue.x, newValue.y, newValue.z);
    }

    private IEnumerator FadeAndDestroy(float delay, float fadingSpeed, GameObject popup)
    {
        yield return new WaitForSeconds(delay);

        do
        {
            textColor.a -= fadingSpeed * Time.deltaTime;

            //Icon.color = textColor;
            ValueText.color = textColor;

            yield return new WaitForEndOfFrame();
        } while (textColor.a > 0);
    }
}