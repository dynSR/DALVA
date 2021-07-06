using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class UIMainHealthBar : MonoBehaviour
{
    [Header("UI COMPONENTS")]
    public Color[] healthBarColors;
    public Image healthBarFillImage;
    public Image healthBarTreeImage;
    public Animator parentAnimator;
    public Animator healthBarAnimator;

    [Header("DEBUG NUMERIC VALUES")]
    public int currentAmountOfHealth;
    public float healthPercentage;
    public int maxAmountOfLife; // debug

    [Header("SFX")]
    [SoundGroup] public string treeGettingCorruptedSFX;

    private bool yellowColorTransitionHasBeenHandled = false;
    private bool orangeColorTransitionHasBeenHandled = false;
    private bool redColorTransitionHasBeenHandled = false;

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
        }
        else if (healthPercentage <= 75f && healthPercentage >= 50f)
        {
            if (!yellowColorTransitionHasBeenHandled)
            {
                UtilityClass.PlaySoundGroupImmediatly(treeGettingCorruptedSFX, transform);
                yellowColorTransitionHasBeenHandled = true;
            }
            
            healthBarFillImage.color = healthBarColors[1];
            Debug.Log("YELLOW");
        }
        else if (healthPercentage <= 50f && healthPercentage >= 25f)
        {
            if (!orangeColorTransitionHasBeenHandled)
            {
                UtilityClass.PlaySoundGroupImmediatly(treeGettingCorruptedSFX, transform);
                orangeColorTransitionHasBeenHandled = true;
            }

            healthBarFillImage.color = healthBarColors[2];
            Debug.Log("ORANGE");
        }
        else if (healthPercentage <= 25f && healthPercentage >= 0f)
        {
            if (!redColorTransitionHasBeenHandled)
            {
                UtilityClass.PlaySoundGroupImmediatly(treeGettingCorruptedSFX, transform);
                redColorTransitionHasBeenHandled = true;
            }

            healthBarFillImage.color = healthBarColors[3];
            Debug.Log("RED");

            healthBarAnimator.SetTrigger("Blink");
        }
    }

    private void TriggerAnimator()
    {
        parentAnimator.SetTrigger("DamageTaken");
    }
}