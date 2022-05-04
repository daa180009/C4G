using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TowerController : MonoBehaviour
{
    public HPBarController hpBar;
    public Animator animator;

    /// <summary>
    /// The card data of the card that created this tower.
    /// </summary>
    public CardData CardParent;

    /// The amount of HP that the tower begins with by default
    public float baseHP = 10;
    float hp;

    /// <summary>
    /// Association of each status effect with its current duration
    /// </summary>
    Dictionary<Card.Status, float> statusDuration = new Dictionary<Card.Status, float>();

    /// <summary>
    /// The tile that the tower is sitting on
    /// </summary>
    public TileController ParentTile = null;

    Tile.TileDirection facingDirection = Tile.TileDirection.None;
    /// <summary>
    /// The direction the tower is facing, usually used for the tower's attacks and graphics;
    /// Setting it publically automatically changed its graphics
    /// </summary>
    public Tile.TileDirection FacingDirection
    {
        get { return facingDirection; }
        set
        {
            facingDirection = value;
            this.RotateToFace(value);   
        }
    }

    /// <summary>
    /// The behaviours for the tower to carry out, such as spawning materials, generating mana, etc.
    /// </summary>
    TowerBehaviour[] behaviours;

    bool performBehaviours = false;
    /// <summary>
    /// Whether or not the tower should have its behaviours active. 
    /// Setting false publically also causes the tower to become transparent
    /// </summary>
    public bool PerformBehaviours
    {
        get { return performBehaviours; }
        set
        {
            performBehaviours = value;
            GetComponent<Collider>().enabled = performBehaviours;
        }
    }

    /// <summary>
    /// Set to true when the tower is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    /// <summary>
    /// How long to keep around object while dying
    /// </summary>
    public int timeToDeath = 21;
    /// <summary>
    /// Timer counting down in frames while it is at or below 0 HP
    /// </summary>
    int dyingTimer;

    Action<TowerController> cbHovered;
    /// <summary>
    /// Register a method to be called when the tower is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<TowerController> cb) { cbHovered -= cb; cbHovered += cb; }

    Action<TowerController> cbDespawned;
    /// <summary>
    /// Register a function to be called when this tower is set to despawn from being destroyed
    /// </summary>
    public void RegisterDespawnedCB(Action<TowerController> cb) { cbDespawned -= cb; cbDespawned += cb; }
    /// <summary>
    /// Unregister a function to be called when this tower is set to despawn from being destroyed
    /// </summary>
    public void UnregisterDespawnedCB(Action<TowerController> cb) { cbDespawned -= cb; }

    Action<TowerController> cbDied;
    /// <summary>
    /// Register a function to be called when this tower starts to die from being destroyed
    /// </summary>
    public void RegisterDiedCB(Action<TowerController> cb) { cbDied -= cb; cbDied += cb; }

    void OnEnable()
    {
        behaviours = GetComponents<TowerBehaviour>();
        hp = baseHP;
        hpBar.Maximum = baseHP;
        dyingTimer = timeToDeath;
    }

    public void Initiate()
    {
        foreach (TowerBehaviour behaviour in behaviours)
        {
            behaviour.OnInitiate();
            RegisterDiedCB(behaviour.OnDeath);
        }
    }

    void FixedUpdate()
    {
        hpBar.transform.localPosition = (new Vector3(-.25f, 0, .25f)).Rotated(facingDirection);

        if (ParentTile != null)
        {
            transform.position = ParentTile.transform.position + new Vector3(0, ParentTile.Height, 0);

            if (hovered)
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;

        // decrease duration of all statuses
        Card.Status[] activeStatuses = statusDuration.Keys.ToArray();
        foreach (Card.Status status in activeStatuses)
        {
            statusDuration[status] -= Time.deltaTime;
            if (statusDuration[status] < 0f)
                statusDuration.Remove(status);
        }

        // take damage from burn
        if (HasStatus(Card.Status.Burn))
        {
            hp -= 1f * Time.deltaTime;
        }

        // If has both burn and freeze then remove both and take damage
        if (HasStatus(Card.Status.Burn) && HasStatus(Card.Status.Frozen))
        {
            RemoveStatus(Card.Status.Burn);
            RemoveStatus(Card.Status.Frozen);

            // "coldsnap" damage
            hp -= 5f;
        }

        // If has both attack up and down, cancel out
        if (HasStatus(Card.Status.Attack_Up) && HasStatus(Card.Status.Attack_Down))
        {
            RemoveStatus(Card.Status.Attack_Up);
            RemoveStatus(Card.Status.Attack_Down);
        }

        // If has both defense up and down, cancel out
        if (HasStatus(Card.Status.Defense_Up) && HasStatus(Card.Status.Defense_Down))
        {
            RemoveStatus(Card.Status.Defense_Up);
            RemoveStatus(Card.Status.Defense_Down);
        }

        if (hp <= 0f || dyingTimer < timeToDeath)
        {
            if (dyingTimer == timeToDeath)
                die();

            dyingTimer -= 1;
            if (dyingTimer <= 0)
            {
                // actually despawn
                cbDespawned(this);
            }
            else
            {
                // just shrinky dink
                transform.localScale = new Vector3(1, 1, 1) * ((float)dyingTimer / (float)timeToDeath);
            }
        }
        else
        {
            hpBar.Amount = hp;
            hpBar.gameObject.SetActive(hp < baseHP);
        }
    }

    void die()
    {
        if (animator != null)
            animator.SetBool("Dead", true);

        if(cbDied != null)
            cbDied(this);
    }

    /// <summary>
    /// Called when the tower is hovered over by the user's mouse cursor. Sets hovered to true and calls hovered callback
    /// </summary>
    public void Hover()
    {
        hovered = true;
        if (cbHovered != null)
            cbHovered(this);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when tower takes damage from usually a projectile
    /// </summary>
    /// <param name="damage"></param>
    public void DirectDamage(float damage, bool isProjectile = false)
    {
        float damageModifier = 1f;

        if (HasStatus(Card.Status.Defense_Up))
            damageModifier *= 0.5f;
        if (HasStatus(Card.Status.Defense_Down))
            damageModifier *= 1.5f;

        hp -= damage * damageModifier;
    }

    /// <summary>
    /// Call when the tower collides with a projectile.
    /// </summary>
    void projectileDamage(ProjectileController projectile)
    {
        DirectDamage(projectile.GetDamage(this), true);
        projectile.Hit();
    }

    /// <summary>
    /// Add a new status to the list or continue its duration if its already present
    /// </summary>
    public void AddStatus(Card.Status status, float duration)
    {
        // if doesnt already have status or existing status has shorter duration
        if (!statusDuration.ContainsKey(status) || statusDuration[status] < duration)
            statusDuration[status] = duration;
    }

    /// <summary>
    /// Remove a status from this object if it is active
    /// </summary>
    public void RemoveStatus(Card.Status status)
    {
        statusDuration.Remove(status);
    }

    public bool HasStatus(Card.Status status)
    {
        return statusDuration.ContainsKey(status);
    }

    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileColliding = trigger.transform.parent.GetComponent<ProjectileController>();
        if (projectileColliding != null && projectileColliding.isEvil)
            projectileDamage(projectileColliding);
    }
}