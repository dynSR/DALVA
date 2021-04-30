using UnityEngine;

public class SteleBillboard : Billboard
{
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private GameObject healthBar;
    private SteleLogic stele;

    protected override void OnEnable()
    {
        stele.OnInteraction += DisplayBuildButtons;
        stele.OnEndOFInteraction += HideBuildButtons;

        //stele.OnActivation += DisplayHealthBar;
        //stele.OnSteleDeath += HideHealthBar;
    }

    protected override void OnDisable()
    {
        stele.OnInteraction -= DisplayBuildButtons;
        stele.OnEndOFInteraction -= HideBuildButtons;

        //stele.OnActivation -= DisplayHealthBar;
        //stele.OnSteleDeath -= HideHealthBar;
    }

    protected override void Awake()
    {
        base.Awake();
        stele = GetComponentInParent<SteleLogic>();
    }
    protected override void Start() => base.Start();

    protected override void LateUpdate() => base.LateUpdate();

    void DisplayBuildButtons()
    {
        buttonHolder.SetActive(true);
    }

    public void HideBuildButtons()
    {
        buttonHolder.SetActive(false);
    }

    //void DisplayHealthBar()
    //{
    //    healthBar.SetActive(true);
    //}

    //public void HideHealthBar()
    //{
    //    healthBar.SetActive(false);
    //}
}
