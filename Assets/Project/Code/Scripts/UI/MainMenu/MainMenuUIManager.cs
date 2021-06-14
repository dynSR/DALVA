using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [Tooltip("The warrior animator.")]
    public Animator warriorAnimator;
    [Tooltip("The mage animator.")]
    public Animator mageAnimator;
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
    [Space(10)]*/
            
    //Difficulty text
    [Tooltip("The text  for the easy difficulty.")]
    public GameObject easyWindow;
    [Tooltip("The text  for the medium difficulty.")]
    public GameObject mediumWindow;
    [Tooltip("The text  for the hard difficulty.")]
    public GameObject hardWindow;

    //New difficulty
    [Tooltip("The toggle for easy difficulty.")]
    public Button easyButton;
    [Tooltip("The toggle for medium difficulty.")]
    public Button mediumButton;
    [Tooltip("The toggle for hard difficulty.")]
    public Button hardButton;
    [Space(10)]

    [Tooltip("The button that start the game.")]
    public Button startGameButton;

    #endregion

    #region Fields

    //General
    private Animator myAnimator;

    private GameParameters myGameParameters;

    private bool hasPickedADifficulty = false;
    private string gameSceneName;

    private int currentOpenedTab;
    private int nextOpenedTab;

    private bool hasPickedAClass = false;
    private bool characterClass = false; //false = Warrior, true = mage
    #endregion

    void Start()
    {
        //Init
        myAnimator = GetComponentInParent<Animator>();
        myGameParameters = GameParameters.Instance;

        //Level Progress
        LevelProgress();

        //Fade
        myAnimator.SetBool("fadingOut", true);
    }

    void Update()
    {
        ClassTextHighlight();
        DifficultyTextHighlight();
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

    private void ClassTextHighlight()
    {
        if (warriorButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
        {
            if (!hasPickedAClass) warriorText.SetActive(true);
            else if(characterClass)
            {
                warriorText.SetActive(true);
                mageText.SetActive(false);
            }
        }
        else if (!hasPickedAClass) warriorText.SetActive(false);
        else
        {
            warriorText.SetActive(false);
            mageText.SetActive(true);
        }

        if (mageButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
        {
            if (!hasPickedAClass) mageText.SetActive(true);
            else if (!characterClass)
            {
                warriorText.SetActive(false);
                mageText.SetActive(true);
            }
        }
        else if (!hasPickedAClass) mageText.SetActive(false);
        else
        {
            warriorText.SetActive(true);
            mageText.SetActive(false);
        }
    }

    private void DifficultyTextHighlight()
    {
        if (!hasPickedADifficulty)
        {
            if (easyButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy) easyWindow.SetActive(true);
            else easyWindow.SetActive(false);

            if (mediumButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy) mediumWindow.SetActive(true);
            else mediumWindow.SetActive(false);

            if (hardButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy) hardWindow.SetActive(true);
            else hardWindow.SetActive(false);
        }
        else
        {
            if(gameSceneName == easyGameSceneName)
            {
                if (mediumButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
                {
                    easyWindow.SetActive(false);
                    mediumWindow.SetActive(true);
                    return;
                }
                else
                {
                    easyWindow.SetActive(true);
                    mediumWindow.SetActive(false);
                }

                if (hardButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
                {
                    easyWindow.SetActive(false);
                    hardWindow.SetActive(true);
                    return;
                }
                else
                {
                    easyWindow.SetActive(true);
                    hardWindow.SetActive(false);
                }
            }

            if (gameSceneName == mediumGameSceneName)
            {
                if (easyButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
                {
                    mediumWindow.SetActive(false);
                    easyWindow.SetActive(true);
                    return;
                }
                else
                {
                    mediumWindow.SetActive(true);
                    easyWindow.SetActive(false);
                }

                if (hardButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
                {
                    mediumWindow.SetActive(false);
                    hardWindow.SetActive(true);
                    return;
                }
                else
                {
                    mediumWindow.SetActive(true);
                    hardWindow.SetActive(false);
                }
            }

            if (gameSceneName == hardGameSceneName)
            {
                if (easyButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
                {
                    hardWindow.SetActive(false);
                    easyWindow.SetActive(true);
                    return;
                }
                else
                {
                    hardWindow.SetActive(true);
                    easyWindow.SetActive(false);
                }

                if (mediumButton.GetComponent<UIButtonHighlight>().myBorder.activeInHierarchy)
                {
                    hardWindow.SetActive(false);
                    mediumWindow.SetActive(true);
                    return;
                }
                else
                {
                    hardWindow.SetActive(true);
                    mediumWindow.SetActive(false);
                }
            }
        }
    }

    private void LevelProgress()
    {
        switch (myGameParameters.maxLevelDone)
        {
            case 0:
                break;
            case 1:
                mediumButton.interactable = true;
                break;
            case 2:
                mediumButton.interactable = true;
                hardButton.interactable = true;
                break;
            default:
                break;
        }
    }

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
        GameParameters.Instance.classIsMage = charaClass;

        if (characterClass)
        {
            warriorButton.interactable = true;
            warriorButton.GetComponent<UIButtonHighlight>().HideBorder();
            mageButton.interactable = false;

            myAnimator.SetBool("isMage", true);
            mageAnimator.SetBool("UsesSecondAbility", true);

            warriorText.SetActive(false);
            mageText.SetActive(true);
        }
        else
        {
            warriorButton.interactable = false;
            mageButton.interactable = true;
            mageButton.GetComponent<UIButtonHighlight>().HideBorder();

            myAnimator.SetBool("isMage", false);
            warriorAnimator.SetBool("UsesSecondAbility", true);

            warriorText.SetActive(true);
            mageText.SetActive(false);
        }

        myAnimator.SetBool("classSelected", true);
        hasPickedAClass = true;

        CanStartGameCheck();
    }

    public void ResetClassAndDifficulty()
    {
        //Class
        hasPickedAClass = false;
        myAnimator.SetBool("classSelected", false);
        characterClass = false;
        myAnimator.SetBool("isMage", false);
        warriorButton.interactable = true;
        warriorButton.GetComponent<UIButtonHighlight>().HideBorder();
        mageButton.interactable = true;
        mageButton.GetComponent<UIButtonHighlight>().HideBorder();

        //Difficulty
        hasPickedADifficulty = false;
        easyButton.interactable = true;
        easyButton.GetComponent<UIButtonHighlight>().HideBorder();
        mediumButton.interactable = true;
        mediumButton.GetComponent<UIButtonHighlight>().HideBorder();
        hardButton.interactable = true;
        hardButton.GetComponent<UIButtonHighlight>().HideBorder();

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
    //0 = None, 1 = Play, 2 = Encyclopedia, 3 = Shop, 4 = Options, 5 = Credits, 6 = Quit, 7 = Cutscenes

    public void ChangeNewDifficultyButton(Button button)
    {
        if (button == easyButton)
        {
            gameSceneName = easyGameSceneName;
            easyButton.interactable = false;
            if(myGameParameters.maxLevelDone >= 1) mediumButton.interactable = true;
            mediumButton.GetComponent<UIButtonHighlight>().HideBorder();
            if(myGameParameters.maxLevelDone >= 1) hardButton.interactable = true;
            hardButton.GetComponent<UIButtonHighlight>().HideBorder();

            easyWindow.SetActive(true);
            mediumWindow.SetActive(false);
            hardWindow.SetActive(false);
        }

        if (button == mediumButton)
        {
            gameSceneName = mediumGameSceneName;
            easyButton.interactable = true;
            easyButton.GetComponent<UIButtonHighlight>().HideBorder();
            mediumButton.interactable = false;
            if (myGameParameters.maxLevelDone >= 1) hardButton.interactable = true;
            hardButton.GetComponent<UIButtonHighlight>().HideBorder();

            easyWindow.SetActive(false);
            mediumWindow.SetActive(true);
            hardWindow.SetActive(false);
        }

        if (button == hardButton)
        {
            gameSceneName = hardGameSceneName;
            easyButton.interactable = true;
            easyButton.GetComponent<UIButtonHighlight>().HideBorder();
            if (myGameParameters.maxLevelDone >= 1) mediumButton.interactable = true;
            mediumButton.GetComponent<UIButtonHighlight>().HideBorder();
            hardButton.interactable = false;

            easyWindow.SetActive(false);
            mediumWindow.SetActive(false);
            hardWindow.SetActive(true);
        }

        hasPickedADifficulty = true;
        CanStartGameCheck();
    }

   

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
