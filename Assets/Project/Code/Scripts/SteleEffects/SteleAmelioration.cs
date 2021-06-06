using UnityEngine;

public abstract class SteleAmelioration : MonoBehaviour
{
    [SerializeField] private SteleLogic stele = null;
    [SerializeField] private string steleEffectName;
    [SerializeField] private Sprite steleIconImage;
    [Multiline][SerializeField] private string upgradeDescriptionI;
    [Multiline] [SerializeField] private string upgradeDescriptionII;
    [Multiline] [SerializeField] private string upgradeDescriptionFinalEvolution;
    public GameObject renderers;

    public string UpgradeDescriptionI { get => upgradeDescriptionI; }
    public string UpgradeDescriptionII { get => upgradeDescriptionII; }
    public string UpgradeDescriptionFinalEvolution { get => upgradeDescriptionFinalEvolution; }
    public SteleLogic Stele { get => stele; set => stele = value; }
    public string SteleEffectName { get => steleEffectName; set => steleEffectName = value; }
    public Sprite SteleIconImage { get => steleIconImage; set => steleIconImage = value; }

    private Animator myAnimator;
    public Animator MyAnimator { get => myAnimator; set => myAnimator = value; }

    public void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    public abstract void UpgradeEffect();
}