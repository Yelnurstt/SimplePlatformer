using System.Collections.Generic;
using UnityEngine;

public enum LevelGoal { CollectCoinsAndExit, DefeatBoss }

// One enemy to create when the level starts
[System.Serializable]
public class EnemySpawn
{
    public GameObject prefab;   // Patroller, Shooter or Charger prefab (its stats are on the prefab)
    public Vector2 position;    // where it appears
}

// A level as data. LevelLoader reads it and builds the level:
// enemies, goal, time limit. Ground and platforms stay in the scene.
[CreateAssetMenu(fileName = "Level_01", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Info")]
    public int levelNumber = 1;           // used by the save and the level select
    public string levelName = "Level 1";  // shown in the menu
    public string sceneName = "Level1";
    public string nextScene = "Level2";   // loaded after the level is completed

    [Header("Goal")]
    public LevelGoal goal = LevelGoal.CollectCoinsAndExit;
    public int coinsToOpenExit = 3;
    public float timeLimit = 90f;         // s: time runs out -> restart
    public float fallDeathY = -6f;        // falling below this -> restart

    [Header("Enemies")]
    public List<EnemySpawn> enemies = new List<EnemySpawn>();
}
