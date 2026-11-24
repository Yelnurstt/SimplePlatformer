using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Main menu: level select (locked / unlocked from save.json),
// Music and SFX volume sliders and "Reset progress" with a confirm panel.
public class MainMenuUI : MonoBehaviour
{
    [Header("Level select")]
    [SerializeField] private LevelData[] levels;        // Level_01, Level_02, Level_03
    [SerializeField] private Button[] levelButtons;     // one button for each level

    [Header("Volume (sliders: Min 0.0001, Max 1)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioClip sfxPreview;   // plays when the SFX slider moves

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

        // Put the saved values on the sliders first, THEN listen for changes
        SaveData data = SaveManager.Instance.Data;
        musicSlider.SetValueWithoutNotify(data.musicVolume);
        sfxSlider.SetValueWithoutNotify(data.sfxVolume);
        musicSlider.onValueChanged.AddListener(SaveManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        resetButton.onClick.AddListener(() => confirmPanel.SetActive(true));
        noButton.onClick.AddListener(() => confirmPanel.SetActive(false));
        yesButton.onClick.AddListener(ResetProgress);

        RefreshLevels();
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

    void OnSfxChanged(float value)
    {
        SaveManager.Instance.SetSfxVolume(value);   // mixer + save.json

        // Short sound so the player hears the new SFX level (not on every tiny move)
        if (Time.unscaledTime - lastPreviewTime > 0.25f)
        {
            AudioManager.Instance.PlaySfx(sfxPreview);
            lastPreviewTime = Time.unscaledTime;
        }
    }

    private float lastPreviewTime;

    void ResetProgress()
    {
        SaveManager.Instance.ResetProgress();
        confirmPanel.SetActive(false);
        RefreshLevels();
    }
}
