using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleController : MonoBehaviour
{
    /// <summary>
    /// The scriptable object that manages the players available resources
    /// </summary>
    public PlayerResourceManager playerResourceManager;

    /// <summary>
    /// Event that is to be invoked at the beginning of a cycle
    /// </summary>
    public GameEvent cycleBegin;
    /// <summary>
    /// Event that is to be invoked at the end of a cycle
    /// </summary>
    public GameEvent cycleEnd;

    /// <summary>
    /// Event that is invoked at the beginning of a round
    /// </summary>
    public GameEvent roundBegin;
    /// <summary>
    /// Event that is invoked at the end of a round
    /// </summary>
    public GameEvent roundEnd;

    /// <summary>
    /// How many game ticks between the beginning and end of each cycle
    /// </summary>
    public int CycleDuration = 300;
    int CycleTimer;
    bool cycleActive = false;

    public GameObject cycleProgressDisplay;

    void Awake()
    {
        Debug.Log(roundBegin);
        roundBegin.RegisterListener(OnRoundBegin);
        roundEnd.RegisterListener(OnRoundEnd);
    }

    void FixedUpdate()
    {
        if (CycleTimer > 0 && cycleActive)
            CycleTimer -= 1;
        cycleProgressDisplay.GetComponent<ProgressBarController>().Amount = CycleDuration - CycleTimer;
    }

    /// <summary>
    /// Called at the start of the round to reset the cycles
    /// </summary>
    public void OnRoundBegin()
    {
        CycleTimer = CycleDuration;
        cycleProgressDisplay.GetComponent<ProgressBarController>().Maximum = CycleDuration;
        cycleActive = true;

        cycleEnd.Raise();
        cycleBegin.Raise();
    }

    /// <summary>
    /// called when round ends to stop the cycling
    /// <summary>
    public void OnRoundEnd()
    {
        cycleActive = false;
        cycleEnd.Raise();
    }

    /// <summary>
    /// Calls for the next cycle to start; only works if cycle timer is low enough
    /// </summary>
    public void NextCycle() 
    { 
        if (true)//CycleTimer <= 0 && cycleActive)
        {
            CycleTimer = CycleDuration;
            
            // end cycle 
            cycleEnd.Raise();
            playerResourceManager.Reset();

            // begin cycle
            cycleBegin.Raise();
        }
    }
}