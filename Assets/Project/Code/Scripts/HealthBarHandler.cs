using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    private void OnEnable()
    {
        CharacterStat.OnHealthValueChanged += SetHealthBar; //function that updates healthbar
    }

    private void OnDisable()
    {
        CharacterStat.OnHealthValueChanged -= SetHealthBar; //function that updates healthbar
    }

    void SetHealthBar(float currentValue, float maxValue)
    {
        healthBarFill.fillAmount = currentValue / maxValue;
    }
}
