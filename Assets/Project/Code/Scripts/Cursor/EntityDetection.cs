using UnityEngine;

public enum TypeOfEntity 
{ 
    None,
    Player, Monster, Minion,
    Stele, SteleEffect,
    Harvester 
}

[RequireComponent(typeof(VisibilityState))]
public class EntityDetection : MonoBehaviour
{
    [SerializeField] private TypeOfEntity typeOfEntity;
    public TypeOfEntity TypeOfEntity { get => typeOfEntity; set => typeOfEntity = value; }

    [Header("OUTLINES")]
    public Outline Outline;
    public Outline SecondOutline;

    [Header("SELECTION GO")]
    public GameObject SelectionObject;

    [Header("COLORS")]
    public Color defaultOutlineColor;
    public Color otherDefaultOutlineColor;

    private void Awake()
    {
        SetSelectionObject();
    }

    private void Start() => SetOutlineColor();

    void SetOutlineColor()
    {
        if(Outline != null)
            Outline.OutlineColor = defaultOutlineColor;

        if (SecondOutline != null)
            SecondOutline.OutlineColor = otherDefaultOutlineColor;
    }

    #region Found entity type
    public bool ThisTargetIsAPlayer(EntityDetection targetFound)
    {
        if (targetFound.TypeOfEntity == TypeOfEntity.Player) return true;
        return false;
    }

    public bool ThisTargetIsAMonster(EntityDetection targetFound)
    {
        if (targetFound.TypeOfEntity == TypeOfEntity.Monster) return true;
        return false;
    }

    public bool ThisTargetIsAMinion(EntityDetection targetFound)
    {
        if (targetFound.TypeOfEntity == TypeOfEntity.Minion) return true;
        return false;
    }
    public bool ThisTargetIsAStele(EntityDetection targetFound)
    {
        if (targetFound.TypeOfEntity == TypeOfEntity.Stele) return true;
        return false;
    }

    public bool ThisTargetIsASteleEffect(EntityDetection targetFound)
    {
        if (targetFound.TypeOfEntity == TypeOfEntity.SteleEffect) return true;
        return false;
    }

    public bool ThisTargetIsAHarvester(EntityDetection targetFound)
    {
        if (targetFound.TypeOfEntity == TypeOfEntity.Harvester) return true;
        return false;
    }
    #endregion

    #region Outline Toggle
    public void ActivateTargetOutlineOnHover(Outline targetOutlineFound, Color outlineColor)
    {
        targetOutlineFound.enabled = true;
        targetOutlineFound.OutlineColor = outlineColor;

        if (SecondOutline != null)
        {
            SecondOutline.enabled = true;
            SecondOutline.OutlineColor = outlineColor;
        }  
    }

    public void DeactivateTargetOutlineOnHover(Outline targetOutlineFound)
    {
        if (targetOutlineFound.OutlineColor != defaultOutlineColor)
            targetOutlineFound.OutlineColor = defaultOutlineColor;

        if (SecondOutline != null && SecondOutline.OutlineColor != defaultOutlineColor)
        {
            SecondOutline.OutlineColor = otherDefaultOutlineColor;
        }

        //Debug.Log("SETTING OUTLINE TO BLACK COLOR", transform);
        //targetOutlineFound.enabled = false;
    }
    #endregion

    void SetSelectionObject()
    {
        if (SelectionObject == null) return;

        ParticleSystem selectionObjectPSComponent = SelectionObject.GetComponent<ParticleSystem>();
        EntityStats stats = transform.GetComponent<EntityStats>();
        InteractiveBuilding interactiveBuilding = transform.GetComponent<InteractiveBuilding>();

        var main = selectionObjectPSComponent.main;

        if (stats != null && stats.EntityTeam == EntityTeam.DALVA 
            || interactiveBuilding  != null && interactiveBuilding.EntityTeam == EntityTeam.DALVA)
        {
            main.startColor = Color.blue;
        }
        else if (stats != null && stats.EntityTeam == EntityTeam.HULRYCK)
        {
            main.startColor = Color.red;
        }
        else if (stats != null && stats.EntityTeam == EntityTeam.NEUTRAL
            || interactiveBuilding != null && interactiveBuilding.EntityTeam == EntityTeam.NEUTRAL)
        {
            if (typeOfEntity == TypeOfEntity.Stele || typeOfEntity == TypeOfEntity.Harvester)
            {
                main.startColor = Color.yellow;
            }
            else if (typeOfEntity == TypeOfEntity.Monster)
            {
                main.startColor = Color.red;
            }
        }
    }

    public void DisplaySelectionEffect()
    {
        if (SelectionObject != null && !SelectionObject.activeInHierarchy) SelectionObject.SetActive(true);
    }
}