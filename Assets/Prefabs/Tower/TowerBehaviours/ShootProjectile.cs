using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour that shoots a projectile
/// </summary>
public class ShootProjectile : TowerBehaviour
{
    /// <summary>
    /// The projectile to be shot by the tower
    /// </summary>
    public GameObject ProjectilePrefab;

    /// <summary>
    /// The animator of the model that represents the tower
    /// </summary>
    public Animator TowerAnimator;

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
    /// Y position to spawn the projectile at
    /// </summary>
    public float ProjectileY = .5f;

    /// <summary>
    /// Amount of time units left until the tower fires a new projectile
    /// </summary>
    int projectileTimer;

    /// <summary>
    /// The rotation clockwise away from the tower's facing angle to shoot the projectile from.
    /// </summary>
    public float rotation = 0f;

    /// <summary>
    /// The displacement for the ejection point of the projectile away from the center of the tower.
    /// </summary>
    public Vector2 displacement = new Vector2(0, 0);

    protected override void Initiate()
    {
        projectileTimer = InitialWait + AnimationWait;
        displacement = displacement.Rotated(-transform.localEulerAngles.y);

        // Preload a few objects for the projectiles shot by the tower
        SimplePool.Preload(ProjectilePrefab, 3);
    }

    protected override void Behave()
    {
        projectileTimer -= 1;

        if(projectileTimer == AnimationWait)
        {
            TowerAnimator.SetTrigger("Attack");
        }

        if (projectileTimer < 0)
        {
            projectileTimer += ProjectileInterval;
            SpawnProjectile();
        }
    }

    /// <summary>
    /// Creates a projectile object and shoots it
    /// </summary>
    void SpawnProjectile()
    {
        Vector3 projectilePosition = new Vector3(transform.position.x + displacement.x, ProjectileY, transform.position.z + displacement.y);
        GameObject projectileObject = SimplePool.Spawn(ProjectilePrefab, projectilePosition, Quaternion.identity);
        ProjectileController projectileController = projectileObject.GetComponent<ProjectileController>();
        projectileController.SetRotation(transform.localEulerAngles.y + rotation);
    }

    public override string GetDescription()
    {
        return "Projectile";
    }
}
