using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class that manages spawning enemies in waves
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    public TargetSelectionController targetSelectionController;

    //debug variables
    int enemySpawnTimer = 90;

    public RoundData CurrentRound;

    int waveIndex;
    WaveData CurrentWave
    {
        get { return CurrentRound.EnemyWaves[waveIndex]; }
    }

    public List<TileController> activeEntrances;
    /// <summary>
    /// The entrances that are available to spawn enemies out of
    /// </summary>
    public List<TileController> ActiveEntrances
    {
        set { activeEntrances = value; }
    }

    /// <summary>
    /// The count of active enemies on screen
    /// </summary>
    int enemiesCount = 0;

    Action<EnemyController> cbEnemySpawned;
    /// <summary>
    /// Register a function to be called whenever the an enemy is spawned
    /// </summary>
    public void RegisterEnemySpawnedCB(Action<EnemyController> cb) { cbEnemySpawned += cb; }

    void Start()
    {
        setWave(0);
    }

    void FixedUpdate()
    {
        enemySpawnTimer += 1;
        if (enemySpawnTimer > 10)
        {
            if(attemptSpawn())
                enemySpawnTimer = 0;
        }
    }

    void setWave(int index)
    {
        waveIndex = index;

        foreach (WaveEnemyData data in CurrentWave.Enemies)
            data.Restart();
    }

    // try to spawn an enemy from the wave parameters; returns true if an enemy was spawned
    bool attemptSpawn()
    {
        if (enemiesCount > 3 || activeEntrances.Count == 0)
            return false;

        WaveEnemyData chosenEnemy = CurrentWave.Enemies[UnityEngine.Random.Range(0, CurrentWave.Enemies.Count)];
        TileController chosenEntrance = activeEntrances[UnityEngine.Random.Range(0, activeEntrances.Count)];

        if (chosenEnemy.enemiesSpawned >= chosenEnemy.enemiesCount)
            return false;

        enemySpawn(chosenEnemy.prefab, chosenEntrance);
        chosenEnemy.enemiesSpawned += 1;
        return true;
    }

    /// <summary>
    /// Spawns an enemy at a specified entrance
    /// </summary>
    void enemySpawn(GameObject enemyPrefab, TileController entrance)
    {
        Vector3 enemyPosition = new Vector3(0f, 0f, 0f);
        GameObject enemyObject = SimplePool.Spawn(enemyPrefab, enemyPosition, Quaternion.identity);
        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();

        enemyController.FromTile = entrance;

        enemyObject.transform.SetParent(this.transform);
        enemyController.RegisterDespawnedCB(OnEnemyDespawn);
        enemyController.RegisterHoveredCB(targetSelectionController.EnemyHovered);

        enemiesCount += 1;

        if(cbEnemySpawned != null)
            cbEnemySpawned(enemyController);
    }

    void OnEnemyDespawn(EnemyController enemy)
    {
        SimplePool.Despawn(enemy.gameObject);
        enemiesCount -= 1;
    }
}
