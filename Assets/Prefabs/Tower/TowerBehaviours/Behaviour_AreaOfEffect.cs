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

        List<TileController> affectedTiles = MainController.ParentTile.GetAreaAroundTile(areaOfEffect, MainController.FacingDirection)[1];

        HashSet<EnemyController> affectedEnemies = new HashSet<EnemyController>();
        foreach(TileController tile in affectedTiles)
        {
            List<EnemyController> tileEnemies = tile.GetPresentEnemies();
            foreach (EnemyController enemy in tileEnemies)
                affectedEnemies.Add(enemy);

            tile.Ping(4);
        }

        foreach(EnemyController enemy in affectedEnemies)
        {
            enemy.DirectDamage(5f);
        }
    }

    public override string GetDescription()
    {
        return "AOE";
    }
}
