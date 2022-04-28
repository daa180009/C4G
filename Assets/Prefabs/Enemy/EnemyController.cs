using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// The speed the enemy will move at across the stage if they have a variance of 0
    /// </summary>
    public float BaseSpeed = 1f;
    /// <summary>
    /// The the maximum additional speed an enemy can spawn with randomly
    /// </summary>
    public float SpeedVariance = 1f;

    /// <summary>
    /// The tile the enemy started wallking from
    /// </summary>
    TileController fromTile = null;
    /// <summary>
    /// The tile the enemy is walking to currently
    /// </summary>
    TileController toTile = null;
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
                    toTile = value.Neighbors[fromTile.Directions.to];
                else
                    cbDespawned(this);
        }
    }

    /// <summary>
    /// The percentage distance along the path from the last to next tile that the enemy has travelled
    /// </summary>
    float distance = 0f;
    /// <summary>
    /// The enemy's health points. When they reach 0, FixedUpdate() will despawn the enemy
    /// </summary>
    float hp = 1;
    /// <summary>
    /// When enemy is created, give it random additioanl speed so that enemies don''t all move the same exactly
    /// </summary>
    float randomSpeed;

    Action<EnemyController> cbDespawned;
    /// <summary>
    /// Register a function to be called when this enemy is set to despawn from being destroyed
    /// </summary>
    public void RegisterDespawnedCB(Action<EnemyController> cb){ cbDespawned += cb; }

    /// <summary>
    /// Set to true when the enemy is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    Action<EnemyController> cbHovered;
    /// <summary>
    /// Register a method to be called when the enemy is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<EnemyController> cb) { cbHovered += cb; }

    void OnEnable()
    {
        hp = 1;
        distance = 0f;
        cbDespawned = null;
        randomSpeed = UnityEngine.Random.Range(0, SpeedVariance) + BaseSpeed;
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(toTile.FlatPosition(), this.FlatPosition()) < .02f)
        {
            FromTile = toTile;
            distance = 0f;
        }
        else
        {
            distance += randomSpeed;
                
            Vector2 flatPosition = Vector2.Lerp(fromTile.FlatPosition(), toTile.FlatPosition(), distance);
            transform.position = new Vector3(flatPosition.x, 0, flatPosition.y);
        }


        if (hovered)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;

        if (hp < 0f)
        {
            cbDespawned(this);
        }
    }

    /// <summary>
    /// Called when the tower is hovered over by the user's mouse cursor. Sets hovered to true and calls hovered callback
    /// </summary>
    public void Hover()
    {
        hovered = true;
        if(cbHovered != null)
            cbHovered(this);
    }

    /// <summary>
    /// Call when something like a card effect deals damage directly to the enemy, rather than through a projectile
    /// </summary>
    public void DirectDamage(float damage)
    {
        hp -= damage;
    }

    /// <summary>
    /// Call when the enemy collides with a projectile.
    /// </summary>
    void projectileDamage(ProjectileController projectile)
    {
        hp -= 2f;
        projectile.HitEnemy();
    }

    public void OnTriggerEnter(Collider trigger)
    {
        ProjectileController projectileHitBy = trigger.transform.parent.GetComponent<ProjectileController>();
        projectileDamage(projectileHitBy);
    }
}
