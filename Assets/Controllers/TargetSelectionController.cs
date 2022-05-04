using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Class that moves the preview tower and manages placing the tower when the user clicks
/// </summary>
public class TargetSelectionController : MonoBehaviour
{
    public WorldController worldController;
    public GameObject handDisplayController;
    public WorldInfo worldInfo;

    /// <summary>
    /// A TowerController that is placed at the tile selected by the user as a preview
    /// </summary>
    GameObject previewTower;

    /// <summary>
    /// The type of object that needs to be selected
    /// </summary>
    Card.TargetType targetType = Card.TargetType.None;
    /// <summary>
    /// The quality to match for the target that needs to be chosen
    /// </summary>
    CardEffectQuality currentQuality;
    ResolutionInfo currentResolutionInfo;
    /// <summary>
    /// Current area to display, if any
    /// </summary>
    AreaOfEffect currentArea;

    /// <summary>
    /// Whether the player has selected enough targets, meaning they can click the Submit button and continue without selecting more
    /// </summary>
    bool allowStop;

    /// <summary>
    /// The tile that the user has hovered over most recently, 
    /// may not necessarily be a valid tile I.E. can be wrong type or already have a tower
    /// </summary>
    TileController tileHovered;
    /// <summary>
    /// The tower that the user has hovered over most recently
    /// </summary>
    TowerController towerHovered;
    /// <summary>
    /// The enemy that the user has hovered over most recently
    /// </summary>
    EnemyController enemyHovered;

    /// <summary>
    /// Time since the last time an object was hovered over 
    /// </summary>
    int lastHovered = 0;

    /// <summary>
    /// The direction that the user has input for the preview tower to face,
    /// also used when the actual tower is placed downo
    /// </summary>
    Tile.TileDirection previewDirection = Tile.TileDirection.Up;

    /// <summary>
    /// Whether selection is enabled; is disabled while hovering other UI elements
    /// </summary>
    bool canPlace = true;

    Action<TileController, Tile.TileDirection> cbSelectTile;
    /// <summary>
    /// Register a method to be called when the user selects a tile
    /// </summary>
    public void RegisterSelectTileCB(Action<TileController, Tile.TileDirection> cb) { cbSelectTile += cb; }

    Action<TowerController> cbSelectTower;
    /// <summary>
    /// Register a method to be called when the user selects a tower
    /// </summary>
    public void RegisterSelectTowerCB(Action<TowerController> cb) { cbSelectTower += cb; }

    Action<EnemyController> cbSelectEnemy;
    /// <summary>
    /// Register a method to be called when the user selects a tower
    /// </summary>
    public void RegisterSelectEnemyCB(Action<EnemyController> cb) { cbSelectEnemy += cb; }

    void Start()
    {
        // set up function to receive info on whether the hand display is active (if so, disable user input on stage)
        handDisplayController.GetComponent<HandDisplayController>().RegisterHoveredChanged(OnHandDisplayHoveredChanged);
    }

    void FixedUpdate()
    {
        // we reset the last tile hovered if the user hasnt hovered recently since that likely means their mouse is off screen
        lastHovered -= 1;
        if(lastHovered == 0)
        {
            tileHovered = null;
            towerHovered = null;
            enemyHovered = null;
        }
    }

    public void StartTowerPreview(GameObject towerPrefab, ResolutionInfo resolutionInfo)
    {
        if(previewTower != null)
        {
            Destroy(previewTower);
        }

        previewTower = (GameObject)Instantiate(towerPrefab, Vector3.zero, Quaternion.identity);
        previewTower.transform.parent = this.transform;
        previewTower.GetComponent<TowerController>().PerformBehaviours = false;
        previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;

        StartTargetSelection(Card.TargetType.Tiles, null, resolutionInfo, false);
    }

    public void StartTargetSelection(Card.TargetType type, CardEffectQuality quality, ResolutionInfo resolutionInfo, bool shouldAllowStop = false, AreaOfEffect affectedArea = null)
    {
        targetType = type;
        currentQuality = quality;
        currentResolutionInfo = resolutionInfo;
        allowStop = shouldAllowStop;
        currentArea = affectedArea;
    }

