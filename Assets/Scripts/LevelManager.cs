using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Level))]
public class LevelManager : MonoBehaviour
{
    private Level level;

    private float nextDebris;
    private float nextIsland;
    private float nextCloud;
    private float nextEnemy;
    private float nextEnemyTower;

    private Vector2 spawnerTop;
    private Vector2 spawnBot;
    private LineRenderer lineRenderer;

    void Start()
    {
        level = GameManager.Instance.level;

        lineRenderer = GetComponent<LineRenderer>();

        //first spawn timers
        nextDebris = 2f;
        nextCloud = 3f;
        nextIsland = 3f;
        nextEnemy = 10f;
        nextEnemyTower = 12f;
    }

    void Update()
    {
        // HandleClouds();
        // HandleIslands();
        // HandleDebris();
        // HandleEnemies();
        // HandleEnemyTowers();

        spawnerTop = new Vector2(GameManager.Instance.Mainland.transform.position.x + 300f, GameManager.Instance.Mainland.transform.position.y + 300f);
        spawnBot = new Vector2(GameManager.Instance.Mainland.transform.position.x + 300f, GameManager.Instance.Mainland.transform.position.y - 300f);
        lineRenderer.SetPosition(0, spawnerTop);
        lineRenderer.SetPosition(1, spawnBot);
    }

    private void HandleEnemyTowers()
    {
        if (Time.time < nextEnemyTower)
            return;

        StukaTower stukaTower = Instantiate(level.enemyTowerPrefab, GetSpawnPosition(), Quaternion.Euler(new Vector3(0, 0, 0))) as StukaTower;

        float cooldown = 1f / level.enemySpawnCurve.Evaluate(GameManager.Instance.LevelProgress);

        nextEnemyTower = Time.time + cooldown;
    }

    private void HandleEnemies()
    {
        if (Time.time < nextEnemy)
            return;

        int era = GameManager.Instance.EraManager.CurrentEra;

        Stuka stuka = Instantiate(level.enemyPrefabs[era], GetSpawnPosition(), Quaternion.Euler(new Vector3(0,0,90f))) as Stuka;
        
        float cooldown = 1f / level.enemySpawnCurve.Evaluate(GameManager.Instance.LevelProgress);

        nextEnemy = Time.time + cooldown;
    }

    private void HandleClouds()
    {
        if (Time.time < nextCloud)
            return;

        int rand = Random.Range(0, level.cloudPrefabs.Length);
        Cloud cloud = Instantiate(level.cloudPrefabs[rand], GetSpawnPosition(), Quaternion.identity) as Cloud;
        cloud.SetVelocity(new Vector2(-7, 2f));

        float cooldown = 1f / level.cloudCurve.Evaluate(GameManager.Instance.LevelProgress);

        nextCloud = Time.time + cooldown;
    }

    private void HandleIslands()
    {
        if (Time.time < nextIsland)
            return;

        int rand = Random.Range(0, level.cloudPrefabs.Length);

        GameObject islandGO = Instantiate(level.islandPrefabs[rand], GetSpawnPosition(), Quaternion.identity) as GameObject;
        islandGO.GetComponent<Rigidbody2D>().AddForce(new Vector2(-60f, 0), ForceMode2D.Impulse);

        float cooldown = 1f / level.islandCurve.Evaluate(GameManager.Instance.LevelProgress);

        nextIsland = Time.time + cooldown;
    }

    private void HandleDebris()
    {
        if (Time.time < nextDebris)
            return;

        int rand = Random.Range(0, level.debrisPrefabs.Length);

        GameObject debrisGO = Instantiate(level.debrisPrefabs[rand], GetSpawnPosition(), Quaternion.identity) as GameObject;
        Rigidbody2D debrisBody = debrisGO.GetComponent<Rigidbody2D>();
        debrisBody.AddForce(new Vector2(-40f, 0), ForceMode2D.Impulse);
        debrisBody.AddTorque(25f, ForceMode2D.Impulse);

        float cooldown = 1f / level.debrisCurve.Evaluate(GameManager.Instance.LevelProgress);

        nextDebris = Time.time + cooldown;
    }

    private Vector2 GetSpawnPosition()
    {
        return Vector2.Lerp(spawnerTop, spawnBot, Random.Range(0f, 1f));
    }
}
