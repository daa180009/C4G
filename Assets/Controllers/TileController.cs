using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileController : MonoBehaviour
{
    Material cubeMaterial;
    public WorldController worldController;

    /// <summary>
    /// The color of the tile based on its current TileType. Multiplied with other values each frame
    /// </summary>
    Color baseColor;

    public int X { set; get; }
    public int Y { set; get; }
    /// <summary>
    /// Whether the X + Y of the tile is even or odd. Used to create the checker pattern of tiles
    /// </summary>
    public bool Parity
    {
        get
        {
            return (X + Y) % 2 == 0;
        }
    }

    public bool UseParity = true;

    /// <summary>
    /// The main cube of the tile graphic
    /// </summary>
    public GameObject Cube;
    /// <summary>
    /// The meshes used to display the enemy path direction of the tower
    /// </summary>
    public GameObject StraightPath;
    public GameObject CornerPath;

    public Tile.TileType Type = Tile.TileType.Floor;

    /// <summary>
    /// The material for the enemy path display going from one side of the tile to one not straight across
    /// </summary>
    public Material CornerMaterial;
    /// <summary>
    /// The material for the enemy path display for going straight across the tile
    /// </summary>
    public Material AcrossMaterial;

    /// <summary>
    /// Set to true when the tile is hovered over which is then monitored in the next FixedUpdate()
    /// </summary>
    bool hovered = false;
    /// <summary>
    /// Set high when pinged (mostly for debug)
    /// </summary>
    int pinged = 0;

    Action<TileController> cbHovered;
    /// <summary>
    /// Register a method to be called when the tile is hovered over by the user's mouse cursor
    /// </summary>
    public void RegisterHoveredCB(Action<TileController> cb){ cbHovered += cb; }

    /*
    /// <summary>
    /// The TileType of the tile. Setting this publically automatically updates the tile's graphics accordingly
    /// </summary>
    public Tile.TileType Type
    {
        get { return type; }
        set
        {
            type = value;
            UpdateTypeDisplay(); ;
        }
    }
    */

    /// <summary>
    /// Public accessor for the height of the tile (based on its tile type)
    /// </summary>
    public float Height;

    (Tile.TileDirection from, Tile.TileDirection to) directions = (Tile.TileDirection.None, Tile.TileDirection.None);
    /// <summary>
    /// The pair of TileDirections in and out of the tile for an enemy path going over the tile. Setting this automatically updates the tile's enemy path display graphics accordingly
    /// </summary>
    public (Tile.TileDirection from, Tile.TileDirection to) Directions
    {
        get { return directions; }
        set
        {
            directions = value;
            UpdateDirectionDisplay();
        }
    }

    /// <summary>
    /// The tiles in each direction from the tile.
    /// </summary>
    public Dictionary<Tile.TileDirection, TileController> Neighbors = new Dictionary<Tile.TileDirection, TileController>();

    /// <summary>
    /// The tower currently placed on this tile. Should only support one at a time probably
    /// </summary>
    public TowerController PresentTower;

    // Start is called before the first frame update
    void Start()
    {
        cubeMaterial = Cube.GetComponent<Renderer>().material; //new Material(Shader.Find("Standard"));
                                                               //cubeMaterial.mainTextureOffset = new Vector2(UnityEngine.Random.Range(-.1f, .1f), 0f);

        if ((Tile.TileType.Floor | Tile.TileType.Entrance | Tile.TileType.Exit).HasFlag(Type))
        { 
            cubeMaterial.mainTextureOffset = new Vector2(UnityEngine.Random.Range(0, 5) / 6f, UnityEngine.Random.Range(0, 2) / 3f);
        }

        baseColor = cubeMaterial.color;
        switch (Type)
        {
            case Tile.TileType.Entrance: baseColor = Color.green; break;
            case Tile.TileType.Exit: baseColor = Color.red; break;
            case Tile.TileType.Raised: baseColor = new Color(.5f, .2f, 0); break;
        }

        if (Parity && UseParity)
            baseColor *= new Color(.5f, .5f, .5f);
    }

    void FixedUpdate()
    {
        cubeMaterial.color = baseColor;
        if(hovered)
            cubeMaterial.color *= new Color(.3f, .3f, 1);
        if(pinged >= 0)
            cubeMaterial.color *= new Color(1, .3f, .3f);

        hovered = false;
        pinged -= 1;
    }

    /*
    /// <summary>
    /// Update the visual type of the tile, including the cube height and coloured
    /// </summary>
    void UpdateTypeDisplay()
    {
        baseColor = tileTypeColors[type];

        // increase height of cube and adjust its position
        Cube.transform.localScale = new Vector3(1f, Height, 1f); ;
        Cube.transform.localPosition = new Vector3(0, Height / 2, 0);

        // increase height of visual elements above cube oso they are not inside
        DirectionsDisplay.transform.localPosition = new Vector3(0, Height + .001f, 0);

        // darken colour for every other tile to create checker pattern
        if (Parity)
            baseColor *= new Color(.8f, .8f, .8f);
    }*/

    /// <summary>
    /// Update the visual directions display of the tile to show enemy paths
    /// </summary>
    void UpdateDirectionDisplay()
    {
        if ((Tile.TileType.Wall | Tile.TileType.Raised | Tile.TileType.Barrier).HasFlag(Type))
            return;

        StraightPath.SetActive(false);
        CornerPath.SetActive(false);
        if (directions.from != Tile.TileDirection.None && directions.to != Tile.TileDirection.None)
        {
            float baseRotation = 0f;

            // Tile.TileDirection is a bit flag enum, so you can bitwise-OR them like this
            // which allows us to ignore which one is from and which one is to, since it doesn't matter here
            switch (directions.from | directions.to)
            {
                case Tile.TileDirection.Left | Tile.TileDirection.Right:
                    baseRotation = 90f;
                    StraightPath.SetActive(true);
                    break;
                case Tile.TileDirection.Up | Tile.TileDirection.Down:
                    baseRotation = 0f;
                    StraightPath.SetActive(true);
                    break;
                case Tile.TileDirection.Down | Tile.TileDirection.Left:
                    baseRotation = 180f;
                    CornerPath.SetActive(true);
                    break;
                case Tile.TileDirection.Left | Tile.TileDirection.Up:
                    baseRotation = 270f;
                    CornerPath.SetActive(true);
                    break;
                case Tile.TileDirection.Up | Tile.TileDirection.Right:
                    baseRotation =  0f;
                    CornerPath.SetActive(true);
                    break;
                case Tile.TileDirection.Right | Tile.TileDirection.Down:
                    baseRotation = 90f;
                    CornerPath.SetActive(true);
                    break;
            }

            StraightPath.transform.localEulerAngles = new Vector3(StraightPath.transform.localEulerAngles.x, baseRotation, StraightPath.transform.localEulerAngles.z);
            CornerPath.transform.localEulerAngles = new Vector3(CornerPath.transform.localEulerAngles.x, baseRotation, CornerPath.transform.localEulerAngles.z);
        }
    }

    public List<EnemyController> GetPresentEnemies()
    {
        LayerMask mask = LayerMask.GetMask("Enemy");
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, mask);

        List<EnemyController> enemiesCollided = new List<EnemyController>();
        foreach(Collider collider in hitColliders)
        {
            EnemyController enemyCollided = collider.GetComponent<EnemyController>();
            if (enemyCollided != null)
            {
                enemiesCollided.Add(enemyCollided);
            }
            else
            {
                EnemyController enemyCollidedParent = collider.transform.parent.GetComponent<EnemyController>();
                if (enemyCollidedParent != null)
                    enemiesCollided.Add(enemyCollidedParent);
            }
        }

        return enemiesCollided;
    }

    /// <summary>
    /// Get area around tower based on an area of effect
    /// </summary>
    public List<TileController>[] GetAreaAroundTile(AreaOfEffect area, Tile.TileDirection direction = Tile.TileDirection.None)
    {
        return worldController.GetAreaAroundTile(this, area, direction);
    }

    /// <summary>
    /// Used for debug mostly to temporarily colour in a tile
    /// </summary>
    public void Ping(int amt = 15)
    {
        pinged = amt;
    }

    /// <summary>
    /// Called when the tile is hovered over by the user's mouse cursor. Sets hovered to true and calls hovered callback
    /// </summary>
    public void Hover()
    {
        GetPresentEnemies();
        hovered = true;
        if(cbHovered != null)
            cbHovered(this);
    }
}
