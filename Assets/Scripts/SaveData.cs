using System.Collections.Generic;

// Best result for one level (JsonUtility cannot save a Dictionary, so we use a list)
[System.Serializable]
public class LevelResult
{
    public int levelNumber;
    public float bestTime;   // s: fastest finish
}

// Everything that goes into save.json.
// Plain C# class (not MonoBehaviour) so JsonUtility can turn it into text.
[System.Serializable]
public class SaveData
{
    public const int CurrentVersion = 1;

    public int version = CurrentVersion;

    // Progress: which levels are open. Level 1 is always open.
    public List<int> unlockedLevels = new List<int> { 1 };

    // Best score: best time per level
    public List<LevelResult> results = new List<LevelResult>();

    // Settings
    public float volume = 1f;   // 0..1
}
