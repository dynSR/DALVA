using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] private bool isAStele = false;
    [SerializeField] private Color allyColor;
    [SerializeField] private Color enemyColor;
    [SerializeField] private float healthBarBlinkDuration = 0f;

    #region Ref
    private Image HealthBarFill => transform.GetChild(1).GetComponent<Image>();
    public Billboard Billboard => GetComponentInParent<Billboard>();
    private EntityStats stats;
    private SteleLogic stele;
    #endregion

    private void Awake()
    {
        if (isAStele) stele = transform.parent.GetComponentInParent<SteleLogic>();
        else stats = transform.parent.GetComponentInParent<EntityStats>();
    }

    private void OnEnable()
    {
        if (isAStele)
        {
            stele.OnHealthValueChanged += SetHealthBar;
        }
        else
        {
            stats.OnHealthValueChanged += SetHealthBar;
            stats.OnDamageTaken += SetHalthBarColor;
        }
    }

    private void OnDisable()
    {
        if (isAStele)
        {
            stele.OnHealthValueChanged -= SetHealthBar;
        }
        else
        {
            stats.OnHealthValueChanged -= SetHealthBar;
            stats.OnDamageTaken -= SetHalthBarColor;
        }
    }

    void SetHealthBar(float currentValue, float maxValue)
    {
        HealthBarFill.fillAmount = currentValue / maxValue;
    }

    void SetHalthBarColor()
    {
        StartCoroutine(HealthBarBlinking(healthBarBlinkDuration));
    }

    private IEnumerator HealthBarBlinking(float delay)
    {
        Color currentColor = HealthBarFill.color;

        HealthBarFill.color = Color.white;

        yield return new WaitForSeconds(delay);

        HealthBarFill.color = currentColor;


        ////Modify Colors for correct team idk how
        //if (Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.AllyPlayer
        //    || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.AllyMinion
        //    || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.AllyStele)
        //{
        //    HealthBarFill.color = allyColor;
        //}
        //else if (Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.EnemyPlayer
        //    || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.EnemyMinion
        //    || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.EnemyStele
        //    || Billboard.EntityDetection.TypeOfEntity == TypeOfEntity.Monster)
        //{
        //    HealthBarFill.color = enemyColor;
        //}
        //else
        //{
        //    HealthBarFill.color = Color.yellow;
        //}
    }
}