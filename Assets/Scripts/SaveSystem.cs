using System;
using System.IO;
using UnityEngine;

// Reads and writes save.json in Application.persistentDataPath.
// Rule: a missing or broken file must NEVER crash the game -> defaults.
public static class SaveSystem
{
    public static string FilePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    private static string TempPath =>
        Path.Combine(Application.persistentDataPath, "save.tmp");

    public static SaveData Load()
    {
        // 1. No file (first launch) -> defaults
        if (!File.Exists(FilePath))
        {
            Debug.Log("No save file yet, using defaults");
            return new SaveData();
        }

        // 2. File exists but may be broken -> try/catch
        try
        {
            string json = File.ReadAllText(FilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
                throw new Exception("file is empty");

            FixValues(data);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save file is broken, using defaults: " + e.Message);
            return new SaveData();
        }
    }

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);

            // Write to a temp file first. If the phone dies in the middle,
            // only save.tmp is broken, the old save.json is still fine.
            File.WriteAllText(TempPath, json);
            File.Copy(TempPath, FilePath, true);
            File.Delete(TempPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save: " + e.Message);
        }
    }

    public static void Delete()
    {
        if (File.Exists(FilePath)) File.Delete(FilePath);
        if (File.Exists(TempPath)) File.Delete(TempPath);
    }

    // JSON can be valid but have strange values (edited by hand, old version)
    private static void FixValues(SaveData data)
    {
        if (data.unlockedLevels == null)
            data.unlockedLevels = new System.Collections.Generic.List<int>();

        if (!data.unlockedLevels.Contains(1))
            data.unlockedLevels.Add(1);

        if (data.results == null)
            data.results = new System.Collections.Generic.List<LevelResult>();

        // Old save (version 1) has no musicVolume/sfxVolume: JsonUtility leaves
        // the default 0.8 from SaveData, so nothing else is needed here.
        data.musicVolume = Mathf.Clamp01(data.musicVolume);
        data.sfxVolume = Mathf.Clamp01(data.sfxVolume);

        if (data.version < SaveData.CurrentVersion)
            data.version = SaveData.CurrentVersion;
    }
}
