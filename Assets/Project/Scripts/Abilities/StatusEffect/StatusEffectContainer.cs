using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectContainer : MonoBehaviour
{
    private TextMeshProUGUI StatusEffectDurationText => GetComponentInChildren<TextMeshProUGUI>();
    private Image StatusEffectIconContained { get => GetComponent<Image>(); set => StatusEffectIconContained.sprite = StatusEffect.StatusEffectIcon; } 

    [SerializeField] private StatusEffect statusEffect;
    public StatusEffect StatusEffect { get => statusEffect; set => statusEffect = value; }

    float localTimer;

    private void Start()
    {
        localTimer = StatusEffect.StatusEffectDuration;
        SetStatusEffectDurationTextAndTheDuration();
    }

    private void Update()
    {
        CheckForStatusEffectTimerAndRemoveItFromUIWhenExpired();
    }

    void SetStatusEffectDurationTextAndTheDuration()
    {
        int minutes = Mathf.FloorToInt(localTimer / 60F);
        int seconds = Mathf.FloorToInt(localTimer - minutes * 60);

        string timer = string.Format("{0:0}:{1:00}", minutes, seconds);
        StatusEffectDurationText.text = timer;
    }

    public void DestroyContainer()
    {
        Destroy(gameObject);
    }

    void CheckForStatusEffectTimerAndRemoveItFromUIWhenExpired()
    {
        localTimer -= Time.deltaTime;
        SetStatusEffectDurationTextAndTheDuration();

        if (localTimer <= 0)
        {
            DestroyContainer();
        }
    }
}
