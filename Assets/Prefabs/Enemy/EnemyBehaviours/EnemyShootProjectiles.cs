using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Behaviour that shoots a projectile
/// </summary>
public class EnemyShootProjectiles : EnemyBehaviour
{
    /// <summary>
    /// The projectile to be shot by the tower
    /// </summary>
    public GameObject ProjectilePrefab;

    /// <summary>
    /// The animator of the model that represents the tower
    /// </summary>
    public Animator EnemyAnimator;

    /// <summary>
    /// How long to wait before firing the next bullet after the last
    /// </summary>
    public int ProjectileInterval;

    /// <summary>
    /// How long to wait when first created in addition to projectile interval
    /// </summary>
    public int InitialWait = 0;

    /// <summary>
    /// How long before the projectile is fired to trigger the animation
    /// </summary>
    public int AnimationWait = 5;

    /// <summary>
    /// Base amount of damage for projectiles to do on hit
    /// </summary>
    public float baseDamage = 5f;

    /// <summary>
    /// Y position to spawn the projectile at
    /// </summary>
    public float ProjectileY = .5f;

    /// <summary>
    /// Amount of time units left until the tower fires a new projectile
    /// </summary>
    public int projectileTimer;

    /// <summary>
    /// The rotation clockwise away from the tower's facing angle to shoot the projectile from.
    /// </summary>
    public float rotation = 0f;

    /// <summary>
    /// Enemy range if they use projectile attacks
    /// </summary>
    public float range;

    /// <summary>
    /// The displacement for the ejection point of the projectile away from the center of the tower.
    /// </summary>
    public Vector2 displacement = new Vector2(0, 0);

    protected override void Initiate()
    {
        projectileTimer = InitialWait + AnimationWait + 1;
        displacement = displacement.Rotated(-transform.localEulerAngles.y);
    }

    protected override void Behave()
    {
        TowerController targetTower = detectTower();

        if (targetTower != null)
        {
            projectileTimer -= 1;

            if (projectileTimer == AnimationWait)
            {
                EnemyAnimator.SetTrigger("Attack");
            }

            if (projectileTimer < 0)
            {
                projectileTimer += ProjectileInterval;
                SpawnProjectile();
            }
        }
    }

    /// <summary>
    /// Used by ranged enemies to detect if towers are in range
    /// </summary>
    TowerController detectTower()
    {
        // get all TowerController components
        List<TowerController> towers = new List<TowerController>(GameObject.FindObjectsOfType<TowerController>());
        // only include TowerController components that are activated
        towers = new List<TowerController>(towers.Where(tower => tower.PerformBehaviours));
        // sort them by their distance
        towers = new List<TowerController>(towers.OrderBy(tower => towerDistance(tower)));

        // if there are no active towers or the closest is out of our range, return null (because no tower)
        if (towers.Count <= 0 || towerDistance(towers[0]) > range)
            return null;

        return towers[0];
    }

    float towerDistance(TowerController tower)
    {
        return Vector3.Distance(tower.transform.position, this.transform.position);
    }

    /// <summary>
    /// Creates a projectile object and shoots it
    /// </summary>
    public void SpawnProjectile()
    {
        Vector3 projectilePosition = new Vector3(transform.position.x + displacement.x, ProjectileY, transform.position.z + displacement.y);
        GameObject projectileObject = Instantiate(ProjectilePrefab, projectilePosition, Quaternion.identity);
        ProjectileController projectileController = projectileObject.GetComponent<ProjectileController>();

        projectileController.isEvil = true;
        projectileController.SetRotation(transform.localEulerAngles.y + rotation);
        projectileController.baseDamage = baseDamage;
    }
}
