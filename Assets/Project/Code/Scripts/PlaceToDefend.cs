using DarkTonic.MasterAudio;
using UnityEngine;

public class PlaceToDefend : MonoBehaviour
{
    public delegate void HealthValueHandler(int value);
    public static event HealthValueHandler OnHealthValueChanged;

    public int health;
    public EntityTeam team;
    public GameObject passingThroughPortalVFX;

    private Animator myAnimator;
    private int maxHealth;

    [SoundGroup] public string passingThroughSFX;

    private void Start()
    {
        myAnimator = GetComponentInParent<Animator>();
        maxHealth = health * GameManager.Instance.placesToDefend.Count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ApplyDamageDebug();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ApplyDamageToBase(other);
    }

    private void ApplyDamageToBase(Collider other)
    {
        EntityStats stats = other.GetComponent<EntityStats>();
        NPCController npcController = other.GetComponent<NPCController>();

        if (stats != null && stats.EntityTeam != team && stats.EntityTeam != EntityTeam.NEUTRAL)
        {
            GameManager.Instance.DalvaLifePoints -= stats.DamageAppliedToThePlaceToDefend;

            UtilityClass.PlaySoundGroupImmediatly(passingThroughSFX, stats.transform);

            UIManager.Instance.SetDamageLoss(stats.DamageAppliedToThePlaceToDefend);

            OnHealthValueChanged?.Invoke(GameManager.Instance.DalvaLifePoints);

            CheckForLifeAnimation();

            if (!passingThroughPortalVFX.activeInHierarchy)
                passingThroughPortalVFX.SetActive(true);

            if (GameManager.Instance.DalvaLifePoints <= 0)
            {
                GameManager.Instance.Defeat();
            }

            if (npcController != null && npcController.IsABossWaveMember
                || npcController != null && GameManager.Instance.itsFinalWave)
            {
                GameManager.Instance.UpdateRemainingMonsterValue(-1);
            }

            Destroy(stats.gameObject);
        }
    }

    private void ApplyDamageDebug()
    {
        GameManager.Instance.DalvaLifePoints -= 1;

        UIManager.Instance.SetDamageLoss(-1);

        OnHealthValueChanged?.Invoke(GameManager.Instance.DalvaLifePoints);

        if (GameManager.Instance.DalvaLifePoints <= 0)
        {
            GameManager.Instance.Defeat();
        }
    }

    private void CheckForLifeAnimation()
    {
        if (GameManager.Instance.DalvaLifePoints > maxHealth * 2 / 3)
        {
            myAnimator.SetInteger("Status", 2);
            return;
        }
        else if (GameManager.Instance.DalvaLifePoints > maxHealth / 3)
        {
            myAnimator.SetInteger("Status", 1);
            return;
        }
        else myAnimator.SetInteger("Status", 0);
    }

}