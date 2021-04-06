using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private bool isAStele = false;
    [SerializeField] private Color allyColor;
    [SerializeField] private Color enemyColor;

    #region Ref
    private Image HealthBarFill => transform.GetChild(1).GetComponent<Image>();
    public Billboard Billboard => GetComponentInParent<Billboard>();
    private EntityStats stats;
    private SteleLogic stele;
    #endregion

    private void Awake()
    {
        SetHealthBarColor();

        if (isAStele) stele = transform.parent.GetComponentInParent<SteleLogic>();
        else stats = transform.parent.GetComponentInParent<EntityStats>();
    }

    private void OnEnable()
    {
        if (isAStele)
        {
            stele.OnHealthValueChanged += SetHealthBar;
            stele.OnActivation += SetHealthBarColor;
        }
        else stats.OnHealthValueChanged += SetHealthBar;
    }

    private void OnDisable()
    {
        if (isAStele)
        {
            stele.OnHealthValueChanged -= SetHealthBar;
            stele.OnActivation -= SetHealthBarColor;
        }
        else stats.OnHealthValueChanged += SetHealthBar;
    }

    void SetHealthBar(float currentValue, float maxValue)
    {
        HealthBarFill.fillAmount = currentValue / maxValue;

        //SetHealthBarColor();
    }

    void SetHealthBarColor()
    {
        //Modify Colors for correct team idk how
        if (Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.AllyPlayer
            || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.AllyMinion
            || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.AllyStele)
        {
            HealthBarFill.color = allyColor;
        }
        else if (Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.EnemyPlayer
            || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.EnemyMinion
            || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.EnemyStele
            || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.Monster)
        {
            HealthBarFill.color = enemyColor;
        }
        else
        {
            HealthBarFill.color = Color.yellow;
        }
    }
}
