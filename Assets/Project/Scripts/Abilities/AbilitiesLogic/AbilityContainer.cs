using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityContainer : MonoBehaviour
{
    [SerializeField] private Ability containedAbility;
    [SerializeField] private Image containedAbilityIcon;

    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private TextMeshProUGUI keyText;

    [SerializeField] private GameObject cooldownContainerGameObject;
    [SerializeField] private Image cooldownFiller;
    [SerializeField] private TextMeshProUGUI cooldownText;

    public KeyCode AbilityKey { get => abilityKey; set => abilityKey = value; }
    public Ability ContainedAbility { get => containedAbility; set => containedAbility = value; }

    void Start()
    {
        CooldownHandler.OnAbitilityUsed += UpdateAbilityCooldownUI;
        SetTheAbilityUI(this.ContainedAbility);
    }

    protected void SetTheAbilityUI(Ability containedAbility)
    {
        //Set ability icon
        //ContainedAbilityIcon.sprite = containedAbility.AbilityIcon;
        //Set ability key
        abilityKey = containedAbility.AbilityKey;
        keyText.text = abilityKey.ToString();
    }

    private void UpdateAbilityCooldownUI(Ability containedAbility)
    {
        if (containedAbility == ContainedAbility)
        {
            //Activer le timer text et l'image filled
            cooldownContainerGameObject.SetActive(true);

            //Update le texte et l'image filled
            StartCoroutine(UpdateCooldownUIComponents(containedAbility));
        }
    }

    private IEnumerator UpdateCooldownUIComponents(Ability containedAbility)
    {
        float storedCooldown = containedAbility.AbilityCooldown;

        do
        {
            storedCooldown -= Time.deltaTime;

            //Mettre à jour le timer text
            cooldownText.text = storedCooldown.ToString("0.0");
            //Update l'image filled
            cooldownFiller.fillAmount = storedCooldown / containedAbility.AbilityCooldown;

            yield return new WaitForEndOfFrame();
        } while (storedCooldown > 0);

        //Désactiver le timer text et l'image filled
        cooldownContainerGameObject.SetActive(false);
    }
}
