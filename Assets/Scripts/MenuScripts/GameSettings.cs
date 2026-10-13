using UnityEngine;
using UnityEngine.InputSystem;

public static class GameSettings
{
    // setting up default values
    public const float DefaultScrollSpeed = 5f;
    public const int DefaultAudioOffset = 0;
    public const float DefaultMusicVolume = 0.8f;
    public const float DefaultSfxVolume = 0.8f;
    public static readonly Key[] DefaultLaneKeys = { Key.A, Key.S, Key.L, Key.Semicolon };

    // assigning the values to each corresponding aspect of the game
    public static float ScrollSpeed = DefaultScrollSpeed;
    public static int AudioOffset = DefaultAudioOffset;
    public static float MusicVolume = DefaultMusicVolume;
    public static float SfxVolume = DefaultSfxVolume;
    public static Key[] LaneKeys = (Key[])DefaultLaneKeys.Clone();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        ScrollSpeed = PlayerPrefs.GetFloat("ScrollSpeed", DefaultScrollSpeed);
        AudioOffset = PlayerPrefs.GetInt("AudioOffset", DefaultAudioOffset);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", DefaultMusicVolume);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", DefaultSfxVolume);

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
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);

        for (int i = 0; i < LaneKeys.Length; i++)
            PlayerPrefs.SetInt("LaneKey" + i, (int)LaneKeys[i]);

        PlayerPrefs.Save();
    }
}