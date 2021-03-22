using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectContainerLogic : MonoBehaviour
{
    private TextMeshProUGUI StatusEffectDurationText => GetComponentInChildren<TextMeshProUGUI>();
    public StatusEffectLogic ContainedStatusEffectSystem { get; set; }

    private Image StatusEffectIconContained { get => GetComponent<Image>(); set => StatusEffectIconContained = value; }

    private float localTimer;

    private void Start() => SetContainer();

    private void Update() => RemoveExpiredStatusEffectFromUI();

    void SetContainer()
    {
        SetContainerIcon();
        ResetTimer();
    }

    public void DestroyContainer()
    {
        Destroy(this.gameObject);
    }

    private void SetContainerIcon()
    {
        StatusEffectIconContained.sprite = ContainedStatusEffectSystem.StatusEffect.StatusEffectIcon;
    }

    public void ResetTimer()
    {
        localTimer = ContainedStatusEffectSystem.StatusEffect.StatusEffectDuration;
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
            ContainedStatusEffectSystem.GetTargetStatusEffectHandler(ContainedStatusEffectSystem.Target).RemoveEffectFromStatusEffectHandler(ContainedStatusEffectSystem);
            DestroyContainer();
        }
    }
}