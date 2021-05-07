using UnityEngine;

public abstract class SteleAmelioration : MonoBehaviour
{
    [SerializeField] private SteleLogic stele;
    [SerializeField] private string steleEffectName;
    [Multiline][SerializeField] private string upgradeDescriptionI;
    [Multiline] [SerializeField] private string upgradeDescriptionII;
    [Multiline] [SerializeField] private string upgradeDescriptionFinalEvolutionI;
    [Multiline] [SerializeField] private string upgradeDescriptionFinalEvolutionII;

    public string UpgradeDescriptionI { get => upgradeDescriptionI; }
    public string UpgradeDescriptionII { get => upgradeDescriptionII; }
    public string UpgradeDescriptionFinalEvolutionI { get => upgradeDescriptionFinalEvolutionI; }
    public string UpgradeDescriptionFinalEvolutionII { get => upgradeDescriptionFinalEvolutionII; }
    public SteleLogic Stele { get => stele; set => stele = value; }
    public string SteleEffectName { get => steleEffectName; set => steleEffectName = value; }

    public abstract void UpgradeEffect();
}