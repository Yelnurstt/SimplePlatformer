using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// All game sound goes through here, so every sound passes the GameMix mixer.
// Lives in Assets/Resources/AudioManager.prefab. It is created automatically
// before the first scene and survives scene loads (like SaveManager).
//
//   Music: 2 AudioSources (Output = Tracks), crossfade menu <-> level.
//   SFX:   a few AudioSources (Output = SFX), PlayOneShot + random pitch.
//   UI:    1 AudioSource (Output = UI), click on every Button.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer (Assets/Audio/GameMix)")]
    [SerializeField] private AudioMixer mixer;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private string[] menuScenes = { "MainMenu", "WinScreen" };
    [SerializeField] private float crossfadeTime = 1f;   // s

    [Header("Game events")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Variation")]
    [SerializeField] private float minPitch = 0.92f;
    [SerializeField] private float maxPitch = 1.08f;

    private const int SfxSourceCount = 4;   // 4 sounds can have their own pitch at once

    private AudioSource musicA, musicB, currentMusic;
    private AudioSource[] sfxSources;
    private int nextSfx;
    private AudioSource uiSource;
    private AudioClip lastRandomClip;

    private AudioMixerSnapshot normalSnapshot, pausedSnapshot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateOnStart()
    {
        AudioManager prefab = Resources.Load<AudioManager>("AudioManager");
        if (prefab == null)
        {
            Debug.LogError("Assets/Resources/AudioManager.prefab is missing");
            return;
        }
        Instantiate(prefab);
    }

    void Awake()
    {
        // A second copy (should not happen, but safe) removes itself
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // A fixed set of sources, made once. Each one goes to its mixer group.
        AudioMixerGroup tracks = FindGroup("Tracks");
        AudioMixerGroup sfx = FindGroup("SFX");
        AudioMixerGroup ui = FindGroup("UI");

        musicA = MakeSource(tracks, true);
        musicB = MakeSource(tracks, true);
        currentMusic = musicA;

        sfxSources = new AudioSource[SfxSourceCount];
        for (int i = 0; i < SfxSourceCount; i++)
            sfxSources[i] = MakeSource(sfx, false);

        uiSource = MakeSource(ui, false);

        if (mixer != null)
        {
            // Pause sets Time.timeScale = 0: snapshot slides must use real time
            mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
            normalSnapshot = mixer.FindSnapshot("Normal");
            pausedSnapshot = mixer.FindSnapshot("Paused");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // SetFloat does not work in Awake, so saved volumes are applied here
        ApplySavedVolumes();
    }

    AudioSource MakeSource(AudioMixerGroup group, bool loop)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.outputAudioMixerGroup = group;
        s.playOnAwake = false;
        s.loop = loop;
        s.spatialBlend = 0f;   // 2D game: same volume everywhere
        return s;
    }

    AudioMixerGroup FindGroup(string groupName)
    {
        if (mixer == null) return null;
        AudioMixerGroup[] groups = mixer.FindMatchingGroups(groupName);
        if (groups.Length == 0)
        {
            Debug.LogError("Mixer group not found: " + groupName);
            return null;
        }
        return groups[0];
    }

    // ---------- every scene ----------

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPaused(false);   // a new scene is never paused

        bool isMenu = System.Array.IndexOf(menuScenes, scene.name) >= 0;
        PlayMusic(isMenu ? menuMusic : levelMusic);

        // Click sound on every UI button in the scene (also hidden pause panels)
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button b in buttons)
            b.onClick.AddListener(PlayClick);
    }

    // ---------- music ----------

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // Retry reloads the level: the same track just keeps playing
        if (currentMusic.clip == clip && currentMusic.isPlaying) return;

        AudioSource next = currentMusic == musicA ? musicB : musicA;
        next.clip = clip;
        next.volume = 0f;
        next.Play();

        StopAllCoroutines();
        StartCoroutine(Crossfade(currentMusic, next, crossfadeTime));
        currentMusic = next;
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to, float time)
    {
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / time)
        {
            from.volume = 1f - t;
            to.volume = t;
            yield return null;
        }
        from.Stop();
        from.volume = 0f;
        to.volume = 1f;
    }

    // ---------- sound effects ----------

    // Short effect with a small random pitch, so repeats do not sound like a machine gun
    public void PlaySfx(AudioClip clip, float volume = 1f, bool randomPitch = true)
    {
        if (clip == null) return;

        AudioSource s = sfxSources[nextSfx];
        nextSfx = (nextSfx + 1) % sfxSources.Length;

        s.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : 1f;
        s.PlayOneShot(clip, volume);
    }

    // One of 2-3 versions of the same sound, never the same one twice in a row
    public void PlayRandom(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        int i = Random.Range(0, clips.Length);
        if (clips[i] == lastRandomClip && clips.Length > 1)
            i = (i + 1) % clips.Length;

        lastRandomClip = clips[i];
        PlaySfx(clips[i], volume);
    }

    public void PlayClick() { if (clickSound != null) uiSource.PlayOneShot(clickSound, 0.7f); }
    // Win and lose are short melodies: no random pitch, or they sound out of tune
    public void PlayWin() { PlaySfx(winSound, 1f, false); }
    public void PlayLose() { PlaySfx(loseSound, 1f, false); }

    // Player died (HP 0 or fell into a pit)
    public void PlayDeath()
    {
        PlaySfx(deathSound);
        PlayLose();
    }

    // ---------- pause ----------

    public void SetPaused(bool paused)
    {
        AudioMixerSnapshot target = paused ? pausedSnapshot : normalSnapshot;
        if (target != null)
            target.TransitionTo(0.3f);
    }

    // ---------- volume (sliders in the main menu) ----------

    // Slider 0..1 -> decibels. 1 = 0 dB, 0.5 = -6 dB, 0.0001 = -80 dB (silent)
    public static float ToDecibels(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }

    public void SetMusicVolume(float value)
    {
        if (mixer != null) mixer.SetFloat("MusicVol", ToDecibels(value));
    }

    public void SetSfxVolume(float value)
    {
        if (mixer != null) mixer.SetFloat("SFXVol", ToDecibels(value));
    }

    void ApplySavedVolumes()
    {
        SaveData data = SaveManager.Instance.Data;
        SetMusicVolume(data.musicVolume);
        SetSfxVolume(data.sfxVolume);
    }
}
