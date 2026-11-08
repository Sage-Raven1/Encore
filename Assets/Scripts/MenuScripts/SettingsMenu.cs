using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject gameplayTab;
    public GameObject displayTab;
    public GameObject audioTab;
    public GameObject saveFileTab;
    public GameObject[] hideWhileOpen;

    public AudioMixer mixer;

    // sliders
    public Slider scrollSlider;
    public Slider offsetSlider;
    public Slider qualitySlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider ambienceVolumeSlider;

    // value text
    public TMP_Text scrollValue;
    public TMP_Text offsetValue;
    public TMP_Text qualityValue;
    public TMP_Text masterVolumeValue;
    public TMP_Text musicVolumeValue;
    public TMP_Text sfxVolumeValue;
    public TMP_Text ambienceVolumeValue;
    public TMP_Text scoreValue;
    public TMP_Text completionValue;

    // keybinds
    public Button[] laneButtons = new Button[4];

    // buttons (specifically in the settings panel)
    public Button saveCloseButton;
    public Button discardButton;
    public Button gameplayButton;
    public Button resetKeysButton;
    public Button displayButton;
    public Button audioButton;
    public Button saveFileButton;
    public Button DELETESAVEFILE;

    //dropdowns
    public TMP_Dropdown fullscreenDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown audioDeviceDropdown;

    // pendings settings (changeable)
    float pendingScroll;
    int pendingOffset;
    int pendingQuality;
    int pendingFullscreen;
    int pendingResolution;
    float pendingMasterVol;
    float pendingMusicVol;
    float pendingSfxVol;
    float pendingAmbienceVol;

    Key[] pendingKeys = new Key[4];

    int listeningLane = -1;
    int listenStartFrame;
    bool updatingUI;
    Resolution[] resolutions;

    private void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> resolutionList = new List<string>();
        resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " X " + resolutions[i].height;
            resolutionList.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) pendingResolution = i;
        }
        SwitchResolution(pendingResolution);
        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.RefreshShownValue();
        RefreshQuality(GameSettings.Quality);
    }
    void Awake()
    {
        // baseline restrictions for sliders
        SetupSlider(scrollSlider, 2, 20);
        SetupSlider(offsetSlider, -100, 100);
        SetupSlider(qualitySlider, 0, 5);
        SetupSlider(masterVolumeSlider, 0, 100);
        SetupSlider(musicVolumeSlider, 0, 100);
        SetupSlider(sfxVolumeSlider, 0, 100);
        SetupSlider(ambienceVolumeSlider, 0, 100);

        //slider listeners
        scrollSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingScroll = v / 2f; RefreshLabels(); } });
        offsetSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingOffset = Mathf.RoundToInt(v); RefreshLabels(); } });
        qualitySlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingQuality = Mathf.RoundToInt(v); RefreshQuality(v); } });
        masterVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel("MasterVolume", masterVolumeSlider.value); pendingMasterVol = masterVolumeSlider.value; } });
        musicVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel("MusicVolume", musicVolumeSlider.value); pendingMusicVol = musicVolumeSlider.value; } });
        sfxVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel("SFXVolume", sfxVolumeSlider.value); pendingSfxVol = sfxVolumeSlider.value; } });
        ambienceVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel("AmbienceVolume", ambienceVolumeSlider.value); pendingAmbienceVol = ambienceVolumeSlider.value; } });

        //key rebind listeners
        for (int i = 0; i < laneButtons.Length; i++)
        {
            int lane = i;
            if (laneButtons[i] != null)
                laneButtons[i].onClick.AddListener(() => StartRebind(lane));
        }

        //button listeners
        resetKeysButton.onClick.AddListener(ResetToDefaults);
        saveCloseButton.onClick.AddListener(SaveAndClose);
        discardButton.onClick.AddListener(DiscardChanges);
        gameplayButton.onClick.AddListener(() => SettingsTab(gameplayTab));
        displayButton.onClick.AddListener(() => SettingsTab(displayTab));
        audioButton.onClick.AddListener(() => SettingsTab(audioTab));
        saveFileButton.onClick.AddListener(() => SettingsTab(saveFileTab));
        DELETESAVEFILE.onClick.AddListener(DeleteSaveFile);

        //dropdown listeners
        fullscreenDropdown.onValueChanged.AddListener(delegate
        {
            pendingFullscreen = fullscreenDropdown.value;
            SwitchFullscreen(pendingFullscreen);
        });
        resolutionDropdown.onValueChanged.AddListener(delegate
        {
            pendingResolution = resolutionDropdown.value;
            SwitchResolution(pendingResolution);
        });

        settingsPanel.SetActive(false);
    }

    // slider 
    void SetupSlider(Slider slider, float min, float max)
    {
        if (slider == null) return;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = true;
    }

    public void Open()
    {
        pendingScroll = GameSettings.ScrollSpeed;
        pendingOffset = GameSettings.AudioOffset;
        pendingQuality = GameSettings.Quality;
        pendingFullscreen = GameSettings.DefaultFullscreen;
        pendingResolution = GameSettings.Resolution;
        pendingMasterVol = GameSettings.MasterVolume;
        pendingMusicVol = GameSettings.MusicVolume;
        pendingSfxVol = GameSettings.SfxVolume;
        pendingAmbienceVol = GameSettings.AmbienceVolume;
        pendingMusicVol = GameSettings.MusicVolume;
        pendingSfxVol = GameSettings.SfxVolume;

        for (int i = 0; i < pendingKeys.Length; i++)
            pendingKeys[i] = GameSettings.LaneKeys[i];

        listeningLane = -1;

        RefreshUI();
        RefreshQuality(pendingQuality);

        foreach (GameObject obj in hideWhileOpen)
            if (obj != null) obj.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void Close()
    {
        listeningLane = -1;
        settingsPanel.SetActive(false);
        foreach (GameObject obj in hideWhileOpen)
            if (obj != null) obj.SetActive(true);
    }

    void Update()
    {
        if (settingsPanel == null || !settingsPanel.activeSelf) return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (listeningLane >= 0)
        {
            if (Time.frameCount == listenStartFrame) return;

            if (kb.escapeKey.wasPressedThisFrame)
            {
                listeningLane = -1;
                RefreshLabels();
                return;
            }

            foreach (KeyControl key in kb.allKeys)
            {
                if (key != null && key.wasPressedThisFrame)
                {
                    AssignKey(listeningLane, key.keyCode);
                    return;
                }
            }
            return;
        }

        if (kb.escapeKey.wasPressedThisFrame)
            Close();
    }

    public void SettingsTab(GameObject tab)
    {
        if (tab != null)
        {
            gameplayTab.SetActive(false);
            displayTab.SetActive(false);
            audioTab.SetActive(false);
            saveFileTab.SetActive(false);

            tab.SetActive(true);
        }
    }

    // rebind 
    void StartRebind(int lane)
    {
        listeningLane = lane;
        listenStartFrame = Time.frameCount;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        RefreshLabels();
    }

    // assigning key binds
    void AssignKey(int lane, Key key)
    {
        for (int i = 0; i < pendingKeys.Length; i++)
        {
            if (i != lane && pendingKeys[i] == key)
                pendingKeys[i] = pendingKeys[lane];
        }
        pendingKeys[lane] = key;
        listeningLane = -1;
        RefreshLabels();
    }

    // reset to default values 
    public void ResetToDefaults()
    {
        pendingScroll = GameSettings.DefaultScrollSpeed;
        pendingOffset = GameSettings.DefaultAudioOffset;
        pendingMusicVol = GameSettings.DefaultMusicVolume;
        pendingSfxVol = GameSettings.DefaultSfxVolume;
        for (int i = 0; i < pendingKeys.Length; i++)
            pendingKeys[i] = GameSettings.DefaultLaneKeys[i];

        listeningLane = -1;
        RefreshUI();
    }

    // save the previous values
    public void SaveAndClose()
    {
        GameSettings.ScrollSpeed = pendingScroll;
        GameSettings.AudioOffset = pendingOffset;
        GameSettings.Quality = pendingQuality;
        GameSettings.Fullscreen = pendingFullscreen;
        GameSettings.Resolution = pendingResolution;
        GameSettings.MusicVolume = pendingMusicVol;
        GameSettings.SfxVolume = pendingSfxVol;
        GameSettings.AmbienceVolume = pendingAmbienceVol;

        for (int i = 0; i < pendingKeys.Length; i++)
            GameSettings.LaneKeys[i] = pendingKeys[i];

        GameSettings.Save();
        Close();
    }

    public void DiscardChanges()
    {
        pendingScroll = GameSettings.ScrollSpeed;
        pendingOffset = GameSettings.AudioOffset;
        pendingQuality = GameSettings.Quality;
        pendingFullscreen = GameSettings.DefaultFullscreen;
        pendingResolution = GameSettings.Resolution;
        pendingMasterVol = GameSettings.MasterVolume;
        pendingMusicVol = GameSettings.MusicVolume;
        pendingSfxVol = GameSettings.SfxVolume;
        pendingAmbienceVol = GameSettings.AmbienceVolume;
        pendingMusicVol = GameSettings.MusicVolume;
        pendingSfxVol = GameSettings.SfxVolume;

        for (int i = 0; i < pendingKeys.Length; i++)
            GameSettings.LaneKeys[i] = pendingKeys[i];

        RefreshUI();
    }

    // refreshing UI after changes
    void RefreshUI()
    {
        updatingUI = true;
        scrollSlider.value = pendingScroll * 2f; // times two due to drop height being a bit lower than average
        offsetSlider.value = pendingOffset;
        qualitySlider.value = pendingQuality;
        masterVolumeSlider.value = Mathf.Round(pendingMasterVol);
        musicVolumeSlider.value = Mathf.Round(pendingMusicVol);
        sfxVolumeSlider.value = Mathf.Round(pendingSfxVol);
        ambienceVolumeSlider.value = Mathf.Round(pendingAmbienceVol);
        updatingUI = false;

        RefreshLabels();
    }

    // refreshing the value text
    void RefreshLabels()
    {
        scrollValue.text = pendingScroll.ToString("0.0");
        offsetValue.text = pendingOffset.ToString("+0;-0;0");
        masterVolumeValue.text = Mathf.RoundToInt(pendingMasterVol) + "%";
        musicVolumeValue.text = Mathf.RoundToInt(pendingMusicVol) + "%";
        sfxVolumeValue.text = Mathf.RoundToInt(pendingSfxVol) + "%";
        ambienceVolumeValue.text = Mathf.RoundToInt(pendingAmbienceVol) + "%";

        for (int i = 0; i < laneButtons.Length; i++)
        {
            if (laneButtons[i] == null) continue;
            TMP_Text label = laneButtons[i].GetComponentInChildren<TMP_Text>();
            if (label == null) continue;
            label.text = i == listeningLane ? "?" : KeyName(pendingKeys[i]);
        }
    }

    void RefreshQuality(float val)
    {
        switch (qualitySlider.value)
        {
            case 0:
                qualityValue.text = "Very Low";
                break;
            case 1:
                qualityValue.text = "Low";
                break;
            case 2:
                qualityValue.text = "Medium";
                break;
            case 3:
                qualityValue.text = "High";
                break;
            case 4:
                qualityValue.text = "Very High";
                break;
            case 5:
                qualityValue.text = "Ultra";
                break;
        }
        int quality = (int)val;
        QualitySettings.SetQualityLevel(quality);
        qualitySlider.value = pendingQuality;
    }

    void SwitchFullscreen(int val)
    {
        Debug.Log("Fullscreen switcher = ");
        switch (val)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Debug.Log("Fullscreen");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Debug.Log("Borderless");
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Debug.Log("Windowed");
                break;
        }

    }

    void SwitchResolution(int val)
    {
        Resolution resolution = resolutions[val];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    // key names of keybind settings
    string KeyName(Key key)
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && key != Key.None)
        {
            string name = kb[key].displayName;
            if (!string.IsNullOrEmpty(name)) return name.ToUpper();
        }
        return key.ToString().ToUpper();
    }

    public void AudioLevel(string channel, float sliderVal)
    {
        sliderVal = sliderVal / 100;
        sliderVal = (sliderVal == 0) ? 0.0001f : sliderVal;
        mixer.SetFloat(channel, Mathf.Log10(sliderVal) * 20);
    }

    public void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/Save/player.savefile";
        {
            string savefolder = Application.persistentDataPath + "/Save";
            if (Directory.Exists(savefolder))
            {
                Debug.Log("Save folder found successfully");
            }
            else
            {
                Debug.Log("Save folder not found");
                Debug.Log("creating folder...");
                Directory.CreateDirectory(Application.persistentDataPath + "/Save");
            }
        }
        if (File.Exists(path))
        {
            Debug.Log("Savefile exists, removing...");
            File.Delete(path);
            Debug.Log("Done");
        }
        else
        {
            Debug.Log("No savefile found");
        }
    }
}