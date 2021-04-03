using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SteleState
{
    Inactive, 
    Active, 
    StandBy,
}

public class SteleLogic : InteractiveBuilding, IKillable, IDamageable
{
    public delegate void SteleInteractionHandler();
    public event SteleInteractionHandler OnInteraction;
    public event SteleInteractionHandler OnEndOFInteraction;

    [SerializeField] private int healthPoints = 0;
    [SerializeField] private SteleState steleState;
    private bool interactionIsHandled = false;
    private bool isDead = false;
    public int HealthPoints { get => healthPoints; set => healthPoints = value; }
    public SteleState SteleState { get => steleState; private set => steleState = value; }

    void Start()
    {
        SetSteleToInactiveMode();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (InteractingPlayer != null && !interactionIsHandled)
        {
            OnInteraction?.Invoke();
            interactionIsHandled = true;
        }
        else if (InteractingPlayer == null && interactionIsHandled)
        {
            OnEndOFInteraction?.Invoke();
            interactionIsHandled = false;
        }
    }

    public void SetSteleToActiveMode(int steleHealthPointsRelativeToEffect)
    {
        SteleState = SteleState.Active;
        InteractingPlayer.Target = null;
        InteractingPlayer = null;

        //pour l'équipe adversaire faire ça (ci-dessous)---
        //EntityDetection.TypeOfEntity = TypeOfEntity.Enemy;
        //-------------------------------------------------

        HealthPoints = steleHealthPointsRelativeToEffect;
    }

    private void SetSteleToInactiveMode()
    {
        IsInteractable = true;

        isDead = false;
        SteleState = SteleState.Inactive;
    }

    IEnumerator SetSteleToStandByMode()
    {
        yield return new WaitForSeconds(ReinitializationDelay);
        yield return new WaitForEndOfFrame();

        SetSteleToInactiveMode();
    }

    public void OnDeath()
    {
        IsInteractable = false;
        StartCoroutine(SetSteleToStandByMode());
    }

    public void TakeDamage(Transform character, float targetPhysicalResistances, float targetMagicalResistances, float characterPhysicalPower, float characterMagicalPower, float characterCriticalStrikeChance, float characterCriticalStrikeMultiplier, float characterPhysicalPenetration, float characterMagicalPenetration)
    {
        HealthPoints -= (int)characterPhysicalPower;
        Debug.Log("STELE TOOK DAMAGE");

        if (HealthPoints == 0)
        {
            isDead = true;
            OnDeath();
        }
    }
}