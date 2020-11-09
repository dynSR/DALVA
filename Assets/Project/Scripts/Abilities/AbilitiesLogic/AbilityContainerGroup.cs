using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContainerGroup : MonoBehaviour
{
    private Character CharacterCharacteristics => transform.parent.GetComponentInParent<Character>();

    [SerializeField] private int numberOfAbilities = 4;
    [SerializeField] private List<KeyCode> keys;
    [SerializeField] private GameObject abilityContainerPrefab;
    [SerializeField] private List<AbilityContainer> abilityContainers = new List<AbilityContainer>();
    
    private void Start()
    {
        CreateAbilityContainers();
    }

    void CreateAbilityContainers()
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
