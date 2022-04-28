using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    Tile.TileDirection facingDirection;
    public Tile.TileDirection FacingDirection
    {
        get
        {
            return facingDirection;
        }

        set
        {
            facingDirection = value;
            this.RotateToFace(value);
        }
    }

    void OnEnable()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if(GetComponent<Rigidbody>().velocity.magnitude < .01)
        {
            GetComponent<Rigidbody>().AddForce(100 * transform.forward);
        }

        TileController underTile;
        if(!detectTile(out underTile) || underTile.Type == Tile.TileType.Wall)
        { 
            Despawn();
        }
    }

    /// <summary>
    /// Detects if there is a tile beneath the project and outputs it
    /// </summary>
    bool detectTile(out TileController tile)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        LayerMask mask = LayerMask.GetMask("Tile");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            tile = hit.transform.parent.GetComponent<TileController>();
            return true;
        }

        tile = null;
        return false;
    }

    /// <summary>
    /// Destroys the projectile
    /// </summary>
    void Despawn()
    {
        SimplePool.Despawn(gameObject);
    }

    /// <summary>
    /// Called when the projectile collides wihth an enemy
    /// </summary>
    public void HitEnemy()
    {
        Despawn();
    }
}
