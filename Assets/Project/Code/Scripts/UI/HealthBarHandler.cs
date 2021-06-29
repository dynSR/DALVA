using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [Header("PROPERTIES")]
    [SerializeField] private bool isAStele = false;
    [SerializeField] private Color defaultColor;
    [SerializeField] private GameObject blinkImage;
    [SerializeField] private bool isAPlayerHealthBar = false;

    [Header("HEALTHBAR UI")]
    [SerializeField] private bool healthBarCanBlink = true;
    [SerializeField] private float healthBarBlinkDuration = 0f;
    [SerializeField] private Image healthBarFill;

    [Header("SHIELDBAR UI")]
    [SerializeField] private Image shieldBarFill;

    [Header("REFERENCES")]
    [SerializeField] private TextMeshProUGUI healthValueText;
    [SerializeField] private TextMeshProUGUI regenerationValueText;
    [SerializeField] private EntityStats stats;
    [SerializeField] private SteleLogic stele;
    private Animator MyAnimator => GetComponent<Animator>();

    private void Awake()
    {
        if (stats == null || isAStele && stele == null)
            Debug.LogError("You need to reference root parent's SteleLogic or EntityStats script. ", transform);

        //if (healthValueText != null)
        //    SetHealthBar(stats.GetStat(StatType.Health).Value, stats.GetStat(StatType.Health).MaxValue);

        ResetHealthBarColor();

        SetHealthRegenerationValue(stats);
    }

    private void OnEnable()
    {
        //if (isAStele) stele.OnHealthValueChanged += SetHealthBar;

        if (stats != null)
        {
            stats.OnHealthValueChanged += SetHealthBar;
            stats.OnEntityRespawn += ResetHealthBarColor;
            stats.OnDamageTaken += SetHealthBarColor;
            stats.OnShieldValueChanged += SetShieldBar;
            stats.OnStatsValueChanged += SetHealthRegenerationValue;
        }
    }

    private void OnDisable()
    {
        //if (isAStele) stele.OnHealthValueChanged -= SetHealthBar;

        if (stats != null)
        {
            stats.OnHealthValueChanged -= SetHealthBar;
            stats.OnEntityRespawn -= ResetHealthBarColor;
            stats.OnDamageTaken -= SetHealthBarColor;
            stats.OnShieldValueChanged -= SetShieldBar;
            stats.OnStatsValueChanged -= SetHealthRegenerationValue;
        }
    }

    void SetHealthBar(float currentValue, float maxValue)
    {
        healthBarFill.fillAmount = currentValue / maxValue;

        if (healthValueText != null)
            healthValueText.text = currentValue.ToString("0") + " / " + maxValue.ToString("0");

        //Under 20% HP
        if (isAPlayerHealthBar)
        {
            if (stats.HealthPercentage <= 0.2f)
            {
                blinkImage.SetActive(true);
                MyAnimator.SetBool("Blink", true);
            }
            else if (stats.HealthPercentage <= 0)
            {
                MyAnimator.SetBool("Blink", false);
                blinkImage.SetActive(false);
            }
            else
            {
                MyAnimator.SetBool("Blink", false);
                blinkImage.SetActive(false);
            }
        }
    }

    void SetShieldBar(float currentValue, float maxValue)
    {
        if(shieldBarFill != null)
            shieldBarFill.fillAmount = currentValue / maxValue;

        if (shieldBarFill.fillAmount >= 0.5f) shieldBarFill.fillAmount = 0.5f;
    }

    void SetHealthRegenerationValue(EntityStats stats)
    {
        if (regenerationValueText != null)
        {
            regenerationValueText.gameObject.SetActive(true);
            regenerationValueText.text = "Régénération " + " + " + stats.GetStat(StatType.HealthRegeneration).Value.ToString("0.0");
        }
    }

    void ResetHealthBarColor()
    {
        healthBarFill.color = defaultColor;
    }

    void SetHealthBarColor()
    {
        if (!healthBarCanBlink) return;
        
        StartCoroutine(HealthBarBlinking(healthBarBlinkDuration));
    }

    private IEnumerator HealthBarBlinking(float delay)
    {
        if (stats.IsDead) yield break;

        healthBarFill.color = Color.white;

        yield return new WaitForSeconds(delay);

        healthBarFill.color = defaultColor;
    }
}