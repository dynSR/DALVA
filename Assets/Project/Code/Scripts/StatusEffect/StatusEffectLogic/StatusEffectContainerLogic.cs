using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectContainerLogic : MonoBehaviour
{
    private TextMeshProUGUI StatusEffectDurationText => GetComponentInChildren<TextMeshProUGUI>();
    public StatusEffectLogic ContainedStatusEffectSystem { get; set; }

    private Image StatusEffectIconContained { get => GetComponent<Image>(); set => StatusEffectIconContained = value; }

    private float localTimer;

    private void Start()
    {
        StatusEffectIconContained.sprite = ContainedStatusEffectSystem.StatusEffect.StatusEffectIcon;
        localTimer = ContainedStatusEffectSystem.StatusEffect.StatusEffectDuration;
        SetUIForContainedStatusEffect();
    }

    private void Update()
    {
        CheckForStatusEffectTimerAndRemoveItFromUIWhenExpired(ContainedStatusEffectSystem);
    }

    void SetUIForContainedStatusEffect()
    {
        StatusEffectDurationText.text = localTimer.ToString("0");

        if (localTimer <= 1)
            StatusEffectDurationText.text = localTimer.ToString("0.0");
    }

    public void DestroyContainer()
    {
        Destroy(this.gameObject);
    }

    void CheckForStatusEffectTimerAndRemoveItFromUIWhenExpired(StatusEffectLogic statusEffect)
    {
        localTimer -= Time.deltaTime;
        SetUIForContainedStatusEffect();

        if (localTimer <= 0)
        {
            statusEffect.RemoveEffect();
            DestroyContainer();
        }
    }
}