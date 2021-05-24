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

    public Outline Outline;

    private void Start() => SetOutlineColor();

    void SetOutlineColor()
    {
        //switch (TypeOfEntity)
        //{
        //    case TypeOfEntity.Stele:
        //        Outline.OutlineColor = Color.yellow;
        //        break;
        //    case TypeOfEntity.Harvester:
        //        Outline.OutlineColor = Color.yellow;
        //        break;
        //}

        Outline.OutlineColor = Color.black;
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
    }

    public void DeactivateTargetOutlineOnHover(Outline targetOutlineFound)
    {
        targetOutlineFound.enabled = false;
    }
    #endregion
}