using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WorldController : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject initialManaTowerPrefab;
    public StageData stageData;
    public WorldInfo worldInfo;

    /// <summary>
    /// The game event this script will invoke at the beginning of a round
    /// </summary>
    public GameEvent roundBegin;

    public CameraController cameraController;
    public EnemySpawnController enemySpawnController;
    public TargetSelectionController targetSelectionController;
    public CardGameController cardGameController;
    public MinimapController minimapCameraController;

    public GameObject tilesContainer;
    public GameObject towersContainer;

    public TileController[,] Tiles;
    List<TileController> entrances = new List<TileController>();
    List<TileController> exits = new List<TileController>();

    void Awake()
    {
        worldInfo.worldController = this;
    }

    void Start()
    {
        // fill in tile objects
        Tiles = new TileController[stageData.Width, stageData.Height];
        for (int y = 0; y < stageData.Height; y++)
        {
            for (int x = 0; x < stageData.Width; x++)
            {
                // instantiate tile object
                GameObject tileObject = (GameObject)Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
                TileController tile = tileObject.GetComponent<TileController>();
                Tiles[x, y] = tile;

                // set personal variables
                tile.X = x;
                tile.Y = y;
                tile.worldController = this;
                tile.name = "Tile_" + x + "_" + y;
                Tiles[x, y].Type = stageData.Values[x,y];

                // position in space
                tile.transform.position = new Vector3(-x + stageData.Width / 2 - (float)(1 - stageData.Width % 2) / 2, 0, y - stageData.Height / 2 + (float)(1 - stageData.Height % 2) / 2);
                tile.transform.parent = tilesContainer.transform;

                // register callbacks
                tile.RegisterHoveredCB(targetSelectionController.TileHovered);

                // add to relevant lists for easy access
                if (Tiles[x, y].Type == Tile.TileType.Entrance)
                    entrances.Add(Tiles[x, y]);
                if (Tiles[x, y].Type == Tile.TileType.Exit)
                    exits.Add(Tiles[x, y]);
            }
        }

        // assign each tiles neighbors
        // has to be done after filling array as otherwise some neighbors wouldnt exist yet
        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                if (x != 0)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Left] = Tiles[x - 1, y];
                if (x != stageData.Width - 1)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Right] = Tiles[x + 1, y];
                if (y != 0)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Up] = Tiles[x, y - 1];
                if (y != stageData.Height - 1)
                    Tiles[x, y].Neighbors[Tile.TileDirection.Down] = Tiles[x, y + 1];
            }
        }

        // set up minimap
        minimapCameraController.UpdateStageData(stageData);

        // generate enemy paths
        RandomizePaths();

        // randomly generate some number of mana towers for the player to start with
        RandomizeInitialTowers();

        // begin the round
        roundBegin.Raise();
    }

    /// <summary>
    /// Set up new enemy paths and pass them to the enemy spawner.
    /// </summary>
    void RandomizePaths()
    {
        /// Most of the work is done by the PathGenerator class
        List<TileController> activeEntrances = new List<TileController>();
        PathGenerator.RandomizePaths(Tiles, entrances, exits, activeEntrances);
        enemySpawnController.ActiveEntrances = activeEntrances;
    }

    /// <summary>
    /// Player has to start with some mana towers in order to place more
    /// </summary>
    void RandomizeInitialTowers()
    {
        List<TileController> raisedTiles = new List<TileController>();
        for (int x = 0; x < stageData.Width; x++)
            for (int y = 0; y < stageData.Height; y++)
                if (Tiles[x, y].Type == Tile.TileType.Raised)
                    raisedTiles.Add(Tiles[x, y]);

        raisedTiles.Shuffle();
        if(raisedTiles.Count >= 2)
            for (int i = 0; i < 2; i++)
                SpawnTower(initialManaTowerPrefab, raisedTiles[i], Tile.TileDirection.Right);
    }

    /// <summary>
    /// Creates a new tower object with the parameters as settings and places it in the world
    /// </summary>
    public void SpawnTower(GameObject towerPrefab, TileController parentTile, Tile.TileDirection facingDirection)
    {
        Vector3 towerPosition = parentTile.transform.position;
        GameObject towerObject = SimplePool.Spawn(towerPrefab, towerPosition, Quaternion.identity);
        TowerController towerController = towerObject.GetComponent<TowerController>();

        parentTile.PresentTower = towerController;
        towerController.ParentTile = parentTile;
        towerController.FacingDirection = facingDirection;
        towerController.PerformBehaviours = true;
        towerObject.transform.SetParent(towersContainer.transform);

        towerController.RegisterHoveredCB(targetSelectionController.TowerHovered);

        towerController.Initiate();
    }

    public List<TileController>[] GetAreaAroundTile(TileController tile, AreaOfEffect area, Tile.TileDirection direction = Tile.TileDirection.None)
    {
        List<TileController>[] arr = new List<TileController>[area.Max + 1];

        for (int i = 0; i <= area.Max; i++)
            arr[i] = new List<TileController>();

        int centerX = area.Width / 2;
        int centerY = area.Height / 2;
        int[,] areaValues = area.GetRotated(direction);

        for (int x = 0; x < stageData.Width; x++)
        {
            for (int y = 0; y < stageData.Height; y++)
            {
                // converting the tile coordinate into the coordinate within the area
                int adjustedX = (x - tile.X) + centerX;
                int adjustedY = (y - tile.Y) + centerY;

                if (adjustedX >= 0 && adjustedX < area.Width && adjustedY >= 0 && adjustedY < area.Height)
                {
                    int category = areaValues[adjustedX, adjustedY];
                    if(category != -1)
                        arr[category].Add(Tiles[x, y]);
                }
            }
        }

        return arr;
    }
}