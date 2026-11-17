using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// The SAME script is in Level1, Level2 and BossLevel.
// Only the LevelData asset is different: it says which enemies to create,
// how many coins open the exit, the time limit and the next scene.
public class LevelLoader : MonoBehaviour
{
    // Other scripts (player, exit, boss) ask the current level through this
    public static LevelLoader Instance { get; private set; }

    [SerializeField] private LevelData level;      // Level_01, Level_02 or Level_03
    [SerializeField] private TMP_Text timerText;   // "Time: 87"

    public LevelData Level => level;

    private float timeLeft;
    private bool finished;
    private string bestText = "";

    void Awake()
    {
        Instance = this;

        // Build the level from data: create every enemy at its position
        foreach (EnemySpawn spawn in level.enemies)
        {
            if (spawn.prefab != null)
                Instantiate(spawn.prefab, spawn.position, Quaternion.identity);
        }

        timeLeft = level.timeLimit;

        // Best time from save.json, e.g. "  Best: 34s"
        float best = SaveManager.Instance.GetBestTime(level.levelNumber);
        if (best > 0f)
            bestText = "  Best: " + Mathf.CeilToInt(best) + "s";
    }

    void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(Mathf.Max(0f, timeLeft)) + bestText;

        if (timeLeft <= 0f)
        {
            finished = true;
            Debug.Log("Time is up!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Called by the ExitGate (coins + exit) or by the boss (boss defeated)
    public void CompleteLevel()
    {
        if (finished) return;
        finished = true;

        Debug.Log(level.levelName + " complete!");

        // Save right after the level: opens the next level, keeps best time
        float timeUsed = level.timeLimit - timeLeft;
        SaveManager.Instance.LevelCompleted(level.levelNumber, timeUsed);

        SceneManager.LoadScene(level.nextScene);
    }
}
