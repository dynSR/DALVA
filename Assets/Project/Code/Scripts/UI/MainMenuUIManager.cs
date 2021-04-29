using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum MalusType {Life, Attack, Speed, Stele }

public class MainMenuUIManager : MonoBehaviour
{
    #region Assets

    //General assets
    [Header("General assets")]
    [Tooltip("The big black image for the fade in/fade out transition.")]
    public Image fadeInImage;
    [Tooltip("The name of the game scene.")]
    public string gameSceneName;

    //Main tab's buttons (always visible)
    [Header("Main tab")]
    [Tooltip("The play button from the main tab (always visible).")]
    public Button playTabButton;
    [Tooltip("The encyclopedia button from the main tab (always visible).")]
    public Button encyclopediaTabButton;
    [Tooltip("The shop button from the main tab (always visible).")]
    public Button shopTabButton;
    [Tooltip("The options button from the main tab (always visible).")]
    public Button optionsTabButton;
    [Tooltip("The quit button from the main tab (always visible).")]
    public Button quitTabButton;

    //Secondary tabs (not always visible)
    [Header("Secondary tabs")]
    [Tooltip("The play tab.")]
    public GameObject playTab;
    [Tooltip("The encyclopedia tab.")]
    public GameObject encyclopediaTab;
    [Tooltip("The shop tab.")]
    public GameObject shopTab;
    [Tooltip("The options tab.")]
    public GameObject optionsTab;
    [Tooltip("The quit tab.")]
    public GameObject quitTab;

    //Play Tab
    [Header("Play tab assets")]
        //Class
    [Tooltip("The Warrior class button.")]
    public Button warriorButton;
    [Tooltip("The Mage class button.")]
    public Button mageButton;
    [Space(10)]
        //Difficulty
            //Life assets
    [Tooltip("The minus button for the enemies' life bonus.")]
    public Button minusLifeButton;
    [Tooltip("The plus button for the enemies' life bonus.")]
    public Button plusLifeButton;
    [Tooltip("The text of the current value of the enemies' life bonus.")]
    public TMP_Text currentLifeBonusText;
    [Space(10)]
            //Attack assets
    [Tooltip("The minus button for the enemies' attack bonus.")]
    public Button minusAttackButton;
    [Tooltip("The plus button for the enemies' attack bonus.")]
    public Button plusAttackButton;
    [Tooltip("The text of the current value of the enemies' attack bonus.")]
    public TMP_Text currentAttackBonusText;
    [Space(10)]
            //Speed assets
    [Tooltip("The minus button for the enemies' speed bonus.")]
    public Button minusSpeedButton;
    [Tooltip("The plus button for the enemies' speed bonus.")]
    public Button plusSpeedButton;
    [Tooltip("The text of the current value of the enemies' speed bonus.")]
    public TMP_Text currentSpeedBonusText;
    [Space(10)]
            //Stele assets
    [Tooltip("The minus button for the stele malus.")]
    public Button minusSteleButton;
    [Tooltip("The plus button for the stele malus.")]
    public Button plusSteleButton;
    [Tooltip("The text of the current value of the stele malus.")]
    public TMP_Text currentSteleMalusText;
    [Space(10)]
            //Difficulty text
    [Tooltip("The text  for the easy difficulty.")]
    public GameObject easyText;
    [Tooltip("The text  for the medium difficulty.")]
    public GameObject mediumText;
    [Tooltip("The text  for the hard difficulty.")]
    public GameObject hardText;
    [Tooltip("The text  for the impossible difficulty.")]
    public GameObject impossibleText;

    #endregion

    #region Fields

    //General
    private GameObject currentOpenedTab;
    private GameObject nextOpenedTab;

    private bool characterClass; //false = Warrior, true = mage

    //Difficulty
    private int totalDifficulty;
    private int mediumDifficulty;
    private int hardDifficulty;
    private int impossibleDifficulty;

    private int currentLifeBonus;
    private int difficultyValueLifeBonus;

    private int currentAttackBonus;
    private int difficultyValueAttackBonus;

    private int currentSpeedBonus;
    private int difficultyValueSpeedBonus;

    private int currentSteleMalus;
    private int difficultyValueSteleMalus;

    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeDifficultyButton(bool plus, MalusType malus)
    {
        int button = 0;
        if (plus) button = 1;
        else button = -1;

        switch (malus)
        {
            case MalusType.Life:
                currentLifeBonus += button;
                totalDifficulty += difficultyValueLifeBonus;
                currentLifeBonusText.text = "+ " + (currentLifeBonus * 20).ToString() + "%";
                break;
            case MalusType.Attack:
                currentAttackBonus += button;
                totalDifficulty += difficultyValueAttackBonus;
                currentAttackBonusText.text = "+ " + (currentAttackBonus * 25).ToString() + "%";
                break;
            case MalusType.Speed:
                currentSpeedBonus += button;
                totalDifficulty += difficultyValueSpeedBonus;
                currentSpeedBonusText.text = "+ " + (currentSpeedBonus * 10).ToString() + "%";
                break;
            case MalusType.Stele:
                currentSteleMalus += button;
                totalDifficulty += difficultyValueSteleMalus;
                currentSteleMalusText.text = "- " + (currentSteleMalus*2).ToString();
                break;
            default:
                break;
        }
    }
}
