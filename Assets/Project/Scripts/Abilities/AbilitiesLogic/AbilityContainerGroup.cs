using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContainerGroup : MonoBehaviour
{
    private Stats CharacterCharacteristics => transform.parent.GetComponentInParent<Stats>();

    [Header("PLAYER ABILITIES INFORMATIONS")]
    [Tooltip("Needs to match the number of abilities present on the player (as scripts)")]
    [SerializeField] private int numberOfAbilities = 2;
    [SerializeField] private List<KeyCode> abilitiesInputKeys;
    [SerializeField] private GameObject abilityContainerPrefab;

    private List<AbilityContainer> abilityContainers = new List<AbilityContainer>();
    
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

            abilityContainers.Add(abilityContainerInstance.GetComponent<AbilityContainer>());

            if (abilityContainers.Count == numberOfAbilities)
            {
                for (int j = 0; j < abilityContainers.Count; j++)
                {
                    abilityContainers[j].ContainedAbility = CharacterCharacteristics.CharacterAbilities[j];
                }
            }
        }
    }
}
