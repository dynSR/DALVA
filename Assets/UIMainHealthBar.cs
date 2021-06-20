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
    public GameObject damageEffect;
    public Animator animator;

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

        //Change it with CancelInvoke
        StopAllCoroutines();

        ActivateDamageEffect();
        TriggerAnimator();

        // Set tree icon and heathbar color to match health percentage 100 / 4 > 100 - 75 - 50 - 25
        if (healthPercentage >= 75f)
        {
            healthBarFillImage.color = healthBarColors[0];
            //healthBarTreeImage.sprite = treeIcons[0];
        }
        else if (healthPercentage <= 75f && healthPercentage >= 50f)
        {
            healthBarFillImage.color = healthBarColors[1];
            //healthBarTreeImage.sprite = treeIcons[1];
        }
        else if (healthPercentage <= 50f && healthPercentage >= 25f)
        {
            healthBarFillImage.color = healthBarColors[2];
            //healthBarTreeImage.sprite = treeIcons[2];
        }
        else if (healthPercentage <= 25f && healthPercentage >= 0f)
        {
            healthBarFillImage.color = healthBarColors[3];
            //healthBarTreeImage.sprite = treeIcons[3];
            //Clignotement HB ?

            //Change it with InvokeRepeating
            //StartCoroutine(HealthBarBlinking(0.01f));
        }
    }

    //Change the function to something that blink overtime
    private IEnumerator HealthBarBlinking(float delay)
    {
        if (currentAmountOfHealth <= 0) yield break;

        healthBarFillImage.color = Color.white;

        yield return new WaitForSeconds(delay);

        healthBarFillImage.color = healthBarColors[3];
    }

    //Do the VFX
    void ActivateDamageEffect()
    {
        //if (!damageEffect.activeInHierarchy)
        //{
        //    damageEffect.SetActive(true);
        //}
        //else
        //{
        //    damageEffect.SetActive(false);
        //    damageEffect.SetActive(true);
        //}
    }

    //Do the animation
    private void TriggerAnimator()
    {
        //animator.SetTrigger("");
    }
}