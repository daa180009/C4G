using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TowerController : MonoBehaviour
{
    public GameObject Cube;

    /// <summary>
    /// The material of the tower's model by default. Must be stored in case it is changed to become transparent or otherwise
    /// </summary>
    Material defaultMaterial;

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

            // change towers transparency based on whether its enabled or not
            if (performBehaviours)
                Cube.GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
            else
            {
                // disabled so make transparenty
                Material material = Cube.GetComponent<MeshRenderer>().sharedMaterial;

                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;

                material.color = new Color(material.color.r, material.color.g, material.color.b, 0.1f);

                Cube.GetComponent<MeshRenderer>().material = material;
           
                
            }
        }
    }

    /// <summary>
    /// Set to true when the tower is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;

    Action<TowerController> cbHovered;
    /// <summary>
    /// Register a method to be called when the tower is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<TowerController> cb) { cbHovered += cb; }

    void OnEnable()
    {
        defaultMaterial = Cube.GetComponent<MeshRenderer>().sharedMaterial;
        behaviours = GetComponents<TowerBehaviour>();
    }

    public void Initiate()
    {
        foreach(TowerBehaviour behaviour in behaviours)
        {
            behaviour.OnInitiate();
        }
    }

    void FixedUpdate()
    {
        if(ParentTile != null)
        {
            transform.position = ParentTile.transform.position + new Vector3(0, ParentTile.Height, 0);

            if (hovered)
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            else
                transform.localScale = new Vector3(1, 1, 1);
        }
        hovered = false;
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

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
