using DarkTonic.MasterAudio;
using UnityEngine;

public class PlaceToDefend : MonoBehaviour
{
    public delegate void HealthValueHandler(int value);
    public static event HealthValueHandler OnHealthValueChanged;

    public int health;
    public EntityTeam team;
    public ParticleSystem passingThroughPortalVFX;
    public ParticleSystem changeStateVFX;

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

    public void ApplyDamageToBase(Collider other = null, bool applyDamageByPlayer = false)
    {
        if (other)
        {
            EntityStats stats = other.GetComponent<EntityStats>();
            NPCController npcController = other.GetComponent<NPCController>();

            if (stats != null && stats.EntityTeam != team && stats.EntityTeam != EntityTeam.NEUTRAL)
            {
                GameManager.Instance.DalvaLifePoints -= stats.DamageAppliedToThePlaceToDefend;

                UtilityClass.PlaySoundGroupImmediatly(passingThroughSFX, stats.transform);

                UIManager.Instance.SetDamageLoss(stats.DamageAppliedToThePlaceToDefend);

                passingThroughPortalVFX.Play();

                if (npcController != null && npcController.IsABossWaveMember
                    || npcController != null && GameManager.Instance.itsFinalWave)
                {
                    GameManager.Instance.UpdateRemainingMonsterValue(-1);
                }

                Destroy(stats.gameObject);
            }
        }
        else if (!other && applyDamageByPlayer)
        {
            int damageApplied = GameManager.Instance.damageAppliedByPlayerToTheTree;

            GameManager.Instance.DalvaLifePoints -= damageApplied;

            UtilityClass.PlaySoundGroupImmediatly(passingThroughSFX, GameManager.Instance.placesToDefend[0].transform);

            UIManager.Instance.SetDamageLoss(damageApplied);

            passingThroughPortalVFX.Play();
        }

        OnHealthValueChanged?.Invoke(GameManager.Instance.DalvaLifePoints);

        CheckForLifeAnimation();

        if (GameManager.Instance.DalvaLifePoints <= 0)
        {
            StartCoroutine(GameManager.Instance.Defeat(0.75f));
        }
    }

    private void ApplyDamageDebug()
    {
        GameManager.Instance.DalvaLifePoints -= 1;

        UIManager.Instance.SetDamageLoss(-1);

        OnHealthValueChanged?.Invoke(GameManager.Instance.DalvaLifePoints);

        CheckForLifeAnimation();

        if (GameManager.Instance.DalvaLifePoints <= 0)
        {
            StartCoroutine(GameManager.Instance.Defeat(0.75f));
        }
    }

    private void CheckForLifeAnimation()
    {
        if (GameManager.Instance.DalvaLifePoints == Mathf.Round(maxHealth * 3 / 4) ||
            GameManager.Instance.DalvaLifePoints == Mathf.Round(maxHealth * 2 / 4) ||
            GameManager.Instance.DalvaLifePoints == Mathf.Round(maxHealth / 4)) changeStateVFX.Play();

        if (GameManager.Instance.DalvaLifePoints > maxHealth * 3 / 4) return;
        if (GameManager.Instance.DalvaLifePoints > maxHealth * 2 / 4)
        {
            myAnimator.SetInteger("Status", 2);
            return;
        }
        else if (GameManager.Instance.DalvaLifePoints > maxHealth / 4)
        {
            myAnimator.SetInteger("Status", 1);
            return;
        }
        else myAnimator.SetInteger("Status", 0);
    }

}