using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Main menu: level select (locked / unlocked from save.json),
// volume setting and "Reset progress" with a confirm panel.
public class MainMenuUI : MonoBehaviour
{
    [Header("Level select")]
    [SerializeField] private LevelData[] levels;        // Level_01, Level_02, Level_03
    [SerializeField] private Button[] levelButtons;     // one button for each level

    [Header("Volume")]
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Button volumeDownButton;
    [SerializeField] private Button volumeUpButton;
    [SerializeField] private float volumeStep = 0.1f;

    [Header("Reset progress")]
    [SerializeField] private Button resetButton;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    void Start()
    {
        confirmPanel.SetActive(false);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            LevelData level = levels[i];   // local copy for the click below
            levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(level.sceneName));
        }

        volumeDownButton.onClick.AddListener(() => ChangeVolume(-volumeStep));
        volumeUpButton.onClick.AddListener(() => ChangeVolume(volumeStep));

        resetButton.onClick.AddListener(() => confirmPanel.SetActive(true));
        noButton.onClick.AddListener(() => confirmPanel.SetActive(false));
        yesButton.onClick.AddListener(ResetProgress);

        RefreshLevels();
        RefreshVolume();
    }

    // Read the save and show each level as open or locked
    void RefreshLevels()
    {
        SaveManager save = SaveManager.Instance;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            LevelData level = levels[i];
            bool unlocked = save.IsUnlocked(level.levelNumber);

            levelButtons[i].interactable = unlocked;

            string text = level.levelName;
            if (!unlocked)
            {
                text += "\n<size=28>LOCKED</size>";
            }
            else
            {
                float best = save.GetBestTime(level.levelNumber);
                if (best > 0f)
                    text += "\n<size=28>Best: " + Mathf.CeilToInt(best) + "s</size>";
            }

            levelButtons[i].GetComponentInChildren<TMP_Text>().text = text;
        }
    }

    void ChangeVolume(float delta)
    {
        // Round to steps of 10%, so 0.1 + 0.2 does not become 0.30000001
        float volume = Mathf.Round((SaveManager.Instance.Data.volume + delta) * 10f) / 10f;
        SaveManager.Instance.SetVolume(volume);   // saves right away
        RefreshVolume();
    }

    void RefreshVolume()
    {
        volumeText.text = "Volume: " + Mathf.RoundToInt(SaveManager.Instance.Data.volume * 100f) + "%";
    }

    void ResetProgress()
    {
        SaveManager.Instance.ResetProgress();
        confirmPanel.SetActive(false);
        RefreshLevels();
    }
}
