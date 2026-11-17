using UnityEngine;

// Keeps the save in memory while the game runs.
// Created automatically before the first scene and lives through all scenes,
// so you do NOT need to put it in any scene.
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData Data { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateOnStart()
    {
        GameObject go = new GameObject("SaveManager");
        go.AddComponent<SaveManager>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        Instance = this;
        Data = SaveSystem.Load();
        AudioListener.volume = Data.volume;
        Debug.Log("Save file: " + SaveSystem.FilePath);
    }

    public bool IsUnlocked(int levelNumber)
    {
        return Data.unlockedLevels.Contains(levelNumber);
    }

    // 0 = no result yet
    public float GetBestTime(int levelNumber)
    {
        foreach (LevelResult r in Data.results)
        {
            if (r.levelNumber == levelNumber) return r.bestTime;
        }
        return 0f;
    }

    // Called by LevelLoader when a level is finished -> save right away
    public void LevelCompleted(int levelNumber, float time)
    {
        int next = levelNumber + 1;
        if (!Data.unlockedLevels.Contains(next))
            Data.unlockedLevels.Add(next);

        LevelResult result = Data.results.Find(r => r.levelNumber == levelNumber);
        if (result == null)
        {
            Data.results.Add(new LevelResult { levelNumber = levelNumber, bestTime = time });
        }
        else if (time < result.bestTime)
        {
            result.bestTime = time;
        }

        SaveSystem.Save(Data);
        Debug.Log("Saved: level " + levelNumber + " done in " + time.ToString("0.0") + " s");
    }

    // Phone: app goes to background (home button, call). It may be killed after this.
    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save(Data);
    }

    void OnApplicationQuit()
    {
        SaveSystem.Save(Data);
    }
}
