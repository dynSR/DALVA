using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectContainer : MonoBehaviour
{
    private TextMeshProUGUI StatusEffectDurationText => GetComponentInChildren<TextMeshProUGUI>();
    [SerializeField] private StatusEffect containedStatusEffect;
    public StatusEffect ContainedStatusEffect { get => containedStatusEffect; set => containedStatusEffect = value; }

    private Image StatusEffectIconContained { get => GetComponent<Image>(); set => StatusEffectIconContained = value; }

    private float localTimer;

    private void Start()
    {
        StatusEffectIconContained.sprite = ContainedStatusEffect.StatusEffectIcon;
        localTimer = ContainedStatusEffect.StatusEffectDuration;
        SetUIForContainedStatusEffect();
    }

    private void Update()
    {
        CheckForStatusEffectTimerAndRemoveItFromUIWhenExpired();
    }

    void SetUIForContainedStatusEffect()
    {
        //int minutes = Mathf.FloorToInt(localTimer / 60F);
        //int seconds = Mathf.FloorToInt(localTimer - minutes * 60);

        //string timer = string.Format("{0:0}:{1:00}", minutes, seconds);

        StatusEffectDurationText.text = localTimer.ToString("0");
    }

    public void DestroyContainer()
    {
        Destroy(gameObject);
    }

    void CheckForStatusEffectTimerAndRemoveItFromUIWhenExpired()
    {
        localTimer -= Time.deltaTime;
        SetUIForContainedStatusEffect();

        if (localTimer <= 0)
        {
            DestroyContainer();
        }
    }
}
