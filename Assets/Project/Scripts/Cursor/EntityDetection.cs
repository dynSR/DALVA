using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfEntity { None, Self, Ennemy, Ally, Stele }

public class EntityDetection : MonoBehaviour
{
    [SerializeField] private TypeOfEntity typeOfEntity;
    public TypeOfEntity TypeOfEntity { get => typeOfEntity; }
}