using UnityEngine;
using System.Collections;

public class LevelSettings : MonoBehaviour
{
    public bool IsLoaded { get; private set; }
    public float LevelTime { get; private set; }
    public float IslandMultiplier { get; private set; }
    public float EnemyMultiplier { get; private set; }
    public float DebrisMultiplier { get; private set; }

    public void Initialize(float levelTime, float islandMultiplier, float enemyMultiplier, float debrisMultiplier)
    {
        LevelTime = levelTime;
        IslandMultiplier = islandMultiplier;
        EnemyMultiplier = enemyMultiplier;
        DebrisMultiplier = debrisMultiplier;

        IsLoaded = true;
    }
}
