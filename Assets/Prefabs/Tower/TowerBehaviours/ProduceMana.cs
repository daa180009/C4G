using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProduceMana : TowerBehaviour
{
    /// <summary>
    /// Event that is invoked at the end of a cycle
    /// </summary>
    public GameEvent cycleBegin;

    /// <summary>
    /// The scriptable object that manages the player's resources (including mana)
    /// </summary>
    public PlayerResourceManager playerResourceManager;

    /// <summary>
    /// The mana type to be produced by the tower
    /// </summary>
    public Mana.ManaType manaType;

    protected override void Initiate()
    {
        cycleBegin.RegisterListener(OnCycleBegin);
    }

    // empty because mana tower reacts to cycles, not to behave calls
    protected override void Behave(){ return;  }

    public void OnCycleBegin()
    {
        if(performBehaviour && MainController.PerformBehaviours)
            playerResourceManager.AddMana(manaType, 1);
    }

    public override string GetDescription()
    {
        return "Mana";
    }
}
