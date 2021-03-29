using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBillboard : BillBoard
{
    [Header("CHARACTER BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI characterLevelText;

    private void OnEnable()
    {
        //Call event on level up += setCharacter Level
    }

    private void OnDisable()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        SetCharacterLevel();
    }

    protected override void LateUpdate() => base.LateUpdate();

    void SetCharacterLevel()
    {
        //characterLevelText = Stats.CurrentLevel.ToString();
    }
}