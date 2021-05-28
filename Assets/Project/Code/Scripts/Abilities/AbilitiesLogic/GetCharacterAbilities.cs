using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCharacterAbilities : MonoBehaviour
{
    private EntityStats CharacterStats => transform.parent.GetComponentInParent<EntityStats>();

    [Header("PLAYER ABILITIES INFORMATIONS")]
    [Tooltip("Needs to match the number of abilities present on the player (as scripts)")]
    [SerializeField] private int numberOfAbilities = 2;
    [SerializeField] private List<KeyCode> abilitiesInputKeys;
    [SerializeField] private GameObject abilityContainerPrefab;

    [Header("ABILITY IN USE FEEDBACK")]
    [SerializeField] private GameObject glowFeedbackObject;

    private readonly List<AbilityContainerLogic> abilityContainers = new List<AbilityContainerLogic>();
    
    private void Start()
    {
        CreateAbilityContainersAtStart();
    }

    void CreateAbilityContainersAtStart()
    {
        for (int i = 0 ; i <= numberOfAbilities -1; i++)
        {
            GameObject abilityContainerInstance = Instantiate(abilityContainerPrefab);
            abilityContainerInstance.transform.SetParent(transform);
            abilityContainerInstance.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            abilityContainers.Add(abilityContainerInstance.GetComponent<AbilityContainerLogic>());

            if (abilityContainers.Count == numberOfAbilities)
            {
                for (int j = 0; j < abilityContainers.Count; j++)
                {
                    abilityContainers[j].ContainedAbility = CharacterStats.EntityAbilities[j];
                    CharacterStats.EntityAbilities[j].Container = abilityContainers[j];
                    abilityContainers[j].Parent = this;
                }
            }
        }
    }

    public void LockOtherUnusedAbilities(KeyCode input)
    {
        for (int i = abilitiesInputKeys.Count - 1; i >= 0; i--)
        {
            if (abilityContainers[i].AbilityKey != input)
            {
                abilityContainers[i].ContainedAbility.CanBeUsed = false;
            }
        }
        
    }

    public void UnlockOtherUnusedAbilities(KeyCode input)
    {
        for (int i = abilitiesInputKeys.Count - 1; i >= 0; i--)
        {
            if (abilityContainers[i].AbilityKey != input)
            {
                abilityContainers[i].ContainedAbility.CanBeUsed = true;
            }
        }
    }

    public void DisplayGlowEffect()
    {
        //Start Glow Animation
        glowFeedbackObject.SetActive(true);
    }

    public void HideGlowEffect()
    {
        //Stop Glow Animation
        glowFeedbackObject.SetActive(false);
    }
}
