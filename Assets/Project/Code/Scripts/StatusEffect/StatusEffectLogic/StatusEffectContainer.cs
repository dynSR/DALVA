using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectContainer : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Image statusEffectContainedIcon;
    [SerializeField] private Image timerImage;

    [Header("TIMER IMAGE COLOR")]
    [SerializeField] private Color harmlessEffectColor;
    [SerializeField] private Color harmfulEffectColor;

    private TextMeshProUGUI StatusEffectDurationText => GetComponentInChildren<TextMeshProUGUI>();
    public StatusEffect ContainedStatusEffect { get; set; }
    public StatusEffectHandler StatusEffectHandler { get; set; }

    private float localTimer;

    private void Start() => SetContainer();

    private void Update() => RemoveExpiredStatusEffectFromUI();

    void SetContainer()
    {
        SetIcon();
        ResetTimer();
        SetTimerImageColor();
    }

    public void DestroyContainer()
    {
        Destroy(this.gameObject);
    }

    private void SetIcon()
    {
        statusEffectContainedIcon.sprite = ContainedStatusEffect.StatusEffectIcon;
    }

    public void ResetTimer()
    {
        localTimer = ContainedStatusEffect.StatusEffectDuration;
        StatusEffectDurationText.text = localTimer.ToString("0");
    }

    void UpdateTimer()
    {
        localTimer -= Time.deltaTime;

        StatusEffectDurationText.text = localTimer.ToString("0");

        UpdateTimerImageFillAmount(localTimer, ContainedStatusEffect.StatusEffectDuration);

        if (localTimer <= 1)
            StatusEffectDurationText.text = localTimer.ToString("0.0");
    }

    void RemoveExpiredStatusEffectFromUI()
    {
        UpdateTimer();

        if (localTimer <= 0)
        {
            DestroyContainer();
        }
    }

    void SetTimerImageColor()
    {
        if (ContainedStatusEffect.Type == StatusEffectType.Harmless) timerImage.color = harmlessEffectColor;
        else if (ContainedStatusEffect.Type == StatusEffectType.Harmful) timerImage.color = harmfulEffectColor;
    }

    void UpdateTimerImageFillAmount(float current, float min)
    {
        timerImage.fillAmount = current / min;
    }
}