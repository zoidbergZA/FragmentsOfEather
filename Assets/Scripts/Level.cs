using UnityEngine;
using System.Collections;

public class Level : ScriptableObject
{
    public float levelTime;
    public AnimationCurve islandCurve;
    public AnimationCurve debrisCurve;
    public AnimationCurve cloudCurve;
    public AnimationCurve enemySpawnCurve;

    //prefabs
    public GameObject[] debrisPrefabs;
    public GameObject[] islandPrefabs;
    public Cloud[] cloudPrefabs;
    public Stuka[] enemyPrefabs;
    public StukaTower enemyTowerPrefab;
}
