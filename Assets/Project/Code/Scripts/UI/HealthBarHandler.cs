using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    private Image HealthBarFill => transform.GetChild(1).GetComponent<Image>();
    private CharacterStat stats /*=> transform.parent.GetComponentInParent<CharacterStat>()*/;

    private void Awake()
    {
        stats = transform.parent.GetComponentInParent<CharacterStat>();
    }

    private void OnEnable()
    {
        stats.OnHealthValueChanged += SetHealthBar; //function that updates healthbar
    }

    private void OnDisable()
    {
        stats.OnHealthValueChanged -= SetHealthBar; //function that updates healthbar
    }

    void SetHealthBar(float currentValue, float maxValue)
    {
        HealthBarFill.fillAmount = currentValue / maxValue;
    }
}
