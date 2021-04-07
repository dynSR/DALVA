using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trucDeDerniereMinute : MonoBehaviour
{
    [SerializeField] private EntityStats Stats;
    [SerializeField] private GameObject victoryImage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Stats.IsDead)
        {
            victoryImage.SetActive(true);
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Quit Application");
    }
}
