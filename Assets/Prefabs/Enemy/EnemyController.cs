using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    public GameObject enemyModel;
    public HPBarController hpBar;
    public ParticleSystem fireParticles;
    public ParticleSystem iceParticles;

    /// <summary>
    /// The behaviours for the enemy to carry out, such as spawning materials, generating mana, etc.
    /// </summary>
    EnemyBehaviour[] behaviours;

    bool performBehaviours = true;
    /// <summary>
    /// Whether or not the enemy should have its behaviours active. 
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
    /// The HP the enemy starts at
    /// </summary>
    public float baseHP;
    /// <summary>
    /// The enemy's health points. When they reach 0, FixedUpdate() will despawn the enemy
    /// </summary>
    float hp = 1;

    /// <summary>
    /// The speed the enemy will move at across the stage if they have a variance of 0
    /// </summary>
    public float BaseSpeed = 1f;
    /// <summary>
    /// The the maximum additional speed an enemy can spawn with randomly
    /// </summary>
    public float SpeedVariance = 1f;

    /// <summary>
    /// How much life for the player to lose when the enemy gets through their defenses
    /// </summary>
    public int LifeLossAmount = 2;

    /// <summary>
    /// Specifier of the enemy type. used to call the correct animation for each enemy.
    /// </summary>
    public string enemyType;

    /// <summary>
    /// Association of each status effect with its current duration
    /// </summary>
    Dictionary<Card.Status, float> statusDuration = new Dictionary<Card.Status, float>();

    /// <summary>
    /// The tile the enemy started wallking from
    /// </summary>
    public TileController fromTile = null;
    /// <summary>
    /// The tile the enemy is walking to currently
    /// </summary>
    public TileController toTile = null;
    /// <summary>
    /// The last tile that the enemy stopped at, and which it is travelling from. 
    /// Setting this publically automatically changes its destination tile if valid
    /// </summary>

    public TileController FromTile
    {
        set
        {
            fromTile = value;
            transform.position = value.transform.position;

            if (value.Directions.to != Tile.TileDirection.None)
                if (value.Neighbors.ContainsKey(fromTile.Directions.to))
                {
                    toTile = value.Neighbors[fromTile.Directions.to];
                }
                else
                {
                    cbDespawned(this, true);
                }
        }
    }

    /// <summary>
    /// The percentage distance along the path from the last to next tile that the enemy has travelled
    /// </summary>
    float distance = 0f;
    /// <summary>
    /// When enemy is created, give it random additioanl speed so that enemies don''t all move the same exactly
    /// </summary>
    float randomSpeed;

    Action<EnemyController, bool> cbDespawned;
    /// <summary>
    /// Register a function to be called when this enemy is set to despawn from being destroyed
    /// </summary>
    public void RegisterDespawnedCB(Action<EnemyController, bool> cb) { cbDespawned -= cb; cbDespawned += cb; }

    /// <summary>
    /// Set to true when the enemy is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    /// <summary>
    /// The current tower that the enemy is blocked from moving forward by and which it will attack.
    /// </summary>
    public TowerController currentTowerColliding;

    Action<EnemyController> cbHovered;
    /// <summary>
    /// Register a method to be called when the enemy is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<EnemyController> cb) { cbHovered += cb; }

    void OnEnable()
    {
        hp = baseHP;
        distance = 0f;
        cbDespawned = null;
        randomSpeed = (UnityEngine.Random.Range(0, SpeedVariance) + BaseSpeed) * .01f;
        hpBar.Maximum = baseHP;

        //Walker running animation
        if (this.enemyType == "Walker")
            enemyModel.GetComponent<Animator>().Play("infantry_03_run");

        //Wizard running animation
        if (this.enemyType == "Wizard")
            enemyModel.GetComponent<Animator>().Play("BattleWalkForward");

        behaviours = GetComponents<EnemyBehaviour>();

        foreach (EnemyBehaviour behaviour in behaviours)
        {
            behaviour.OnInitiate();
        }
    }

    void FixedUpdate()
    {
        float speedModifier = 1f;
        if (HasStatus(Card.Status.Frozen))
            speedModifier *= .1f;

            if (toTile == null || fromTile == null)
        {
            cbDespawned(this, true);
        }

        if (currentTowerColliding == null)
        {
            // move
            if (Vector2.Distance(toTile.FlatPosition(), this.FlatPosition()) < .02f)
            {
                FromTile = toTile;
                distance = 0f;
            }

            distance += randomSpeed * speedModifier;

            Vector2 flatPosition = Vector2.Lerp(fromTile.FlatPosition(), toTile.FlatPosition(), distance);
            transform.position = new Vector3(flatPosition.x, 0, flatPosition.y);

            this.RotateToFace(fromTile.Directions.to);
            hpBar.transform.localPosition = (new Vector3(-.25f, 0, .25f)).Rotated(fromTile.Directions.to);
        }
        else
        {
            // attack tower
            currentTowerColliding.DirectDamage(GetMeleeDamage() * Time.deltaTime * speedModifier);
        }

        /// Count down status durations
        Card.Status[] activeStatuses = statusDuration.Keys.ToArray();
        foreach(Card.Status status in activeStatuses)
        {
            statusDuration[status] -= Time.deltaTime;
            if (statusDuration[status] < 0f)
                statusDuration.Remove(status);
        }

        // Deal constant damage if burning
        if(HasStatus(Card.Status.Burn))
        {
            hp -= 1f * Time.deltaTime;
        }

        // If has both burn and freeze then remove both and take damage
        if(HasStatus(Card.Status.Burn) && HasStatus(Card.Status.Frozen))
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

        /// Make enemy appear larger if currently hovered
        if (hovered)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;

        // Despawn the enemy if health below 0
        if (hp <= 0f)
        {
            cbDespawned(this, false);
        }
        else
        {
            hpBar.Amount = hp;
            hpBar.gameObject.SetActive(hp < baseHP);
        }
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

    /// <summary>
    /// Call when something like a card effect deals damage directly to the enemy, rather than through a projectile
    /// </summary>
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
    /// Returns the amount of damage the enemy should deal per second to intercepting targets
    /// </summary>
    /// <returns></returns>
    public float GetMeleeDamage()
    {
        float damageAmount = 1f;

        if (HasStatus(Card.Status.Attack_Up))
            damageAmount *= 1.5f;
        if (HasStatus(Card.Status.Attack_Down))
            damageAmount *= 0.5f;

        return damageAmount;
    }

    /// <summary>
    /// Call when the enemy collides with a projectile.
    /// </summary>
    void projectileDamage(ProjectileController projectile)
    {
        DirectDamage(projectile.GetDamage(this), true);
        projectile.Hit();
    }

    void towerStartAttacking(TowerController tower)
    {
        currentTowerColliding = tower;
        currentTowerColliding.RegisterDespawnedCB(towerStopAttacking);

        //Walker attack animation
        if (this.enemyType == "Walker")
            enemyModel.GetComponent<Animator>().Play("infantry_04_attack_A");

        //Wizard attack animation
        if (this.enemyType == "Wizard")
            enemyModel.GetComponent<Animator>().SetBool("Attacking", true);
    }

    void towerStopAttacking(TowerController tower)
    {
        if (this == null)
            return;

        if (tower == currentTowerColliding)
        {
            if (currentTowerColliding != null)
            {
                currentTowerColliding.UnregisterDespawnedCB(towerStopAttacking);
                currentTowerColliding = null;
            }

            //Walker running animation
            if (this.enemyType == "Walker")
                enemyModel.GetComponent<Animator>().Play("infantry_03_run");

            //Wizard running animation
            if (this.enemyType == "Wizard")
                enemyModel.GetComponent<Animator>().SetBool("Attacking", false);
        }
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

    /// <summary>
    /// Returns true of the given status is active on this object
    /// </summary>
    public bool HasStatus(Card.Status status)
    {
        return statusDuration.ContainsKey(status);
    }

    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileColliding = trigger.transform.parent.GetComponent<ProjectileController>();
        if (projectileColliding != null && !projectileColliding.isEvil)
            projectileDamage(projectileColliding);

        TowerController towerColliding = trigger.GetComponent<TowerController>();
        if (towerColliding != null)
            towerStartAttacking(towerColliding);
    }

    public void OnTriggerExit(Collider trigger)
    {
        TowerController towerColliding = trigger.GetComponent<TowerController>();
        towerStopAttacking(towerColliding);
    }
}