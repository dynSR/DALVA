using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityContainerLogic : MonoBehaviour
{
    [Header("CONTAINED ABILITY")]
    [SerializeField] private AbilityLogic containedAbility;
    [SerializeField] private Image containedAbilityIcon;

    [Header("KEY")]
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private TextMeshProUGUI keyText;

    [Header("COOLDOWN")]
    [SerializeField] private GameObject cooldownContainerGameObject;
    [SerializeField] private Image cooldownFiller;
    [SerializeField] private TextMeshProUGUI cooldownText;

    public KeyCode AbilityKey { get => abilityKey; set => abilityKey = value; }
    public AbilityLogic ContainedAbility { get => containedAbility; set => containedAbility = value; }

    void Start()
    {
        AbilitiesCooldownHandler.OnAbitilityUsed += UpdateAbilityCooldownUI;
        SetTheAbilityUI(this.ContainedAbility);
    }

    protected void SetTheAbilityUI(AbilityLogic containedAbility)
    {
        //Set ability key + Input UI
        abilityKey = containedAbility.Ability.AbilityKey;
        keyText.SetText(abilityKey.ToString());
    }

    private void UpdateAbilityCooldownUI(AbilityLogic containedAbility)
    {
        if (containedAbility == ContainedAbility)
        {
            //Activer le timer text et l'image filled
            cooldownContainerGameObject.SetActive(true);

            //Update le texte et l'image filled
            StartCoroutine(UpdateCooldownUIComponents(containedAbility));
        }
    }

    private IEnumerator UpdateCooldownUIComponents(AbilityLogic containedAbility)
    {
        float storedCooldown = containedAbility.Ability.AbilityCooldown;

        do
        {
            storedCooldown -= Time.deltaTime;

            //Mettre à jour le timer text
            cooldownText.SetText(storedCooldown.ToString("0.0"));
            //Update l'image filled
            cooldownFiller.fillAmount = storedCooldown / containedAbility.Ability.AbilityCooldown;

            yield return new WaitForEndOfFrame();
        } while (storedCooldown > 0);

        //Désactiver le timer text et l'image filled
        cooldownContainerGameObject.SetActive(false);
    }
}
