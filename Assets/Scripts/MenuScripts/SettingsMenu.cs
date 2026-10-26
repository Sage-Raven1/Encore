using System.Reflection;
using TMPro;
using UnityEngine;
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

    
    // sliders
    public Slider scrollSlider;
    public Slider offsetSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

   // value text
    public TMP_Text scrollValue;
    public TMP_Text offsetValue;
    public TMP_Text musicValue;
    public TMP_Text sfxValue;

    // keybinds
    public Button[] laneButtons = new Button[4];

    // buttons (specifically in the settings panel)
    public Button resetButton;
    public Button saveButton;
    public Button saveCloseButton;
    public Button discardButton;
    public Button gameplayButton;
    public Button displayButton;
    public Button audioButton;
    public Button saveFileButton;


    // pendings settings (changeable)
    float pendingScroll;
    int pendingOffset;
    float pendingMusic;
    float pendingSfx;
    Key[] pendingKeys = new Key[4];

    int listeningLane = -1;
    int listenStartFrame;
    bool updatingUI;

    void Awake()
    {
        // baseline restrictions for sliders
        SetupSlider(scrollSlider, 2, 20);
        SetupSlider(offsetSlider, -100, 100);
        SetupSlider(musicSlider, 0, 100);
        SetupSlider(sfxSlider, 0, 100);

        if (scrollSlider != null) scrollSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingScroll = v / 2f; RefreshLabels(); } });
        if (offsetSlider != null) offsetSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingOffset = Mathf.RoundToInt(v); RefreshLabels(); } });
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingMusic = v / 100f; RefreshLabels(); } });
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(v => { if (!updatingUI) { pendingSfx = v / 100f; RefreshLabels(); } });

        for (int i = 0; i < laneButtons.Length; i++)
        {
            int lane = i;
            if (laneButtons[i] != null)
                laneButtons[i].onClick.AddListener(() => StartRebind(lane));
        }

        if (resetButton != null) resetButton.onClick.AddListener(ResetToDefaults);
        if (saveButton != null) saveButton.onClick.AddListener(SaveAndClose);
        if (saveCloseButton != null) saveCloseButton.onClick.AddListener(SaveAndClose);
        if (gameplayButton != null) gameplayButton.onClick.AddListener(() => SettingsTab(gameplayTab));
        if (displayButton != null) displayButton.onClick.AddListener(() => SettingsTab(displayTab));
        if (audioButton != null) audioButton.onClick.AddListener( () => SettingsTab(audioTab));
        if (saveFileButton != null) saveFileButton.onClick.AddListener(() => SettingsTab(saveFileTab));

        if (settingsPanel != null) settingsPanel.SetActive(false);
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
        pendingMusic = GameSettings.MusicVolume;
        pendingSfx = GameSettings.SfxVolume;
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
        pendingMusic = GameSettings.DefaultMusicVolume;
        pendingSfx = GameSettings.DefaultSfxVolume;
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
        GameSettings.MusicVolume = pendingMusic;
        GameSettings.SfxVolume = pendingSfx;
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
        if (musicSlider != null) musicSlider.value = Mathf.Round(pendingMusic * 100f);
        if (sfxSlider != null) sfxSlider.value = Mathf.Round(pendingSfx * 100f);
        updatingUI = false;

        RefreshLabels();
    }

    // refreshing the value text
    void RefreshLabels()
    {
        if (scrollValue != null) scrollValue.text = pendingScroll.ToString("0.0");
        if (offsetValue != null) offsetValue.text = pendingOffset.ToString("+0;-0;0");
        if (musicValue != null) musicValue.text = Mathf.RoundToInt(pendingMusic * 100f) + "%";
        if (sfxValue != null) sfxValue.text = Mathf.RoundToInt(pendingSfx * 100f) + "%";

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
}