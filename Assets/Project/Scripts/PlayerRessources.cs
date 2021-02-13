using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRessources : MonoBehaviour
{
    [Header("PLAYER SHOP")]
    [SerializeField] private OpenCloseShopWindow playerShopWindow;

    [Header("PLAYER RESSOURCES")]
    [SerializeField] private float amountOfPlayerRessources = 500;

    public OpenCloseShopWindow PlayerShopWindow { get => playerShopWindow; set => playerShopWindow = value; }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddRessources(500);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            RemoveRessources(250);
        }
    }

    private void AddRessources(int amountToAdd)
    {
        amountOfPlayerRessources += amountToAdd;
    }

    private void RemoveRessources(int amountToRemove)
    {
        amountOfPlayerRessources -= amountToRemove;
    }
}
