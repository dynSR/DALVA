using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMainHealthBar : MonoBehaviour
{
    [Header("UI COMPONENTS")]
    public Color[] healthBarColors;
    public Sprite[] treeIcons;
    public Image healthBarFillImage;
    public Image healthBarTreeImage;
    public Animator parentAnimator;
    public Animator healthBarAnimator;

    [Header("DEBUG NUMERIC VALUES")]
    public int currentAmountOfHealth;
    public float healthPercentage;
    public int maxAmountOfLife; // debug

    #region Singleton
    public static UIMainHealthBar Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private void OnEnable()
    {
        PlaceToDefend.OnHealthValueChanged += SetHealthBarParameters;
    }

    private void OnDisable()
    {
        PlaceToDefend.OnHealthValueChanged -= SetHealthBarParameters;
    }

    public void SetHealthBarParameters(int healthValue)
    {
        healthBarFillImage.fillAmount = (float)healthValue / (float)maxAmountOfLife;
        healthPercentage = ((float)healthValue / (float)maxAmountOfLife) * 100f;

        if (healthPercentage < 100) TriggerAnimator();

        // Set tree icon and heathbar color to match health percentage 100 / 4 > 100 - 75 - 50 - 25
        if (healthPercentage <= 100 && healthPercentage >= 75f)
        {
            healthBarFillImage.color = healthBarColors[0];
            Debug.Log("GREEN");
            //healthBarTreeImage.sprite = treeIcons[0];
        }
        else if (healthPercentage <= 75f && healthPercentage >= 50f)
        {
            healthBarFillImage.color = healthBarColors[1];
            Debug.Log("YELLOW");
            //healthBarTreeImage.sprite = treeIcons[1];
        }
        else if (healthPercentage <= 50f && healthPercentage >= 25f)
        {
            healthBarFillImage.color = healthBarColors[2];
            Debug.Log("ORANGE");
            //healthBarTreeImage.sprite = treeIcons[2];
        }
        else if (healthPercentage <= 25f && healthPercentage >= 0f)
        {
            healthBarFillImage.color = healthBarColors[3];
            Debug.Log("RED");
            //healthBarTreeImage.sprite = treeIcons[3];

            healthBarAnimator.SetTrigger("Blink");
        }
    }

    private void TriggerAnimator()
    {
        parentAnimator.SetTrigger("DamageTaken");
    }
}