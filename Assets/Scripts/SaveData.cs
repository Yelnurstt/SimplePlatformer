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
    // 2: one "volume" became musicVolume + sfxVolume (week 12)
    public const int CurrentVersion = 2;

    public int version = CurrentVersion;

    // Progress: which levels are open. Level 1 is always open.
    public List<int> unlockedLevels = new List<int> { 1 };

    // Best score: best time per level
    public List<LevelResult> results = new List<LevelResult>();

    // Settings: slider values 0..1 (AudioManager turns them into decibels)
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
}
