using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectContainer : MonoBehaviour
{
    [SerializeField] private Image statusEffectContainedIcon;

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
}