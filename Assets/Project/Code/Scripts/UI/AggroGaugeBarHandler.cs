using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AggroGaugeBarHandler : MonoBehaviour
{
    [SerializeField] private Image aggroGaugeBarFill;
    [SerializeField] private Image aggroFeedbackImage;
    [SerializeField] private NPCController controller;

    private void OnEnable()
    {
        controller.Stats.OnDamageTaken += DisplayAggroImage;
        controller.OnExitingIdleState += DisplayAggroImage;
        controller.OnEnteringIdleState += HideAggroImage;
        controller.OnAggroValueChanged += SetAggroGauge;
    }

    private void OnDisable()
    {
        controller.Stats.OnDamageTaken -= DisplayAggroImage;
        controller.OnExitingIdleState -= DisplayAggroImage;
        controller.OnEnteringIdleState -= HideAggroImage;
        controller.OnAggroValueChanged -= SetAggroGauge;
    }

    private void Start() => SetAggroGauge(controller.AggroStep);

    void DisplayAggroImage()
    {
        Debug.Log("DISPLAY AGGRO GAUGE IMAGE");
        aggroFeedbackImage.gameObject.SetActive(true);
    }

    void HideAggroImage()
    {
        Debug.Log("HIDE AGGRO GAUGE IMAGE");
        aggroFeedbackImage.gameObject.SetActive(false);
    }

    void SetAggroGauge(float current)
    {
        aggroGaugeBarFill.fillAmount = current / (float)controller.MaxAgroStep;
    }
}