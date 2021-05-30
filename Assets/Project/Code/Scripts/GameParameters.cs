using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameters : MonoBehaviour
{
    public enum Class { None, Mage, Warrior }

    public Class classChosen = Class.None;

    #region Singleton
    public static GameParameters Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }
    #endregion

    private void Start()
    {
        ActivateTheRightCharacter();
    }

    public void ActivateTheRightCharacter()
    {
        if(GameManager.Instance != null)
        {
            if (classChosen == Class.Mage)
            {
                GameManager.Instance.MageCharacter.SetActive(true);
            }
            else if (classChosen == Class.Warrior)
            {
                GameManager.Instance.WarriorCharacter.SetActive(true);
            }
        }
    }

    public void SetClassChosenToMage()
    {
        classChosen = Class.Mage;
    }

    public void SetClassChosenToWarrior()
    {
        classChosen = Class.Warrior;
    }
}
