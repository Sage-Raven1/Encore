using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject gameplayTab;
    public GameObject displayTab;
    public GameObject audioTab;
    public GameObject saveFileTab;
    public GameObject[] hideWhileOpen;

    public AudioMixer masterVol;
    public AudioMixer musicVol;
    public AudioMixer sfxVol;
    public AudioMixer ambienceVol;

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
    public Dropdown fullscreenDropdown;
    public Dropdown resolutionDropdown;
    public Dropdown audioDeviceDropdown;

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
        if (scrollSlider != null) scrollSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingScroll = v / 2f; RefreshLabels(); } });
        if (offsetSlider != null) offsetSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingOffset = Mathf.RoundToInt(v); RefreshLabels(); } });
        if (qualitySlider != null) qualitySlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingQuality = Mathf.RoundToInt(v); RefreshLabels(); } });
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel(masterVol, masterVolumeSlider.value); pendingMasterVol = masterVolumeSlider.value; } });
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel(musicVol, musicVolumeSlider.value); pendingMusicVol = musicVolumeSlider.value; } });
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel(sfxVol, sfxVolumeSlider.value); pendingSfxVol = sfxVolumeSlider.value; } });
        if (ambienceVolumeSlider != null) ambienceVolumeSlider.onValueChanged.AddListener(v => { if (!updatingUI) { AudioLevel(ambienceVol, ambienceVolumeSlider.value); pendingAmbienceVol = ambienceVolumeSlider.value; } });

        //key rebind listeners
        for (int i = 0; i < laneButtons.Length; i++)
        {
            int lane = i;
            if (laneButtons[i] != null)
                laneButtons[i].onClick.AddListener(() => StartRebind(lane));
        }

        //button listeners
        if (resetKeysButton != null) resetKeysButton.onClick.AddListener(ResetToDefaults);
        if (saveCloseButton != null) saveCloseButton.onClick.AddListener(SaveAndClose);
        if (discardButton != null) discardButton.onClick.AddListener(() => TempEmpty()); //change
        if (gameplayButton != null) gameplayButton.onClick.AddListener(() => SettingsTab(gameplayTab));
        if (displayButton != null) displayButton.onClick.AddListener(() => SettingsTab(displayTab));
        if (audioButton != null) audioButton.onClick.AddListener( () => SettingsTab(audioTab));
        if (saveFileButton != null) saveFileButton.onClick.AddListener(() => SettingsTab(saveFileTab));
        if (DELETESAVEFILE != null) DELETESAVEFILE.onClick.AddListener(() => TempEmpty()); //change

        //dropdown listeners
        if (fullscreenDropdown != null) fullscreenDropdown.onValueChanged.AddListener(delegate { pendingFullscreen = fullscreenDropdown.value; });
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(delegate { pendingResolution = resolutionDropdown.value; });

        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void TempEmpty() { }

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
        pendingMusicVol = GameSettings.MusicVolume;
        pendingSfxVol = GameSettings.SfxVolume;
        for (int i = 0; i < pendingKeys.Length; i++)
            pendingKeys[i] = GameSettings.LaneKeys[i];

        listeningLane = -1;

        if (gameplayTab != null) gameplayTab.SetActive(true);
        if (displayTab != null) displayTab.SetActive(false);
        if (audioTab != null) audioTab.SetActive(false);
        if (saveFileTab != null) saveFileTab.SetActive(false);

        RefreshUI();

        foreach (GameObject obj in hideWhileOpen)
            if (obj != null) obj.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void Close()
    {
        listeningLane = -1;
        if (settingsPanel != null) settingsPanel.SetActive(false);
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

    void SettingsTab(GameObject tab)
    {
        if (tab != null)
        {
            if (gameplayTab != null) gameplayTab.SetActive(false);
            if (displayTab != null) displayTab.SetActive(false);
            if (audioTab != null) audioTab.SetActive(false);
            if (saveFileTab != null) saveFileTab.SetActive(false);

            tab.SetActive(true);
        }
    }
/*
    void GameplayTab() { if (gameplayTab != null) gameplayTab.SetActive(true); }
    void DisplayTab() { if (gameplayTab != null) gameplayTab.SetActive(true); }
    void AudioTab() { if (gameplayTab != null) gameplayTab.SetActive(true); }
    void SaveFileTab() { if (gameplayTab != null) gameplayTab.SetActive(false); }

    void TurnOffOtherSettingTabs()
    {
        if (gameplayTab != null) gameplayTab.SetActive(false);

    } */

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
        GameSettings.MusicVolume = pendingMusicVol;
        GameSettings.SfxVolume = pendingSfxVol;
        for (int i = 0; i < pendingKeys.Length; i++)
            GameSettings.LaneKeys[i] = pendingKeys[i];

        GameSettings.Save();
        Close();
    }

    // refreshing UI after changes
    void RefreshUI()
    {
        updatingUI = true;
        if (scrollSlider != null) scrollSlider.value = pendingScroll * 2f; // times two due to drop height being a bit lower than average
        if (offsetSlider != null) offsetSlider.value = pendingOffset;
        if (musicVolumeSlider != null) musicVolumeSlider.value = Mathf.Round(pendingMusicVol * 100f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = Mathf.Round(pendingSfxVol * 100f);
        updatingUI = false;

        RefreshLabels();
    }

    // refreshing the value text
    void RefreshLabels()
    {
        if (scrollValue != null) scrollValue.text = pendingScroll.ToString("0.0");
        if (offsetValue != null) offsetValue.text = pendingOffset.ToString("+0;-0;0");
        if (musicVolumeValue != null) musicVolumeValue.text = Mathf.RoundToInt(pendingMusicVol * 100f) + "%";
        if (sfxVolumeValue != null) sfxVolumeValue.text = Mathf.RoundToInt(pendingSfxVol * 100f) + "%";

        for (int i = 0; i < laneButtons.Length; i++)
        {
            if (laneButtons[i] == null) continue;
            TMP_Text label = laneButtons[i].GetComponentInChildren<TMP_Text>();
            if (label == null) continue;
            label.text = i == listeningLane ? "?" : KeyName(pendingKeys[i]);
        }
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

    public void AudioLevel(AudioMixer mixer, float sliderVal)
    {
        sliderVal = sliderVal / 100;
        sliderVal = (sliderVal == 0) ? 0.0001f : sliderVal;
        mixer.SetFloat(mixer.ToString(), Mathf.Log10(sliderVal * 20));
    }
}