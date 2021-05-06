using UnityEngine;

public abstract class SteleAmelioration : MonoBehaviour
{
    [SerializeField] private SteleLogic stele;
    [Multiline][SerializeField] private string upgradeDescriptionI;
    [Multiline] [SerializeField] private string upgradeDescriptionII;
    [Multiline] [SerializeField] private string upgradeDescriptionFinalEvolution;

    public string UpgradeDescriptionI { get => upgradeDescriptionI; }
    public string UpgradeDescriptionII { get => upgradeDescriptionII; }
    public string UpgradeDescriptionFinalEvolution { get => upgradeDescriptionFinalEvolution; }

    protected abstract void UpgradeEffect();
}