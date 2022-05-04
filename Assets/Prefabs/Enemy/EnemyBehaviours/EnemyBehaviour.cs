using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for behaviours that can be attached to enemies to handle individual elements.
/// May not be the best structure for this but seems fine for now.
/// </summary>
abstract public class EnemyBehaviour : MonoBehaviour
{
    /// <summary>
    /// Whether this behaviour should be active
    /// </summary>
    public bool performBehaviour = true;

    /// <summary>
    /// Called every FixedUpdate() if the enemy is enabled
    /// </summary>
    protected abstract void Behave();

    /// <summary>
    /// Call on Start()
    /// </summary>
    protected abstract void Initiate();

    /// <summary>
    /// The main EnemyController script of the enemy this is attached to
    /// </summary>
    protected EnemyController mainController;
    protected EnemyController MainController
    {
        get
        {
            if (mainController == null)
                mainController = GetComponent<EnemyController>();
            return mainController;
        }
    }

    /// <summary>
    /// Call when creating enemy after setting it up
    /// </summary>
    public void OnInitiate()
    {
        Initiate();
    }

    void FixedUpdate()
    {
        if (performBehaviour && MainController.PerformBehaviours)
            Behave();
    }
}
