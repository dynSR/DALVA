using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private CharacterStat stats;
    [SerializeField] private Image healthBarFill;

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
        healthBarFill.fillAmount = currentValue / maxValue;
    }
}
