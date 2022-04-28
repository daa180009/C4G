using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Structure holding lists of different data type to enable
/// predicates to requst different target types
/// </summary>
public class TargetInfo
{
    public List<TileController> Tiles { get; } = new List<TileController>();
    public List<TowerController> Towers { get; } = new List<TowerController>();
    public List<EnemyController> Enemies { get; } = new List<EnemyController>();
    public List<CardController> Cards { get; } = new List<CardController>();

    public void Add(TileController tile) { Tiles.Add(tile); }
    public void Add(TowerController tower) { Towers.Add(tower); }
    public void Add(EnemyController enemy) { Enemies.Add(enemy); }
    public void Add(CardController card) { Cards.Add(card); }

    public Tile.TileDirection Direction { get; set; } = Tile.TileDirection.None;
    public bool StoppedBeforeMax = false;

    public int GetTargetCount(Card.TargetType type)
    {
        switch (type)
        {
            case Card.TargetType.Tiles:   return Tiles.Count;
            case Card.TargetType.Towers:  return Towers.Count;
            case Card.TargetType.Enemies: return Enemies.Count;
            case Card.TargetType.Cards:   return Cards.Count;
        }
        return 0;
    }

    public List<HashSet<TileController>> AOETiles { get; } = new List<HashSet<TileController>>();
    public List<HashSet<TowerController>> AOETowers { get; } = new List<HashSet<TowerController>>();
    public List<HashSet<EnemyController>> AOEEnemies { get; } = new List<HashSet<EnemyController>>();

    /// <summary>
    /// Applies the given area of effect to each of the targetted tiles
    /// </summary>
    public void AOETargetting(AreaOfEffect area, WorldInfo worldInfo, Tile.TileDirection direction = Tile.TileDirection.None)
    {
        for(int i = 0; i <= area.Max; i++)
        {
            AOETiles.Add(new HashSet<TileController>());
            AOETowers.Add(new HashSet<TowerController>());
            AOEEnemies.Add(new HashSet<EnemyController>());
        }

        foreach (TileController targetTile in Tiles)
        {
            List<TileController>[] affectedTiles = worldInfo.worldController.GetAreaAroundTile(targetTile, area, direction);

            for (int i = 0; i <= area.Max; i++)
            {
                foreach (TileController tile in affectedTiles[i])
                {
                    AOETiles[i].Add(tile);

                    if (tile.PresentTower != null)
                        AOETowers[i].Add(tile.PresentTower);

                    List<EnemyController> tileEnemies = tile.GetPresentEnemies();
                    foreach (EnemyController enemy in tileEnemies)
                        AOEEnemies[i].Add(enemy);
                }
            }
        }
    }
}
