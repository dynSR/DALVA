using TMPro;
using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    public int deathAmt = 0;
    public int initialRemainingAmtOfDeath = 3;
    private int remainingAmtOfDeath = 3;

    public float localTimer;
    public TextMeshProUGUI respawnTimerText;
    private bool canUpdateTimer = false;

    public TextMeshProUGUI remainingDeathsText;
    [Multiline] public string [ ] deathMessages;

    private Canvas UITreeLifeCanvas;

    private EntityStats PlayerStats => transform.parent.transform.parent.GetComponent<EntityStats>();
    private Animator MyAnimator => GetComponent<Animator>();


    private void Awake ()
    {
        remainingAmtOfDeath = initialRemainingAmtOfDeath;
        UITreeLifeCanvas = UIManager.Instance.treeLifeObject.GetComponent<Canvas>();
    }

    private void OnEnable ()
    {
        localTimer = PlayerStats.TimeToRespawn;
        canUpdateTimer = true;

        UpdateDeathDatas();
    }

    private void OnDisable ()
    {
        canUpdateTimer = false;
        ResetRemainingDeathDatas();
    }


    private void Update ()
    {
        UpdateRespawnTimer();
    }

    private void UpdateDeathDatas()
    {
        deathAmt += 1;

        //UpdateRemainingDeathsText();

        remainingAmtOfDeath -= 1;

        if (remainingAmtOfDeath == 0)
        {
            UITreeLifeCanvas.overrideSorting = true;
            UITreeLifeCanvas.sortingOrder = 2501;
        }

        MyAnimator.SetTrigger("UpdateText");
    }

    private void UpdateRespawnTimer()
    {
        if (localTimer > 0 && canUpdateTimer)
        {
            localTimer -= Time.deltaTime;

            respawnTimerText.SetText("Réapparition dans : " + localTimer.ToString("0"));

            if (localTimer <= 1)
                respawnTimerText.SetText("Réapparition dans : " + localTimer.ToString("0.0"));
        }
    }

    public void UpdateRemainingDeathsText()
    {
        Debug.Log("UpdateRemainingDeathsText");
        remainingDeathsText.SetText(deathMessages[ remainingAmtOfDeath ] + " " + PlayerStats.DamageAppliedToThePlaceToDefend + " points de vie.");
    }

    public void CheckRemainingDeathAmount()
    {
        if (remainingAmtOfDeath == 0)
        {
            GameManager.Instance.placesToDefend [ 0 ].ApplyDamageToBase(null, true);

            UITreeLifeCanvas.sortingOrder = 0;
            UITreeLifeCanvas.overrideSorting = false;
            //MyAnimator.SetTrigger("UpdateTreeLife");
        }
    }

    private void ResetRemainingDeathDatas()
    {
        if (remainingAmtOfDeath == 0)
        {
            remainingAmtOfDeath = initialRemainingAmtOfDeath;
            remainingDeathsText.SetText("Dans 3 morts, une pénalité de mort sera infligée: l’arbre perdra 3 points de vie.");
        }
    }
}
