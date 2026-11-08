using UnityEngine;
using UnityEngine.InputSystem;

public static class GameSettings
{
    // setting up default values
    public const float DefaultScrollSpeed = 5f;
    public const int DefaultAudioOffset = 0;
    public const int DefaultQuality = 2;
    public const int DefaultFullscreen = 0;
    public const int DefaultResolution = 0;
    public const float DefaultMasterVolume = 70;
    public const float DefaultMusicVolume = 70;
    public const float DefaultSfxVolume = 70;
    public const float DefaultAmbienceVolume = 70;
    public static readonly Key[] DefaultLaneKeys = { Key.A, Key.S, Key.L, Key.Semicolon };

    // assigning the values to each corresponding aspect of the game
    public static float ScrollSpeed = DefaultScrollSpeed;
    public static int AudioOffset = DefaultAudioOffset;
    public static int Quality = DefaultQuality;
    public static int Fullscreen = DefaultFullscreen;
    public static int Resolution = DefaultResolution;
    public static float MasterVolume = DefaultMasterVolume;
    public static float MusicVolume = DefaultMusicVolume;
    public static float SfxVolume = DefaultSfxVolume;
    public static float AmbienceVolume = DefaultAmbienceVolume;
    public static Key[] LaneKeys = (Key[])DefaultLaneKeys.Clone();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        ScrollSpeed = PlayerPrefs.GetFloat("ScrollSpeed", DefaultScrollSpeed);
        AudioOffset = PlayerPrefs.GetInt("AudioOffset", DefaultAudioOffset);
        Quality = PlayerPrefs.GetInt("Quality", DefaultQuality);
        Fullscreen = PlayerPrefs.GetInt("Fullscreen", DefaultFullscreen);
        Resolution = PlayerPrefs.GetInt("Resolution", DefaultResolution);
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", DefaultMasterVolume);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", DefaultMusicVolume);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", DefaultSfxVolume);
        AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", DefaultAmbienceVolume);

        for (int i = 0; i < LaneKeys.Length; i++)
        {
            int saved = PlayerPrefs.GetInt("LaneKey" + i, (int)DefaultLaneKeys[i]);
            bool valid = System.Enum.IsDefined(typeof(Key), saved) && saved != (int)Key.None;
            LaneKeys[i] = valid ? (Key)saved : DefaultLaneKeys[i];
        }
    }

    // saving player prefs so it works in game
    public static void Save()
    {
        PlayerPrefs.SetFloat("ScrollSpeed", ScrollSpeed);
        PlayerPrefs.SetInt("AudioOffset", AudioOffset);
        PlayerPrefs.SetInt("Quality", Quality);
        PlayerPrefs.SetInt("Fullscreen", Fullscreen);
        PlayerPrefs.SetInt("Resolution", Resolution);
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        PlayerPrefs.SetFloat("AmbienceVolume", AmbienceVolume);


        for (int i = 0; i < LaneKeys.Length; i++)
            PlayerPrefs.SetInt("LaneKey" + i, (int)LaneKeys[i]);

        PlayerPrefs.Save();
    }
}