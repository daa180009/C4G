using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_AreaOfEffect : TowerBehaviour
{

    /// <summary>
    /// The animator of the model that represents the tower
    /// </summary>
    public Animator TowerAnimator;

    /// <summary>
    /// The audio to play when shooting projectile
    /// </summary>
    public AudioSource attackNoise;

    /// <summary>
    /// How long to wait before each attack after the last
    /// </summary>
    public int AttackInterval;

    /// <summary>
    /// How long to wait when first created in addition to attack interval
    /// </summary>
    public int InitialWait = 0;

    /// <summary>
    /// How long before the attack is fired to trigger the animation
    /// </summary>
    public int AnimationWait = 5;

    /// <summary>
    /// Base amount of damage to do to enemies on hit
    /// </summary>
    public float baseDamage = 1f;

    /// <summary>
    /// Whether or not to apply a status to hit enemies
    /// </summary>
    public bool applyStatus = false;
    /// <summary>
    /// Which status to apply if so
    /// </summary>
    public Card.Status status;
    /// <summary>
    /// How long said status should last upon application
    /// </summary>
    public float duration;

    /// <summary>
    /// Whether or not to perform the AOE; if it should only be done on special occasions, turn this off
    /// </summary>
    public bool PerformAOE = true;
    /// <summary>
    /// Whether or not to perform the AOE when dying
    /// </summary>
    public bool AOEOnDeath = false;

    /// <summary>
    /// Amount of time units left until the tower attacks again
    /// </summary>
    int attackTimer;

    /// <summary>
    /// The area around the tower to deal damage
    /// </summary>
    public AreaOfEffect areaOfEffect;


    protected override void Initiate()
    {
        attackTimer = InitialWait + AnimationWait + 1;
    }

    protected override void Behave()
    {
        if (!PerformAOE)
            return;

        if (!MainController.HasStatus(Card.Status.Frozen))
            attackTimer -= 1;

        if ((Random.Range(0, 1f) < .1f) || attackTimer < AnimationWait)
            attackTimer -= 1;

        if (attackTimer == AnimationWait)
        {
            TowerAnimator.SetTrigger("Attack");
        }

        if (attackTimer < 0)
        {
            attackTimer += AttackInterval;
            Attack();
        }
    }

    /// <summary>
    /// Creates a projectile object and shoots it
    /// </summary>
    void Attack()
    {
        if (areaOfEffect == null)
            return;

        if (attackNoise != null)
            attackNoise.Play();

        List<TileController> affectedTiles = MainController.ParentTile.GetAreaAroundTile(areaOfEffect, MainController.FacingDirection)[1];

        HashSet<EnemyController> affectedEnemies = new HashSet<EnemyController>();
        foreach(TileController tile in affectedTiles)
        {
            List<EnemyController> tileEnemies = tile.GetPresentEnemies();
            foreach (EnemyController enemy in tileEnemies)
            {
                affectedEnemies.Add(enemy);
            }

            tile.Ping(4);
        }

        foreach(EnemyController enemy in affectedEnemies)
        {
            enemy.DirectDamage(GetDamage());

            if (applyStatus)
                enemy.AddStatus(status, duration);
        }
    }

    /// <summary>
    /// Returns how much damage the projectile should deal
    /// </summary>
    public float GetDamage()
    {
        float damageAmount = baseDamage;

        if (MainController.HasStatus(Card.Status.Attack_Up))
            damageAmount *= 1.5f;
        if (MainController.HasStatus(Card.Status.Attack_Down))
            damageAmount *= 0.5f;

        return damageAmount;
    }

    protected override void Died()
    {
        if (AOEOnDeath)
            Attack();
    }
}
