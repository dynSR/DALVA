using TMPro;
using UnityEngine;

public class CharacterBillboard : Billboard
{
    [Header("CHARACTER BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI characterLevelText;
    [SerializeField] private string stunStatus;

    private void OnEnable()
    {
        //Call event on level up += setCharacter Level
        //Call event for when character is stunned 
    }

    private void OnDisable()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        SetCharacterLevel();
    }
    protected override void Start() => base.Start();

    protected override void LateUpdate() => base.LateUpdate();

    void SetCharacterLevel()
    {
        //characterLevelText = Stats.CurrentLevel.ToString();
    }

    void SetCharacterStatus()
    {
        //if stunned
        nameText.text = stunStatus;
        //else if smth else
    }
}