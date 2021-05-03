using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum MalusType {Life = 0, Attack = 1, Speed = 2, Stele = 3, All = 4}

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

    //Play Tab
    [Header("Play tab assets")]
        //Class
    [Tooltip("The Warrior class button.")]
    public Button warriorButton;
    [Tooltip("The Mage class button.")]
    public Button mageButton;
    [Tooltip("The Warrior class button.")]
    public GameObject warriorText;
    [Tooltip("The Mage class button.")]
    public GameObject mageText;
    [Space(10)]
        //Difficulty
            //Life assets
    [Tooltip("The minus button for the enemies' life bonus.")]
    public Button minusLifeButton;
    [Tooltip("The plus button for the enemies' life bonus.")]
    public Button plusLifeButton;
    [Tooltip("The text of the current value of the enemies' life bonus.")]
    public TMP_Text currentLifeMalusText;
    [Space(10)]
            //Attack assets
    [Tooltip("The minus button for the enemies' attack bonus.")]
    public Button minusAttackButton;
    [Tooltip("The plus button for the enemies' attack bonus.")]
    public Button plusAttackButton;
    [Tooltip("The text of the current value of the enemies' attack bonus.")]
    public TMP_Text currentAttackMalusText;
    [Space(10)]
            //Speed assets
    [Tooltip("The minus button for the enemies' speed bonus.")]
    public Button minusSpeedButton;
    [Tooltip("The plus button for the enemies' speed bonus.")]
    public Button plusSpeedButton;
    [Tooltip("The text of the current value of the enemies' speed bonus.")]
    public TMP_Text currentSpeedMalusText;
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
    private Animator myAnimator;

    private int currentOpenedTab;
    private int nextOpenedTab;

    private bool characterClass; //false = Warrior, true = mage

    private bool faded;

    //Difficulty
    [Header("Difficulty limits")]
    public int mediumDifficulty;
    public int hardDifficulty;
    public int impossibleDifficulty;
    private int totalDifficulty;

    [Header("Difficulty meter values")]
    public int maxLifeMalus;
    private int currentLifeMalus;
    public int difficultyValueLifeMalus;

    public int maxAttackMalus;
    private int currentAttackMalus;
    public int difficultyValueAttackMalus;

    public int maxSpeedMalus;
    private int currentSpeedMalus;
    public int difficultyValueSpeedMalus;

    public int maxSteleMalus;
    private int currentSteleMalus;
    public int difficultyValueSteleMalus;

    #endregion

    void Start()
    {
        //Init
        myAnimator = GetComponentInParent<Animator>();

        //Play Tab
        characterClass = false; //Warrior by default

        currentLifeMalus = 0;
        currentAttackMalus = 0;
        currentSpeedMalus = 0;
        currentSteleMalus = 0;


        //Init text
        DifficultyMalusTextUpdate(MalusType.All);

        //Fade
        myAnimator.SetBool("fadingOut", true);
    }

    void Update()
    {
        
    }

    #region Methods

    private void DifficultyMalusTextUpdate(MalusType malus)
    {
        switch (malus)
        {
            case MalusType.Life:
                currentLifeMalusText.text = "+ " + (currentLifeMalus * (100/maxLifeMalus)).ToString() + "%";
                break;
            case MalusType.Attack:
                currentAttackMalusText.text = "+ " + (currentAttackMalus * (100 / maxAttackMalus)).ToString() + "%";
                break;
            case MalusType.Speed:
                currentSpeedMalusText.text = "+ " + (currentSpeedMalus * (100 / maxSpeedMalus)).ToString() + "%";
                break;
            case MalusType.Stele:
                currentSteleMalusText.text = "- " + (currentSteleMalus * 2).ToString();
                break;
            case MalusType.All:
                currentLifeMalusText.text = "+ " + (currentLifeMalus * (100 / maxLifeMalus)).ToString() + "%";
                currentAttackMalusText.text = "+ " + (currentAttackMalus * (100 / maxAttackMalus)).ToString() + "%";
                currentSpeedMalusText.text = "+ " + (currentSpeedMalus * (100 / maxSpeedMalus)).ToString() + "%";
                currentSteleMalusText.text = "- " + (currentSteleMalus * 2).ToString();
                break;
            default:
                break;
        }

        DifficultyValueTextUpdate();
    }
    //Update the text of the corresponding malus

    private void DifficultyButtonsUpdate(int value, int maxValue, Button minusButton, Button plusButton)
    {
        if (value == 0) minusButton.interactable = false;
        else if (value == maxValue) plusButton.interactable = false;
        else
        {
            minusButton.interactable = true;
            plusButton.interactable = true;
        }
    }
    //Makes the minus/plus button (difficulty) non-interactable/interactable depending of the new malus value

    private void DifficultyValueTextUpdate()
    {
        if(totalDifficulty <= mediumDifficulty)
        {
            easyText.SetActive(true);
            mediumText.SetActive(false);
            hardText.SetActive(false);
            impossibleText.SetActive(false);
            return;
        }
        else if(totalDifficulty <= hardDifficulty)
        {
            easyText.SetActive(false);
            mediumText.SetActive(true);
            hardText.SetActive(false);
            impossibleText.SetActive(false);
            return;
        }
        else if (totalDifficulty <= impossibleDifficulty)
        {
            easyText.SetActive(false);
            mediumText.SetActive(false);
            hardText.SetActive(true);
            impossibleText.SetActive(false);
            return;
        }
        else if (totalDifficulty > impossibleDifficulty)
        {
            easyText.SetActive(false);
            mediumText.SetActive(false);
            hardText.SetActive(false);
            impossibleText.SetActive(true);
            return;
        }
    }
    //Update the general difficulty text
    //(Miam le bon code d'UI)

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    #endregion

    #region Buttons methods

    public void SwitchClass()
    {
        characterClass = !characterClass;

        if (characterClass)
        {
            warriorButton.interactable = true;
            mageButton.interactable = false;
            myAnimator.SetBool("isMage", true);
            warriorText.SetActive(false);
            mageText.SetActive(true);
        }
        else
        {
            warriorButton.interactable = false;
            mageButton.interactable = true;
            myAnimator.SetBool("isMage", false);
            warriorText.SetActive(true);
            mageText.SetActive(false);
        }
    }

    public void ChangeOpenedTab(int tab)
    {
        nextOpenedTab = tab;

        if (currentOpenedTab != nextOpenedTab)
        {
            myAnimator.SetBool("aTabIsOpen", true);
            myAnimator.SetInteger("openedTab", tab);
            
            currentOpenedTab = nextOpenedTab;
        }
        else
        {
            myAnimator.SetBool("aTabIsOpen", false);
            myAnimator.SetInteger("openedTab", 0);

            currentOpenedTab = 0;
        }

        nextOpenedTab = 0;
    }
    //0 = None, 1 = Play, 2 = Encyclopedia, 3 = Shop, 4 = Options, 5 = Credits, 6 = Quit

    public void ChangeDifficultyButton(DifficultyButton button)
    {
        int sign = 0;
        if (button.plus) sign = 1;
        else sign = -1;

        switch (button.malus)
        {
            case MalusType.Life:
                currentLifeMalus += sign;
                totalDifficulty += difficultyValueLifeMalus * sign;
                DifficultyMalusTextUpdate(MalusType.Life);
                DifficultyButtonsUpdate(currentLifeMalus, maxLifeMalus, minusLifeButton, plusLifeButton);
                break;
            case MalusType.Attack:
                currentAttackMalus += sign;
                totalDifficulty += difficultyValueAttackMalus * sign;
                DifficultyMalusTextUpdate(MalusType.Attack);
                DifficultyButtonsUpdate(currentAttackMalus, maxAttackMalus, minusAttackButton, plusAttackButton);
                break;
            case MalusType.Speed:
                currentSpeedMalus += sign;
                totalDifficulty += difficultyValueSpeedMalus * sign;
                DifficultyMalusTextUpdate(MalusType.Speed);
                DifficultyButtonsUpdate(currentSpeedMalus, maxSpeedMalus, minusSpeedButton, plusSpeedButton);
                break;
            case MalusType.Stele:
                currentSteleMalus += sign;
                totalDifficulty += difficultyValueSteleMalus * sign;
                DifficultyMalusTextUpdate(MalusType.Stele);
                DifficultyButtonsUpdate(currentSteleMalus, maxSteleMalus, minusSteleButton, plusSteleButton);
                break;
            case MalusType.All:
                break;
            default:
                break;
        }
    }
    //When a malus button is clicked, check which one (minus/plus), and remove/add an unit to the corresponding malus type.
    //Also, makes the button non-interactable if it's out of range (0 to maxValue).

    public void StartGameButton()
    {
        myAnimator.SetBool("fadingOut", false);
        myAnimator.SetBool("startGame", true);
    }


    public void QuitGameButton()
    {
        Application.Quit();
    }

    #endregion

}
