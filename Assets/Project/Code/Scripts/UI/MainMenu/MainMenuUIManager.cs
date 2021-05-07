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
    [Tooltip("The easy game scene name.")]
    public string easyGameSceneName;
    [Tooltip("The medium game scene name.")]
    public string mediumGameSceneName;
    [Tooltip("The hard game scene name.")]
    public string hardGameSceneName;

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
    /*[Tooltip("The minus button for the enemies' life bonus.")]
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
    public GameObject impossibleText;*/

    //New difficulty
    [Tooltip("The toggle for easy difficulty.")]
    public Toggle easyToggle;
    [Tooltip("The toggle for medium difficulty.")]
    public Toggle mediumToggle;
    [Tooltip("The toggle for hard difficulty.")]
    public Toggle hardToggle;
    [Space(10)]

    [Tooltip("The button that start the game.")]
    public Button startGameButton;

    #endregion

    #region Fields

    //General
    private Animator myAnimator;

    private GameParameters myGameParameters;

    private bool hasPickedADifficulty;
    private string gameSceneName;

    private int currentOpenedTab;
    private int nextOpenedTab;

    private bool hasPickedAClass = false;
    private bool characterClass = false; //false = Warrior, true = mage

    //Difficulty
    /*[Header("Difficulty limits")]
    public float mediumDifficulty;
    public float hardDifficulty;
    public float impossibleDifficulty;
    private float totalDifficulty;

    [Header("Difficulty meter values")]
    public int maxLifeMalus;
    private int currentLifeMalus;
    public int maxLifePercent;
    public float difficultyValueLifeMalus;

    public int maxAttackMalus;
    private int currentAttackMalus;
    public int maxAttackPercent;
    public float difficultyValueAttackMalus;

    public int maxSpeedMalus;
    private int currentSpeedMalus;
    public int maxSpeedPercent;
    public float difficultyValueSpeedMalus;

    public int maxSteleMalus;
    private int currentSteleMalus;
    public int maxStelePercent;
    public float difficultyValueSteleMalus;*/

    #endregion

    void Start()
    {
        //Init
        myAnimator = GetComponentInParent<Animator>();
        myGameParameters = GameParameters.singleton;

        //Play Tab
        //currentLifeMalus = myGameParameters.lifeMalus;
        //currentAttackMalus = myGameParameters.attackMalus;
        //currentSpeedMalus = myGameParameters.speedMalus;
        //currentSteleMalus = myGameParameters.steleMalus;


        //Init text
        //DifficultyMalusTextUpdate(MalusType.All);

        //Fade
        myAnimator.SetBool("fadingOut", true);
    }

    void Update()
    {
        
    }

    #region Methods

    /*private void DifficultyMalusTextUpdate(MalusType malus)
    {
        switch (malus)
        {
            case MalusType.Life:
                currentLifeMalusText.text = "+ " + (currentLifeMalus * (maxLifePercent / maxLifeMalus)).ToString() + "%";
                break;
            case MalusType.Attack:
                currentAttackMalusText.text = "+ " + (currentAttackMalus * (maxAttackPercent / maxAttackMalus)).ToString() + "%";
                break;
            case MalusType.Speed:
                currentSpeedMalusText.text = "+ " + (currentSpeedMalus * (maxSpeedPercent / maxSpeedMalus)).ToString() + "%";
                break;
            case MalusType.Stele:
                currentSteleMalusText.text = "- " + (currentSteleMalus * maxStelePercent).ToString();
                break;
            case MalusType.All:
                currentLifeMalusText.text = "+ " + (currentLifeMalus * (maxLifePercent / maxLifeMalus)).ToString() + "%";
                currentAttackMalusText.text = "+ " + (currentAttackMalus * (maxAttackPercent / maxAttackMalus)).ToString() + "%";
                currentSpeedMalusText.text = "+ " + (currentSpeedMalus * (maxSpeedPercent / maxSpeedMalus)).ToString() + "%";
                currentSteleMalusText.text = "- " + (currentSteleMalus * maxStelePercent).ToString();
                break;
            default:
                break;
        }

        DifficultyValueTextUpdate();
    }*/
    //Update the text of the corresponding malus

    /*private void DifficultyButtonsUpdate(int value, int maxValue, Button minusButton, Button plusButton)
    {
        if (value == 0) minusButton.interactable = false;
        else if (value == maxValue) plusButton.interactable = false;
        else
        {
            minusButton.interactable = true;
            plusButton.interactable = true;
        }
    }*/
    //Makes the minus/plus button (difficulty) non-interactable/interactable depending of the new malus value

    /*private void DifficultyValueTextUpdate()
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
    }*/
    //Update the general difficulty text
    //(Miam le bon code d'UI)

    private void CanStartGameCheck()
    {
        if (hasPickedAClass && hasPickedADifficulty) startGameButton.interactable = true;
        else startGameButton.interactable = false;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    #endregion

    #region Buttons methods

    public void SwitchClass(bool charaClass)
    {
        characterClass = charaClass;

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

        hasPickedAClass = true;
        CanStartGameCheck();
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

    public void ChangeNewDifficultyButton(int difficulty)
    {
        Debug.Log("Bouton difficulté");

        switch (difficulty)
        {
            case 0:
                gameSceneName = easyGameSceneName;
                easyToggle.interactable = false;
                mediumToggle.interactable = true;
                hardToggle.interactable = true;
                Debug.Log("Easy difficulty");
                break;
            case 1:
                gameSceneName = mediumGameSceneName;
                easyToggle.interactable = true;
                mediumToggle.interactable = false;
                hardToggle.interactable = true;
                Debug.Log("Medium difficulty");
                break;
            case 2:
                gameSceneName = hardGameSceneName;
                easyToggle.interactable = true;
                mediumToggle.interactable = true;
                hardToggle.interactable = false;
                Debug.Log("Hard difficulty");
                break;
            default:
                break;
        }

        hasPickedADifficulty = true;
        CanStartGameCheck();
    }

    /*public void ChangeDifficultyButton(DifficultyButton button)
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
    }*/
    //When a malus button is clicked, check which one (minus/plus), and remove/add an unit to the corresponding malus type.
    //Also, makes the button non-interactable if it's out of range (0 to maxValue).

    public void StartGameButton()
    {
        myAnimator.SetBool("fadingOut", false);
        myAnimator.SetBool("startGame", true);
        //myGameParameters.SetGameParameters(characterClass, currentLifeMalus, currentAttackMalus, currentSpeedMalus, currentSteleMalus);
    }


    public void QuitGameButton()
    {
        Application.Quit();
    }

    #endregion

}