    /// <summary>
    /// Submit the currently hovered object to the resolution controller
    /// </summary>
    void confirmSelection(bool noSelection = false)
    {
        switch (targetType)
        {
            case Card.TargetType.Tiles:
                if (tileHovered == null && !noSelection)
                    break;
                targetType = Card.TargetType.None;
                Destroy(previewTower);
                previewTower = null;
                currentArea = null;
                if (cbSelectTile != null)
                    cbSelectTile(noSelection ? null : tileHovered, previewDirection);
                tileHovered = null;
                break;
            case Card.TargetType.Towers:
                if (towerHovered == null && !noSelection)
                    break;
                targetType = Card.TargetType.None;
                currentArea = null;
                if (cbSelectTower != null)
                    cbSelectTower(noSelection ? null : towerHovered);
                towerHovered = null;
                break;
            case Card.TargetType.Enemies:
                if (enemyHovered == null && !noSelection)
                    break;
                targetType = Card.TargetType.None;
                currentArea = null;
                if (cbSelectEnemy != null)
                    cbSelectEnemy(noSelection ? null : enemyHovered);
                enemyHovered = null;
                break;
        }
    }

    /// <summary>
    /// Called when the user hovers over a tile, sets it as the most recently hovered tile and moves the preview tower
    /// </summary>
    public void TileHovered(TileController tile)
    {
        if (targetType != Card.TargetType.Tiles)
            return;

        // i know its bad
        if ((currentQuality == null || currentQuality.CheckQuality(tile, worldInfo, currentResolutionInfo)) && (previewTower == null || !(Tile.TileType.Wall | Tile.TileType.Barrier).HasFlag(tile.Type)))
        {
            tileHovered = tile;
            AreaOfEffect previewArea = currentArea;

            previewTower?.SetActive(true);
            if(previewTower != null)
            {
                previewTower.GetComponent<TowerController>().transform.position = tile.transform.position + new Vector3(0, tile.Height, 0);

                if(previewTower.GetComponent<Behaviour_AreaOfEffect>() != null)
                {
                    previewArea = previewTower.GetComponent<Behaviour_AreaOfEffect>().areaOfEffect;
                }
            }

            if (previewArea != null)
            {
                List<TileController>[] tilesInArea = worldController.GetAreaAroundTile(tileHovered, previewArea, previewDirection);
                foreach (TileController affectedTile in tilesInArea[1])
                    affectedTile.Ping(1);
            }
        }
        else
        {
            tileHovered = null;
            previewTower?.SetActive(false);
        }
        lastHovered = 5;
    }

    public void TowerHovered(TowerController tower)
    {
        if (targetType != Card.TargetType.Towers)
            return;

        towerHovered = tower;
        lastHovered = 5;
    }

    public void EnemyHovered(EnemyController enemy)
    {
        if (targetType != Card.TargetType.Enemies)
            return;

        enemyHovered = enemy;
        lastHovered = 5;
    }

    public void OnTowerRotateCW()
    {
        previewDirection = previewDirection.RotatedCW();

        if(previewTower != null)
            previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;
    }

    public void OnTowerRotateCCW()
    {
        previewDirection = previewDirection.RotatedCCW();

        if (previewTower != null)
            previewTower.GetComponent<TowerController>().FacingDirection = previewDirection;
    }

    void OnSelect(InputValue value)
    {
        if(canPlace)
            confirmSelection();
    }

    public void OnSubmitTargets()
    {
        Debug.Log(allowStop);
        if(allowStop)
            confirmSelection(true);
    }

    /// <summary>
    /// Called when the hand display is hovered over or unhovered
    /// </summary>
    /// <param name="isHovered"></param>
    void OnHandDisplayHoveredChanged(bool isHovered)
    {
        canPlace = !isHovered;
    }
}
