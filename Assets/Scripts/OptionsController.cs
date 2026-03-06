using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("UI")]
    public Slider sensitivitySlider;

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider musicSlider;

    [Header("SFX Settings")]
    public Slider sfxSlider;

    [Header("Vibration")]
    public Button vibrationButton;
    public TextMeshProUGUI vibrationText;

    [Header("Confirmation UI")]
    public GameObject confirmationPanel;
    public Button yesButton;
    public Button noButton;

    void Start()
    {
        if(confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
        LoadOptions();

        LocalizationManager.Instance.OnLanguageChanged += UpdateVibrationUI;
    }

    void LoadOptions()
    {
        var data = SaveManager.Instance.data;

        if (data.sensitivity <= 0f)
            data.sensitivity = 1f;

        sensitivitySlider.minValue = 0.5f;
        sensitivitySlider.maxValue = 3f;

        sensitivitySlider.value = data.sensitivity;

        UpdateVibrationUI();

        // MUSIC
        musicSlider.value = data.musicVolume;

        // SFX
        sfxSlider.value = data.sfxVolume;
    }

    // 🔊 SES AYARLARI

    // Slider'ın OnValueChanged olayına bunu bağla (Dynamic Float olarak)
    public void OnMusicSliderChanged(float value)
    {
        SaveManager.Instance.data.musicVolume = value;
        SaveManager.Instance.SaveGame();
        SetMusicVolume(value);
    }

    void SetMusicVolume(float value)
    {
        // Slider 0-1 arasındaysa, bunu Mixer'ın anlayacağı Desibel (-80 ile 20) değerine çevirir
        // Logaritmik olması daha doğal bir ses azalması sağlar
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("MusicVol", dB);
    }

    // Slider'a bağla (Dynamic Float)
    public void OnSFXSliderChanged(float value)
    {
        SaveManager.Instance.data.sfxVolume = value;
        SaveManager.Instance.SaveGame();
        SetSFXVolume(value);
    }

    void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("SFXVol", dB);
    }

    // 🎮 SENSITIVITY
    public void OnSensitivityChanged(float value)
    {
        SaveManager.Instance.data.sensitivity = value;
        SaveManager.Instance.SaveGame();  
    }

    // 📳 VIBRATION BUTTON
    public void OnVibrationButtonClicked()
    {
        SaveManager.Instance.data.vibrationEnabled =
            !SaveManager.Instance.data.vibrationEnabled;

        UpdateVibrationUI();

        // Açıldıysa küçük test titreşimi
        if (SaveManager.Instance.data.vibrationEnabled)
        SaveManager.Instance.Vibrate();
    }

    void UpdateVibrationUI()
    {
        string key = SaveManager.Instance.data.vibrationEnabled
        ? "VIBRATION_ON"
        : "VIBRATION_OFF";

        vibrationText.text =
            LocalizationManager.Instance.GetText(key);
    }

    // 🌍 LANGUAGE BUTTONS
    public void SetEnglish()
    {
        SaveManager.Instance.data.languageIndex = 0;
        SaveManager.Instance.SaveGame();

        LocalizationManager.Instance.LoadLanguage(0);
    }

    public void SetTurkish()
    {
        SaveManager.Instance.data.languageIndex = 1;
        SaveManager.Instance.SaveGame();

        LocalizationManager.Instance.LoadLanguage(1);
    }

    // START TUTORIAL
    public void OnRestartTutorialClicked()
    {
        SaveManager.Instance.cameFromOptions = true; // Options'tan geldiğini işaretle
        SceneManager.LoadScene("tutorialScene");
    }

    // DELETE ALL DATA
    public void OnDeleteAllDataClicked()
    {
        // Popup aç
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
    }

    // "Yes" butonuna basınca
    public void OnConfirmDeleteYes()
    {
        // Reset işlemi
        SaveManager.Instance.ResetAllData();

        // UI güncelle
        LoadOptions();

        // Popup kapat
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    // "No" butonuna basınca
    public void OnConfirmDeleteNo()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateVibrationUI;
    }
}

