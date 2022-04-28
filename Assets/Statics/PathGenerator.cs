using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class to do the work of setting up enemy paths for the tile collection
/// Really messy and relies on luck to do it well, but we might not use random paths anyways so
/// we can optimize it better later if needed, otherwise its Good enough
/// </summary>
public static class PathGenerator
{
    /// <summary>
    /// Sets the enemy travel paths at random. Always tries to make exactly three paths. Uses GeneratePath().
    /// </summary>
    public static void RandomizePaths(TileController[,] tiles, List<TileController> entrances, List<TileController> exits, List<TileController> activeEntrances)
    {
        int totalSuccesses = 0;
        int totalAttempts = 0;

        while (totalSuccesses != 3 && totalAttempts < 50)
        {
            totalSuccesses = 0;
            foreach (TileController tile in tiles)
                tile.Directions = (Tile.TileDirection.None, Tile.TileDirection.None);

            for (int i = 0; i < 100 && totalSuccesses < 3; i++)
                if (GeneratePath(tiles, entrances, exits))
                    totalSuccesses += 1;

            totalAttempts++;
        }

        activeEntrances.Clear();
        foreach (TileController tile in tiles)
            if (tile.Type == Tile.TileType.Entrance && tile.Directions.to != Tile.TileDirection.None)
                activeEntrances.Add(tile);
    }

    /// <summary>
    /// Create one enemy walking path for RandomizePath().
    /// </summary>
    /// <returns></returns>
    private static bool GeneratePath(TileController[,] tiles, List<TileController> entrances, List<TileController> exits)
    {
        TileController entrance = entrances[UnityEngine.Random.Range(0, entrances.Count)];
        HashSet<TileController> unvisited = new HashSet<TileController>();
        HashSet<TileController> visited = new HashSet<TileController>();
        Dictionary<TileController, Tile.TileDirection> fromDirection = new Dictionary<TileController, Tile.TileDirection>();

        foreach (TileController tile in tiles)
            unvisited.Add(tile);
        unvisited.Remove(entrance);
        visited.Add(entrance);

        Queue<TileController> queue = new Queue<TileController>();
        queue.Enqueue(entrance);

        TileController goalTile = null;
        List<TileController> path = new List<TileController>();

        bool stopSearching = false;
        while (queue.Count > 0 && !stopSearching)
        {
            TileController nextTile = queue.Dequeue();
            List<Tile.TileDirection> keys = new List<Tile.TileDirection>(nextTile.Neighbors.Keys);

            foreach (Tile.TileDirection direction in keys)
            {
                TileController tile = nextTile.Neighbors[direction];

                if (tile.Type == Tile.TileType.Exit)
                {
                    stopSearching = true;
                    fromDirection[tile] = direction.Inversed();
                    goalTile = tile;
                    path.Add(tile);
                    goalTile.Directions = (direction.Inversed(), direction);
                }
                else if (!visited.Contains(tile) 
                    && tile.Type == Tile.TileType.Floor 
                    && tile.Directions.from == Tile.TileDirection.None 
                    && UnityEngine.Random.Range(0, 5) != 0)
                {
                    visited.Add(tile);
                    unvisited.Remove(tile);
                    queue.Enqueue(tile);
                    fromDirection[tile] = direction.Inversed();
                }
            }
        }

        if (goalTile != null)
        {
            stopSearching = false;
            while (!stopSearching)
            {
                TileController nextTile = goalTile.Neighbors[fromDirection[goalTile]];
                path.Add(nextTile);

                if (nextTile.Type == Tile.TileType.Entrance)
                {
                    stopSearching = true;
                    nextTile.Directions = (fromDirection[goalTile], fromDirection[goalTile].Inversed());
                }
                else
                {
                    nextTile.Directions = (fromDirection[nextTile], fromDirection[goalTile].Inversed());
                }

                goalTile = nextTile;
            }
            path.Reverse();

            return true;
        }

        return false;
    }
}
